using Microsoft.AspNetCore.Identity;

namespace Entities
{
    /// <inheritdoc cref="ISellerService"/>
    public class SellerService : ISellerService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public SellerService(ISellerRepository sellerRepository, UserManager<ApplicationUser> userManager)
        {
            _sellerRepository = sellerRepository;
            _userManager = userManager;
        }

        public Task<List<Seller>> GetAllAsync() => _sellerRepository.GetAllAsync();

        public Task<List<Seller>> GetFilteredAsync(bool includeAll, string? sellerId) =>
            _sellerRepository.GetFilteredAsync(includeAll, sellerId);

        public Task<Seller?> GetByIdAsync(string id) => _sellerRepository.GetByIdAsync(id);

        public Task<Seller?> GetByIdWithShopAsync(string id) => _sellerRepository.GetByIdWithShopAsync(id);

        public async Task<IdentityResult> CreateSellerAccountAsync(CreateSellerViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                LastName = model.LastName,
                FirstName = model.FirstName,
                Address = model.Address,
                PhoneNumber = model.Phone,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password!);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, AppConstants.Roles.Vendeur);

                _sellerRepository.Add(new Seller
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                });
                await _sellerRepository.SaveChangesAsync();
            }

            return result;
        }

        public async Task CreateAsync(Seller seller)
        {
            _sellerRepository.Add(seller);
            await _sellerRepository.SaveChangesAsync();
        }

        public Task SaveChangesAsync() => _sellerRepository.SaveChangesAsync();

        public async Task<bool> DeleteAsync(string id)
        {
            var seller = await _sellerRepository.GetByIdAsync(id);
            if (seller == null) return false;

            _sellerRepository.Remove(seller);
            await _sellerRepository.SaveChangesAsync();
            return true;
        }
    }
}
