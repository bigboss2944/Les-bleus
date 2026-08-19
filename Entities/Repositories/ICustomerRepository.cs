namespace Entities
{
    /// <summary>
    /// Accès aux données des clients.
    /// </summary>
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task<bool> ExistsAsync(string id);
        void Add(Customer customer);
        void ApplyUpdate(Customer existing, Customer updated);
        void Remove(Customer customer);
        Task<int> SaveChangesAsync();
    }
}
