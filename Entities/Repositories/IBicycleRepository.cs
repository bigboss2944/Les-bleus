namespace Entities
{
    /// <summary>
    /// Accès aux données des vélos du catalogue.
    /// </summary>
    public interface IBicycleRepository
    {
        Task<PaginatedList<Bicycle>> GetPagedAsync(int page, int pageSize);
        Task<List<StockSummaryViewModel>> GetStockSummariesAsync();
        Task<Bicycle?> GetByIdAsync(long id);
        Task<Bicycle?> GetByIdWithOrderAsync(long id);
        Task<Bicycle?> GetByIdInOrderAsync(long bicycleId, long orderId);
        Task<List<Bicycle>> GetAvailableAsync();
        Task<List<Bicycle>> GetAvailableForOrderAsync(long orderId);
        Task<List<Bicycle>> GetByIdsAsync(IEnumerable<long> ids);
        Task<bool> ExistsAsync(long id);
        void Add(Bicycle bicycle);
        void Update(Bicycle bicycle);
        void Remove(Bicycle bicycle);
        Task<int> SaveChangesAsync();
    }
}
