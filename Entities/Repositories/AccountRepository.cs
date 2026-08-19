using Microsoft.AspNetCore.Identity;

namespace Entities
{
    /// <inheritdoc cref="IAccountRepository"/>
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email) => _userManager.FindByEmailAsync(email);

        public Task<ApplicationUser?> FindByNameAsync(string userName) => _userManager.FindByNameAsync(userName);

        public Task<ApplicationUser?> FindByIdAsync(string id) => _userManager.FindByIdAsync(id);

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password) => _userManager.CreateAsync(user, password);

        public Task<IdentityResult> CreateAsync(ApplicationUser user) => _userManager.CreateAsync(user);

        public Task<bool> IsEmailConfirmedAsync(ApplicationUser user) => _userManager.IsEmailConfirmedAsync(user);

        public Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string code) => _userManager.ConfirmEmailAsync(user, code);

        public Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user) => _userManager.GeneratePasswordResetTokenAsync(user);

        public Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string code, string password) =>
            _userManager.ResetPasswordAsync(user, code, password);

        public Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo info) => _userManager.AddLoginAsync(user, info);

        public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) => _userManager.AddToRoleAsync(user, role);

        public Task<IList<string>> GetValidTwoFactorProvidersAsync(ApplicationUser user) => _userManager.GetValidTwoFactorProvidersAsync(user);

        public Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string provider) =>
            _userManager.GenerateTwoFactorTokenAsync(user, provider);
    }
}
