namespace AspNet_FilRouge_Vendeur.Services
{
    /// <summary>
    /// Contrat pour la gestion de la base de données locale SQLite utilisée
    /// par l'application Vendeur en mode hors-ligne.
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

        /// <summary>Retourne le dictionnaire {id → statut} de toutes les demandes de stock locales.</summary>
        Dictionary<int, string> GetStockRequestStatuses();

        /// <summary>Insère ou met à jour en masse les demandes de stock dans la base locale.</summary>
        Task BulkUpsertStockRequestsAsync(IEnumerable<StockRequest> requests);

        /// <summary>Supprime une demande de stock de la base locale.</summary>
        Task DeleteStockRequestAsync(int requestId);
    }
}
