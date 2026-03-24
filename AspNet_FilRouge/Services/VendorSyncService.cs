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
        Task<int> FlushPendingUpdatesAsync();
    }

    /// <summary>
    /// Service chargé d'écrire les mises à jour de statut des demandes de stock
    /// dans la base locale SQLite du Vendeur, afin que l'application Vendeur
    /// puisse récupérer les décisions prises par l'administrateur.
    /// </summary>
    public class VendorSyncService : IVendorSyncService
    {
        private readonly string _vendeurCachePath;
        private readonly LocalDbService _localDb;
        private readonly ILogger<VendorSyncService> _logger;

        public VendorSyncService(
            IWebHostEnvironment env,
            IConfiguration configuration,
            LocalDbService localDb,
            ILogger<VendorSyncService> logger)
        {
            _localDb = localDb;
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
            var writeResult = await TryWriteStatusToVendorCacheAsync(requestId, status);
            if (writeResult.Success)
            {
                _logger.LogInformation(
                    "Statut de la demande Id={RequestId} mis a jour en '{Status}' dans le cache Vendeur.",
                    requestId, status);
                return;
            }

            await _localDb.EnqueuePendingVendorStatusUpdateAsync(requestId, status, writeResult.Error);

            _logger.LogWarning(
                "Ecriture immediate impossible pour la demande Id={RequestId}. Mise en file locale pour retry. Erreur: {Error}",
                requestId,
                writeResult.Error ?? "inconnue");
        }

        public async Task<int> FlushPendingUpdatesAsync()
        {
            var pending = _localDb.GetPendingVendorStatusUpdates(200);
            if (pending.Count == 0)
            {
                return 0;
            }

            int flushed = 0;
            foreach (var item in pending)
            {
                var writeResult = await TryWriteStatusToVendorCacheAsync(item.RequestId, item.Status);
                if (writeResult.Success)
                {
                    await _localDb.DeletePendingVendorStatusUpdateAsync(item.Id);
                    flushed++;
                }
                else
                {
                    await _localDb.MarkPendingVendorStatusUpdateRetryAsync(item.Id, writeResult.Error);
                    // Le vendeur est probablement encore hors-ligne: on garde le reste en file.
                    break;
                }
            }

            if (flushed > 0)
            {
                _logger.LogInformation("Replay offline: {Count} mise(s) a jour vendeur appliquee(s).", flushed);
            }

            return flushed;
        }

        private async Task<(bool Success, string? Error)> TryWriteStatusToVendorCacheAsync(int requestId, string status)
        {
            if (!File.Exists(_vendeurCachePath))
            {
                return (false, $"Cache vendeur introuvable: {_vendeurCachePath}");
            }

            try
            {
                await Task.Run(() =>
                {
                    using var connection = new SqliteConnection($"Filename={_vendeurCachePath}");
                    connection.Open();

                    using var checkCmd = new SqliteCommand(
                        "SELECT name FROM sqlite_master WHERE type='table' AND name='StockRequests';",
                        connection);
                    if (checkCmd.ExecuteScalar() == null)
                    {
                        throw new InvalidOperationException("Table StockRequests absente dans le cache vendeur.");
                    }

                    using var cmd = new SqliteCommand(
                        "UPDATE StockRequests SET Status=@Status, SyncedAt=@SyncedAt WHERE Id=@Id",
                        connection);
                    cmd.Parameters.AddWithValue("@Id", requestId);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@SyncedAt", DateTime.UtcNow.ToString("o"));
                    cmd.ExecuteNonQuery();
                });

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
