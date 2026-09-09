using Maui_FilRouge.Models;

namespace Maui_FilRouge.Services
{
    /// <summary>
    /// Client de l'API REST du portail admin (AspNet_FilRouge), consommée par l'application MAUI.
    /// Toutes les méthodes nécessitent une connexion réseau : c'est à l'appelant de vérifier
    /// <see cref="IConnectivityService.IsOnline"/> avant d'appeler ce client si le mode hors-ligne
    /// doit être géré (ex. : validation de commande interdite hors connexion).
    /// </summary>
    public interface IApiClient
    {
        Task<LoginResponse> LoginAsync(string userName, string password);

        Task<PagedResult<OrderSummary>> GetOrdersAsync(int page, int pageSize);
        Task<OrderDetail> GetOrderAsync(long id);
        Task<OrderDetail> CreateOrderAsync(CreateOrderRequest request);
        Task<float> AddBicycleAsync(long orderId, long bicycleId);
        Task<float> RemoveBicycleAsync(long orderId, long bicycleId);
        Task<string> ValidateOrderAsync(long orderId);

        Task<List<BicycleSummary>> GetAvailableBicyclesAsync();
        Task<List<SellerSummary>> GetSellersAsync();
        Task<List<CustomerSummary>> GetCustomersAsync();
    }
}
