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
    }
}
