namespace Entities
{
    /// <summary>
    /// Opérations métier sur les clients.
    /// </summary>
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task CreateAsync(Customer customer);
        Task UpdateAsync(Customer existing, Customer updated);
        Task<bool> DeleteAsync(string id);
    }
}
