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
        private readonly LocalDbService _localDb;
        private readonly IWebHostEnvironment _env;
        private readonly string _vendeurCachePath;
        private readonly ILogger<SyncBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public SyncBackgroundService(
            IServiceScopeFactory scopeFactory,
            LocalDbService localDb,
            IWebHostEnvironment env,
            IConfiguration configuration,
            ILogger<SyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _localDb = localDb;
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
        }

        private async Task SyncAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
                "Synchronisation automatique terminée — {Orders} commandes, {Bicycles} vélos, {Sellers} vendeurs, {Customers} clients.",
                orders.Count, bicycles.Count, sellers.Count, customers.Count);

            // Importer les demandes de stock depuis la base locale du Vendeur.
            var imported = await ImportStockRequestsFromVendorAsync(db);
            if (imported > 0)
            {
                _logger.LogInformation(
                    "Synchronisation des demandes de stock : {Count} nouvelles demandes importées depuis la base Vendeur.", imported);
            }
        }

        /// <summary>
        /// Lit les demandes de stock depuis la base locale SQLite du Vendeur et les insère
        /// dans la base Admin si elles n'existent pas encore.
        /// </summary>
        private async Task<int> ImportStockRequestsFromVendorAsync(ApplicationDbContext db)
        {
            if (!File.Exists(_vendeurCachePath))
            {
                return 0;
            }

            var vendorRequests = ReadStockRequestsFromCache(_vendeurCachePath);
            if (vendorRequests.Count == 0) return 0;

            // Récupérer les IDs déjà présents pour éviter les requêtes inutiles.
            var existingIds = await db.StockRequests.Select(r => r.Id).ToHashSetAsync();

            int count = 0;
            foreach (var vr in vendorRequests)
            {
                if (existingIds.Contains(vr.Id)) continue;

                db.StockRequests.Add(new StockRequest
                {
                    Id = vr.Id,
                    BicycleName = vr.BicycleName,
                    Quantity = vr.Quantity,
                    RequestDate = vr.RequestDate,
                    Status = vr.Status,
                    RequestedById = vr.RequestedById,
                    Notes = vr.Notes
                });
                count++;
            }

            if (count > 0)
            {
                await db.SaveChangesAsync();
            }

            return count;
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
