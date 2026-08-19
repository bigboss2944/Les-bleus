using Microsoft.EntityFrameworkCore;

namespace Entities
{
    /// <inheritdoc cref="IOrderRepository"/>
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<Order>> GetPagedAsync(int page, int pageSize, string? sellerId)
        {
            var orders = _db.Orders.Include(o => o.Seller).AsQueryable();

            if (!string.IsNullOrWhiteSpace(sellerId))
                orders = orders.Where(o => o.Seller != null && o.Seller.Id == sellerId);

            orders = orders.OrderByDescending(o => o.Date).ThenByDescending(o => o.IdOrder);

            return await PaginatedList<Order>.CreateAsync(orders, page, pageSize);
        }

        public async Task<Order?> GetByIdAsync(long id) => await _db.Orders.FindAsync(id);

        public async Task<Order?> GetDetailsByIdAsync(long id) =>
            await _db.Orders
                .Include(o => o.Seller)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.Bicycles)
                .FirstOrDefaultAsync(o => o.IdOrder == id);

        public async Task<Order?> GetWithBicyclesAndSellerAsync(long id) =>
            await _db.Orders
                .Include(o => o.Bicycles)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.IdOrder == id);

        public void Add(Order order) => _db.Orders.Add(order);

        public void Remove(Order order) => _db.Orders.Remove(order);

        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
