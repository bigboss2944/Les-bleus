namespace Entities
{
    /// <summary>
    /// Opérations métier sur les vélos du catalogue.
    /// </summary>
    public interface IBicycleService
    {
        Task<PaginatedList<Bicycle>> GetPagedAsync(int page, int pageSize);
        Task<List<StockSummaryViewModel>> GetStockSummariesAsync();
        Task<Bicycle?> GetByIdAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task CreateAsync(Bicycle bicycle);
        Task UpdateAsync(Bicycle bicycle);
        Task<bool> DeleteAsync(long id);
        Task<List<Bicycle>> GetAvailableAsync();
        Task<List<Bicycle>> GetAvailableForOrderAsync(long orderId);
        Task<List<Bicycle>> GetByIdsAsync(IEnumerable<long> ids);
    }
}
