using Microsoft.AspNetCore.Identity;

namespace Entities
{
    /// <summary>
    /// Opérations métier sur les vendeurs.
    /// </summary>
    public interface ISellerService
    {
        Task<List<Seller>> GetAllAsync();
        Task<List<Seller>> GetFilteredAsync(bool includeAll, string? sellerId);
        Task<Seller?> GetByIdAsync(string id);
        Task<Seller?> GetByIdWithShopAsync(string id);
        Task<IdentityResult> CreateSellerAccountAsync(CreateSellerViewModel model);
        Task CreateAsync(Seller seller);
        Task SaveChangesAsync();
        Task<bool> DeleteAsync(string id);
    }
}
