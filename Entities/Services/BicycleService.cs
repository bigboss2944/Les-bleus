namespace Entities
{
    /// <inheritdoc cref="IBicycleService"/>
    public class BicycleService : IBicycleService
    {
        private readonly IBicycleRepository _bicycleRepository;

        public BicycleService(IBicycleRepository bicycleRepository)
        {
            _bicycleRepository = bicycleRepository;
        }

        public Task<PaginatedList<Bicycle>> GetPagedAsync(int page, int pageSize) =>
            _bicycleRepository.GetPagedAsync(page, pageSize);

        public Task<List<StockSummaryViewModel>> GetStockSummariesAsync() =>
            _bicycleRepository.GetStockSummariesAsync();

        public Task<Bicycle?> GetByIdAsync(long id) => _bicycleRepository.GetByIdAsync(id);

        public Task<bool> ExistsAsync(long id) => _bicycleRepository.ExistsAsync(id);

        public async Task CreateAsync(Bicycle bicycle)
        {
            _bicycleRepository.Add(bicycle);
            await _bicycleRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bicycle bicycle)
        {
            _bicycleRepository.Update(bicycle);
            await _bicycleRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var bicycle = await _bicycleRepository.GetByIdAsync(id);
            if (bicycle == null) return false;

            _bicycleRepository.Remove(bicycle);
            await _bicycleRepository.SaveChangesAsync();
            return true;
        }

        public Task<List<Bicycle>> GetAvailableAsync() => _bicycleRepository.GetAvailableAsync();

        public Task<List<Bicycle>> GetAvailableForOrderAsync(long orderId) =>
            _bicycleRepository.GetAvailableForOrderAsync(orderId);

        public Task<List<Bicycle>> GetByIdsAsync(IEnumerable<long> ids) => _bicycleRepository.GetByIdsAsync(ids);
    }
}
