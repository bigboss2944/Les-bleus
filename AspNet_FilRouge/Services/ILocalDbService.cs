namespace AspNet_FilRouge.Services
{
    /// <summary>
    /// Contrat pour la gestion de la base de données locale SQLite utilisée
    /// en mode hors-ligne (synchronisation différée avec le serveur central).
    /// </summary>
    public interface ILocalDbService
    {
        /// <summary>
        /// Insère ou met à jour en masse les commandes, vélos, vendeurs et clients
        /// dans la base locale (opération transactionnelle).
        /// </summary>
        Task BulkUpsertAllAsync(
            IEnumerable<Order> orders,
            IEnumerable<Bicycle> bicycles,
            IEnumerable<Seller> sellers,
            IEnumerable<Customer> customers);

        /// <summary>Retourne toutes les commandes stockées localement.</summary>
        IEnumerable<(long IdOrder, DateTime Date, string? PayMode, float Discount, bool IsValidated)> GetOrders();

        /// <summary>Retourne des statistiques globales sur la base locale.</summary>
        (int Orders, int Bicycles, int Sellers, int Customers, DateTime? LastSyncedAt) GetStats();

        /// <summary>Enregistre le nouveau statut d'une demande de stock dans la base locale.</summary>
        Task SaveStockRequestStatusAsync(int stockRequestId, string newStatus);

        /// <summary>Retourne le dictionnaire {id → statut} de toutes les demandes de stock locales.</summary>
        Dictionary<int, string> GetStockRequestStatuses();

        /// <summary>Insère ou met à jour en masse les demandes de stock dans la base locale.</summary>
        Task BulkUpsertStockRequestsAsync(IEnumerable<StockRequest> requests);

        /// <summary>
        /// Ajoute une mise à jour de statut vendeur en file d'attente locale
        /// pour une tentative d'écriture ultérieure (retry).
        /// </summary>
        Task EnqueuePendingVendorStatusUpdateAsync(int requestId, string status, string? lastError);

        /// <summary>Retourne les mises à jour de statut vendeur en attente (limitées à <paramref name="limit"/> entrées).</summary>
        List<PendingVendorStatusUpdate> GetPendingVendorStatusUpdates(int limit);

        /// <summary>Supprime définitivement une mise à jour en attente après application réussie.</summary>
        Task DeletePendingVendorStatusUpdateAsync(long id);

        /// <summary>Marque une mise à jour en attente comme ayant échoué lors du dernier retry.</summary>
        Task MarkPendingVendorStatusUpdateRetryAsync(long id, string? lastError);
    }
}
