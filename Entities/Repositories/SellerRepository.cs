using Microsoft.EntityFrameworkCore;

namespace Entities
{
    /// <inheritdoc cref="ISellerRepository"/>
    public class SellerRepository : ISellerRepository
    {
        private readonly ApplicationDbContext _db;

        public SellerRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Seller>> GetAllAsync() => await _db.Sellers.ToListAsync();

        public async Task<List<Seller>> GetFilteredAsync(bool includeAll, string? sellerId) =>
            await _db.Sellers
                .Where(s => includeAll || s.Id == sellerId)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

        public async Task<Seller?> GetByIdAsync(string id) => await _db.Sellers.FindAsync(id);

        public async Task<Seller?> GetByIdWithShopAsync(string id) =>
            await _db.Sellers.Include(s => s.Shop).FirstOrDefaultAsync(s => s.Id == id);

        public async Task<bool> ExistsAsync(string id) => await _db.Sellers.AnyAsync(s => s.Id == id);

        public async Task<List<Order>> GetOrdersBySellerIdAsync(string sellerId) =>
            await _db.Orders.Where(o => o.Seller.Id == sellerId).ToListAsync();

        public Task<List<Order>> GetOrdersBySellerIdExplicitJoinAsync(string sellerId)
        {
            // TODO (exercice jointure) : réécrire GetOrdersBySellerIdAsync avec la syntaxe
            // `from ... join ... on ... equals ... select ...` plutôt que via la navigation o.Seller.
            var query = from order in _db.Orders
                        join seller in _db.Sellers on order.Seller.Id equals seller.Id
                        where seller.Id == sellerId
                        select order;
            return query.ToListAsync();
        }

        public void Add(Seller seller) => _db.Sellers.Add(seller);

        public void Remove(Seller seller) => _db.Sellers.Remove(seller);

        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
