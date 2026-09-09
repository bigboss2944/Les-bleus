using Maui_FilRouge.Models;

namespace Maui_FilRouge.Services
{
    /// <summary>
    /// Cache local en lecture (SQLite, accès direct sans Entity Framework — imposé par le
    /// cahier des charges) permettant de consulter commandes, catalogue, vendeurs et clients
    /// hors connexion. Alimenté périodiquement par <see cref="ISyncService"/>.
    /// </summary>
    public interface ILocalCacheService
    {
        Task InitializeAsync();

        Task ReplaceOrdersAsync(IEnumerable<OrderSummary> orders);
        Task<List<OrderSummary>> GetOrdersAsync();

        Task ReplaceBicyclesAsync(IEnumerable<BicycleSummary> bicycles);
        Task<List<BicycleSummary>> GetBicyclesAsync();

        Task ReplaceSellersAsync(IEnumerable<SellerSummary> sellers);
        Task<List<SellerSummary>> GetSellersAsync();

        Task ReplaceCustomersAsync(IEnumerable<CustomerSummary> customers);
        Task<List<CustomerSummary>> GetCustomersAsync();

        Task<DateTime?> GetLastSyncedAtUtcAsync();
    }
}
