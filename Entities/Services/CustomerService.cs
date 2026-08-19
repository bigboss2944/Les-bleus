namespace Entities
{
    /// <inheritdoc cref="ICustomerService"/>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public Task<List<Customer>> GetAllAsync() => _customerRepository.GetAllAsync();

        public Task<Customer?> GetByIdAsync(string id) => _customerRepository.GetByIdAsync(id);

        public Task<bool> ExistsAsync(string id) => _customerRepository.ExistsAsync(id);

        public async Task CreateAsync(Customer customer)
        {
            _customerRepository.Add(customer);
            await _customerRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer existing, Customer updated)
        {
            _customerRepository.ApplyUpdate(existing, updated);
            await _customerRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) return false;

            _customerRepository.Remove(customer);
            await _customerRepository.SaveChangesAsync();
            return true;
        }
    }
}
