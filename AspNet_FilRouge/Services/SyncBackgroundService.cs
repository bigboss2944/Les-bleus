using AspNet_FilRouge.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AspNet_FilRouge.Services
{
    /// <summary>
    /// Service en arrière-plan qui synchronise automatiquement la base centrale
    /// vers la base locale SQLite toutes les 5 minutes.
    /// Il importe également les demandes de stock depuis la base locale du Vendeur.
    /// </summary>
    public class SyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILocalDbService _localDb;
        private readonly IVendorSyncService _vendorSync;
        private readonly IWebHostEnvironment _env;
        private readonly string _vendeurCachePath;
        private readonly ILogger<SyncBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public SyncBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILocalDbService localDb,
            IVendorSyncService vendorSync,
            IWebHostEnvironment env,
            IConfiguration configuration,
            ILogger<SyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _localDb = localDb;
            _vendorSync = vendorSync;
            _env = env;
            _logger = logger;

            var relPath = configuration["Sync:VendeurCachePath"]
                ?? "../AspNet_FilRouge_Vendeur/local_cache.db";
            _vendeurCachePath = Path.GetFullPath(Path.Combine(_env.ContentRootPath, relPath));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Synchronisation automatique démarrée (intervalle : {Interval}).", _interval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la synchronisation automatique de la base locale.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Synchronisation automatique arrêtée.");
        }

        private async Task SyncAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var orders = await db.Orders
                    .Include(o => o.Seller)
                    .Include(o => o.Customer)
                    .Include(o => o.Shop)
                    .Include(o => o.Bicycles)
                    .ToListAsync();

                var bicycles = await db.Bicycles
                    .Include(b => b.Order)
                    .Include(b => b.Shop)
                    .ToListAsync();

                var sellers = await db.Sellers.ToListAsync();

                var customers = await db.Customers.ToListAsync();

                // Écriture atomique en une seule transaction pour éviter les verrous répétés.
                await _localDb.BulkUpsertAllAsync(orders, bicycles, sellers, customers);

                _logger.LogInformation(
                    "Synchronisation des données centrales : {Orders} ordres, {Bicycles} vélos, {Sellers} vendeurs, {Customers} clients.",
                    orders.Count, bicycles.Count, sellers.Count, customers.Count);

                var flushed = await _vendorSync.FlushPendingUpdatesAsync();
                if (flushed > 0)
                {
                    _logger.LogInformation("Replay offline applique: {Count} mise(s) a jour vers le vendeur.", flushed);
                }

                // Importer les demandes de stock depuis la base locale du Vendeur.
                var importedCount = await ImportStockRequestsFromVendorAsync(db);
                _logger.LogInformation(
                    "Stock requests: {ImportedCount} imported/updated from vendor cache.", importedCount);

                // Synchroniser toutes les demandes de stock vers le cache local
                var allRequests = await db.StockRequests.ToListAsync();
                await _localDb.BulkUpsertStockRequestsAsync(allRequests);
                _logger.LogInformation(
                    "Synchronisation Stock Requests: {Count} synced to local cache.", allRequests.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans SyncAsync");
                throw;
            }
        }

        /// <summary>
        /// Lit les demandes de stock depuis la base locale SQLite du Vendeur,
        /// insere les nouvelles et met a jour les existantes dans la base Admin.
        /// RequestedById est neutralise si l'utilisateur n'existe pas en base centrale
        /// afin d'eviter les erreurs de contrainte FK SQLite.
        /// </summary>
        private async Task<int> ImportStockRequestsFromVendorAsync(ApplicationDbContext db)
        {
            if (!File.Exists(_vendeurCachePath))
            {
                _logger.LogDebug("Fichier cache vendeur non trouvé : {Path}", _vendeurCachePath);
                return 0;
            }

            var vendorRequests = ReadStockRequestsFromCache(_vendeurCachePath);
            if (vendorRequests.Count == 0)
            {
                _logger.LogDebug("Aucune demande de stock dans le cache vendeur.");
                return 0;
            }

            var existingIds = new HashSet<int>(await db.StockRequests.Select(r => r.Id).ToListAsync());
            var existingUserIds = new HashSet<string>(await db.Users.Select(u => u.Id).ToListAsync());
            int insertCount = 0, updateCount = 0;
            var dbRequests = new Dictionary<int, StockRequest>();

            // Charger les demandes existantes pour optimiser les mises à jour
            if (existingIds.Count > 0)
            {
                var vendorIds = vendorRequests.Select(vr => vr.Id).ToList();
                foreach (var req in await db.StockRequests.Where(r => vendorIds.Contains(r.Id)).ToListAsync())
                {
                    dbRequests[req.Id] = req;
                }
            }

            foreach (var vr in vendorRequests)
            {
                var requestedById = vr.RequestedById;
                if (!string.IsNullOrWhiteSpace(requestedById) && !existingUserIds.Contains(requestedById))
                {
                    _logger.LogWarning(
                        "Demande de stock Id={RequestId}: RequestedById '{RequestedById}' introuvable dans AspNetUsers. Valeur ignoree.",
                        vr.Id, requestedById);
                    requestedById = null;
                }

                if (existingIds.Contains(vr.Id))
                {
                    // Mise à jour de demande existante
                    if (dbRequests.TryGetValue(vr.Id, out var existing))
                    {
                        var changed = false;
                        if (existing.BicycleName != vr.BicycleName)
                        {
                            existing.BicycleName = vr.BicycleName;
                            changed = true;
                        }
                        if (existing.Quantity != vr.Quantity)
                        {
                            existing.Quantity = vr.Quantity;
                            changed = true;
                        }
                        if (existing.Notes != vr.Notes)
                        {
                            existing.Notes = vr.Notes;
                            changed = true;
                        }
                        if (existing.RequestedById != requestedById)
                        {
                            existing.RequestedById = requestedById;
                            changed = true;
                        }
                        // Note: On ne met pas à jour le Status depuis le vendeur car 
                        // c'est l'admin qui décide du statut final

                        if (changed)
                        {
                            db.StockRequests.Update(existing);
                            updateCount++;
                        }
                    }
                }
                else
                {
                    // Nouvelle demande
                    db.StockRequests.Add(new StockRequest
                    {
                        Id = vr.Id,
                        BicycleName = vr.BicycleName,
                        Quantity = vr.Quantity,
                        RequestDate = vr.RequestDate,
                        Status = vr.Status,
                        RequestedById = requestedById,
                        Notes = vr.Notes
                    });
                    insertCount++;
                }
            }

            if (insertCount > 0 || updateCount > 0)
            {
                await db.SaveChangesAsync();
                _logger.LogInformation(
                    "Stock Requests: {Inserted} new, {Updated} updated from vendor cache.",
                    insertCount, updateCount);
            }

            return insertCount + updateCount;
        }

        private sealed record VendorStockRequest(
            int Id,
            string BicycleName,
            int Quantity,
            DateTime RequestDate,
            string Status,
            string? RequestedById,
            string? Notes);

        private List<VendorStockRequest> ReadStockRequestsFromCache(string cachePath)
        {
            var results = new List<VendorStockRequest>();

            using var connection = new SqliteConnection($"Filename={cachePath}");
            connection.Open();

            // Vérifier que la table existe (le Vendeur a peut-être une ancienne version du cache).
            using var checkCmd = new SqliteCommand(
                "SELECT name FROM sqlite_master WHERE type='table' AND name='StockRequests';", connection);
            if (checkCmd.ExecuteScalar() == null) return results;

            using var cmd = new SqliteCommand(
                "SELECT Id, BicycleName, Quantity, RequestDate, Status, RequestedById, Notes FROM StockRequests",
                connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var dateStr = reader.GetString(3);
                if (!DateTime.TryParse(dateStr, null,
                        System.Globalization.DateTimeStyles.RoundtripKind, out var requestDate))
                {
                    _logger.LogWarning(
                        "Impossible de parser la date '{DateStr}' pour la demande de stock Id={Id}. Enregistrement ignoré.",
                        dateStr, reader.GetInt32(0));
                    continue;
                }

                results.Add(new VendorStockRequest(
                    Id: reader.GetInt32(0),
                    BicycleName: reader.GetString(1),
                    Quantity: reader.GetInt32(2),
                    RequestDate: requestDate,
                    Status: reader.GetString(4),
                    RequestedById: reader.IsDBNull(5) ? null : reader.GetString(5),
                    Notes: reader.IsDBNull(6) ? null : reader.GetString(6)));
            }

            return results;
        }
    }
}
