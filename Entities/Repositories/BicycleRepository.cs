using Microsoft.EntityFrameworkCore;

namespace Entities
{
    /// <inheritdoc cref="IBicycleRepository"/>
    public class BicycleRepository : IBicycleRepository
    {
        private readonly ApplicationDbContext _db;

        public BicycleRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<Bicycle>> GetPagedAsync(int page, int pageSize)
        {
            var bicycles = _db.Bicycles.OrderBy(b => b.Id).AsQueryable();
            return await PaginatedList<Bicycle>.CreateAsync(bicycles, page, pageSize);
        }

        public async Task<List<StockSummaryViewModel>> GetStockSummariesAsync()
        {
            return await _db.Bicycles
                .GroupBy(b => new { b.TypeOfBike, b.Reference, b.Color })
                .Select(group => new StockSummaryViewModel
                {
                    TypeOfBike = group.Key.TypeOfBike,
                    Reference = group.Key.Reference,
                    Color = group.Key.Color,
                    Quantity = group.Sum(b => b.Quantity)
                })
                .OrderBy(summary => summary.TypeOfBike)
                .ThenBy(summary => summary.Reference)
                .ThenBy(summary => summary.Color)
                .ToListAsync();
        }

        public async Task<Bicycle?> GetByIdAsync(long id) => await _db.Bicycles.FindAsync(id);

        public async Task<Bicycle?> GetByIdWithOrderAsync(long id) =>
            await _db.Bicycles.Include(b => b.Order).FirstOrDefaultAsync(b => b.Id == id);

        public async Task<Bicycle?> GetByIdInOrderAsync(long bicycleId, long orderId) =>
            await _db.Bicycles.FirstOrDefaultAsync(b => b.Id == bicycleId && b.Order != null && b.Order.IdOrder == orderId);

        public async Task<List<Bicycle>> GetAvailableAsync() =>
            await _db.Bicycles.Where(b => b.Quantity > 0).ToListAsync();

        public async Task<List<Bicycle>> GetAvailableForOrderAsync(long orderId) =>
            await _db.Bicycles.Where(b => b.Order == null || b.Order.IdOrder == orderId).ToListAsync();

        public async Task<List<Bicycle>> GetByIdsAsync(IEnumerable<long> ids) =>
            await _db.Bicycles.Where(b => ids.Contains(b.Id)).ToListAsync();

        public async Task<bool> ExistsAsync(long id) => await _db.Bicycles.AnyAsync(b => b.Id == id);

        public void Add(Bicycle bicycle) => _db.Bicycles.Add(bicycle);

        public void Update(Bicycle bicycle) => _db.Bicycles.Update(bicycle);

        public void Remove(Bicycle bicycle) => _db.Bicycles.Remove(bicycle);

        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

        public async Task<Bicycle> GetByReferenceAsync(string reference) =>
            await _db.Bicycles.FirstOrDefaultAsync(b => b.Reference == reference) 
            ?? throw new KeyNotFoundException($"Bicycle with reference '{reference}' not found.");

        public async Task<IEnumerable<Bicycle>> GetTopSellingAsync(int n) =>
            await _db.Bicycles.Where(b=>b.Order != null)
                .GroupBy(b=>b.Reference)
                .Select(g => new { Reference = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(n)
                .Join(_db.Bicycles, g => g.Reference, b => b.Reference, (g, b) => b)
                .ToListAsync();

        public Task<List<BicycleOrderDateViewModel>> GetSoldBicyclesWithOrderDateAsync()
        {
            // TODO (exercice jointure) : joindre Bicycle -> Order (b.Order != null)
            // et projeter BicycleId, Reference, OrderDate.
            var query = _db.Bicycles.Where(b=>b.Order != null).Join(_db.Orders,
                bicycle => bicycle.Order.IdOrder,
                order => order.IdOrder,
                (bicycle, order) => new BicycleOrderDateViewModel
                {
                    BicycleId = bicycle.Id,
                    Reference = bicycle.Reference,
                    OrderDate = order.Date
                });
            return query.ToListAsync();
                    
        }
    }
}
