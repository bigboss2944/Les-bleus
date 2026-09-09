using Microsoft.EntityFrameworkCore;

namespace Entities
{
    /// <inheritdoc cref="ICustomerRepository"/>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _db;

        public CustomerRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Customer>> GetAllAsync() => await _db.Customers.ToListAsync();

        public async Task<Customer?> GetByIdAsync(string id) => await _db.Customers.FindAsync(id);

        public async Task<bool> ExistsAsync(string id) => await _db.Customers.AnyAsync(c => c.Id == id);

        public void Add(Customer customer) => _db.Customers.Add(customer);

        public void ApplyUpdate(Customer existing, Customer updated)
        {
            existing.Town = updated.Town;
            existing.PostalCode = updated.PostalCode;
            existing.Address = updated.Address;
            existing.LoyaltyPoints = updated.LoyaltyPoints;
            existing.Phone = updated.Phone;
            existing.Email = updated.Email;
            existing.Gender = updated.Gender;
            existing.LastName = updated.LastName;
            existing.FirstName = updated.FirstName;
        }

        public void Remove(Customer customer) => _db.Customers.Remove(customer);

        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

        public async Task<Dictionary<Shop, int>> CountByShopAsync(long shopId)
        {
            var query = _db.Customers
                .Where(c => c.Shop.ShopId == shopId)
                .GroupBy(c => c.Shop)
                .Select(g => new { Shop = g.Key, Count = g.Count() });

            var result = await query.ToListAsync();

            return result.ToDictionary(x => x.Shop, x => x.Count);
        }

        public Task<List<CustomerShopOrderCountViewModel>> GetCustomersWithShopAndOrderCountAsync()
        {
            // TODO (exercice jointure) : joindre Customer -> Shop -> Order
            // et projeter CustomerId, CustomerLastName, ShopName, OrderCount.
            var query = _db.Customers.Join(_db.Shops,
                    customer => customer.Shop.ShopId,
                    shop => shop.ShopId,
                    (customer, shop) => new { Customer = customer, Shop = shop })
                .GroupJoin(_db.Orders,
                    customerShop => customerShop.Customer.Id,
                    order => order.Customer.Id,
                    (customerShop, orders) => new CustomerShopOrderCountViewModel
                    {
                        CustomerId = customerShop.Customer.Id,
                        CustomerLastName = customerShop.Customer.LastName,
                        ShopName = customerShop.Shop.Name,
                        OrderCount = orders.Count()
                    });
            return query.ToListAsync();
        }
    }
}
