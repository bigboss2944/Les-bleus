using Microsoft.Data.Sqlite;

namespace AspNet_FilRouge.Services
{
    /// <summary>
    /// Contrat pour écrire les mises à jour de statut des demandes de stock
    /// dans la base locale SQLite du Vendeur.
    /// </summary>
    public interface IVendorSyncService
    {
        Task WriteStatusToVendorCacheAsync(int requestId, string status);
    }

    /// <summary>
    /// Service chargé d'écrire les mises à jour de statut des demandes de stock
    /// dans la base locale SQLite du Vendeur, afin que l'application Vendeur
    /// puisse récupérer les décisions prises par l'administrateur.
    /// </summary>
    public class VendorSyncService : IVendorSyncService
    {
        private readonly string _vendeurCachePath;
        private readonly ILogger<VendorSyncService> _logger;

        public VendorSyncService(
            IWebHostEnvironment env,
            IConfiguration configuration,
            ILogger<VendorSyncService> logger)
        {
            _logger = logger;
            var relPath = configuration["Sync:VendeurCachePath"]
                ?? "../AspNet_FilRouge_Vendeur/local_cache.db";
            _vendeurCachePath = Path.GetFullPath(Path.Combine(env.ContentRootPath, relPath));
        }

        /// <summary>
        /// Met à jour le statut d'une demande de stock dans le cache SQLite du Vendeur.
        /// Appelé immédiatement après qu'un administrateur approuve ou rejette une demande.
        /// </summary>
        public async Task WriteStatusToVendorCacheAsync(int requestId, string status)
        {
            if (!File.Exists(_vendeurCachePath))
            {
                _logger.LogWarning(
                    "Cache Vendeur introuvable : {Path}. Impossible de synchroniser le statut de la demande Id={Id}.",
                    _vendeurCachePath, requestId);
                return;
            }

            await Task.Run(() =>
            {
                using var connection = new SqliteConnection($"Filename={_vendeurCachePath}");
                connection.Open();

                using var checkCmd = new SqliteCommand(
                    "SELECT name FROM sqlite_master WHERE type='table' AND name='StockRequests';",
                    connection);
                if (checkCmd.ExecuteScalar() == null) return;

                using var cmd = new SqliteCommand(
                    "UPDATE StockRequests SET Status=@Status, SyncedAt=@SyncedAt WHERE Id=@Id",
                    connection);
                cmd.Parameters.AddWithValue("@Id", requestId);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@SyncedAt", DateTime.UtcNow.ToString("o"));
                cmd.ExecuteNonQuery();
            });

            _logger.LogInformation(
                "Statut de la demande Id={RequestId} mis à jour en '{Status}' dans le cache Vendeur.",
                requestId, status);
        }
    }
}
