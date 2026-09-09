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
        Task<List<Order>> GetOrdersBySellerIdAsync(string sellerId);

        /// <summary>
        /// Même résultat que <see cref="GetOrdersBySellerIdAsync"/> mais écrit avec la syntaxe
        /// LINQ `join` explicite (sur la FK) plutôt qu'avec la navigation, à des fins pédagogiques.
        /// </summary>
        Task<List<Order>> GetOrdersBySellerIdExplicitJoinAsync(string sellerId);
    }
}
