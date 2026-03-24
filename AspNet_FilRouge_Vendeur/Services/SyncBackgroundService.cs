using AspNet_FilRouge_Vendeur.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNet_FilRouge_Vendeur.Services
{
    /// <summary>
    /// Service en arrière-plan qui synchronise automatiquement la base centrale
    /// vers la base locale SQLite toutes les 5 minutes.
    /// </summary>
    public class SyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly LocalDbService _localDb;
        private readonly ILogger<SyncBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public SyncBackgroundService(
            IServiceScopeFactory scopeFactory,
            LocalDbService localDb,
            ILogger<SyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _localDb = localDb;
            _logger = logger;
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

            // Appliquer les mises à jour de statut décidées par l'admin
            // (écrites dans le cache SQLite) avant d'écraser le cache avec les données EF Core.
            await ApplyAdminStatusUpdatesAsync(db);

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

            var stockRequests = await db.StockRequests.ToListAsync();

            // Écriture atomique en une seule transaction pour éviter les verrous répétés.
            await _localDb.BulkUpsertAllAsync(orders, bicycles, sellers, customers);
            await _localDb.BulkUpsertStockRequestsAsync(stockRequests);

            _logger.LogInformation(
                "Synchronisation automatique terminée — {Orders} commandes, {Bicycles} vélos, {Sellers} vendeurs, {Customers} clients, {StockRequests} demandes de stock.",
                orders.Count, bicycles.Count, sellers.Count, customers.Count, stockRequests.Count);
        }

        /// <summary>
        /// Lit les statuts des demandes de stock depuis le cache SQLite local et met à jour
        /// la base EF Core si l'admin a approuvé ou rejeté une demande depuis la dernière sync.
        /// </summary>
        private async Task ApplyAdminStatusUpdatesAsync(ApplicationDbContext db)
        {
            var cachedStatuses = _localDb.GetStockRequestStatuses();
            if (cachedStatuses.Count == 0) return;

            var cachedIds = cachedStatuses.Keys.ToList();
            var dbRequests = await db.StockRequests
                .Where(r => cachedIds.Contains(r.Id))
                .ToListAsync();

            bool anyChanged = false;
            foreach (var req in dbRequests)
            {
                if (cachedStatuses.TryGetValue(req.Id, out var cachedStatus)
                    && cachedStatus != req.Status)
                {
                    _logger.LogInformation(
                        "Demande Id={Id} : statut mis à jour de '{Old}' en '{New}' depuis le cache admin.",
                        req.Id, req.Status, cachedStatus);
                    req.Status = cachedStatus;
                    anyChanged = true;
                }
            }

            if (anyChanged)
                await db.SaveChangesAsync();
        }
    }
}
