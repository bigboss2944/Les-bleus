namespace Entities
{
    /// <summary>
    /// Accès aux données des vendeurs.
    /// </summary>
    public interface ISellerRepository
    {
        Task<List<Seller>> GetAllAsync();
        Task<List<Seller>> GetFilteredAsync(bool includeAll, string? sellerId);
        Task<Seller?> GetByIdAsync(string id);
        Task<Seller?> GetByIdWithShopAsync(string id);
        Task<bool> ExistsAsync(string id);
        void Add(Seller seller);
        void Remove(Seller seller);
        Task<int> SaveChangesAsync();
    }
}
