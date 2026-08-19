namespace Entities
{
    /// <summary>
    /// Opérations métier sur les commandes (composition, validation, tarification).
    /// </summary>
    public interface IOrderService
    {
        Task<PaginatedList<Order>> GetPagedAsync(int page, int pageSize, string? sellerId);
        Task<Order?> GetByIdAsync(long id);
        Task<Order?> GetDetailsByIdAsync(long id);
        Task<Order?> GetWithBicyclesAndSellerAsync(long id);
        Task CreateAsync(Order order);
        Task SaveChangesAsync();
        Task<bool> DeleteAsync(long id);
        float CalculateTotal(Order order);
        Task<OrderOperationResult> AddBicycleAsync(long orderId, long bicycleId);
        Task<OrderOperationResult> RemoveBicycleAsync(long orderId, long bicycleId);
        Task<OrderOperationResult> ValidateAsync(long id);
    }
}
