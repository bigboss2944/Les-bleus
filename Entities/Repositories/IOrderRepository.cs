namespace Entities
{
    /// <summary>
    /// Accès aux données des commandes.
    /// </summary>
    public interface IOrderRepository
    {
        Task<PaginatedList<Order>> GetPagedAsync(int page, int pageSize, string? sellerId);
        Task<Order?> GetByIdAsync(long id);
        Task<Order?> GetDetailsByIdAsync(long id);
        Task<Order?> GetWithBicyclesAndSellerAsync(long id);
        void Add(Order order);
        void Remove(Order order);
        Task<int> SaveChangesAsync();
    }
}
