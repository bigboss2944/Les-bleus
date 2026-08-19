using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Entities
{
    /// <inheritdoc cref="IAccountService"/>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private static readonly PasswordHasher<ApplicationUser> _timingSafetyHasher = new();
        private static readonly string _dummyPasswordHash = _timingSafetyHasher.HashPassword(new ApplicationUser(), Guid.NewGuid().ToString());

        public AccountService(IAccountRepository accountRepository, SignInManager<ApplicationUser> signInManager)
        {
            _accountRepository = accountRepository;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser?> FindUserForLoginAsync(string emailOrUserName) =>
            await _accountRepository.FindByEmailAsync(emailOrUserName)
            ?? await _accountRepository.FindByNameAsync(emailOrUserName);

        public void SimulateTimingSafeVerification(string password)
        {
            // Hash a dummy password so the response takes roughly as long as a real
            // login attempt, preventing user enumeration via timing.
            _timingSafetyHasher.VerifyHashedPassword(new ApplicationUser(), _dummyPasswordHash, password);
        }

        public Task<SignInResult> PasswordSignInAsync(ApplicationUser user, string password, bool rememberMe) =>
            _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

        public Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password) =>
            _accountRepository.CreateAsync(user, password);

        public Task<IdentityResult> CreateUserAsync(ApplicationUser user) => _accountRepository.CreateAsync(user);

        public Task<ApplicationUser?> FindByIdAsync(string id) => _accountRepository.FindByIdAsync(id);

        public Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string code) =>
            _accountRepository.ConfirmEmailAsync(user, code);

        public Task<bool> IsEmailConfirmedAsync(ApplicationUser user) => _accountRepository.IsEmailConfirmedAsync(user);

        public Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user) =>
            _accountRepository.GeneratePasswordResetTokenAsync(user);

        public Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string code, string password) =>
            _accountRepository.ResetPasswordAsync(user, code, password);

        public AuthenticationProperties ConfigureExternalLogin(string provider, string? redirectUrl) =>
            _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        public Task<ExternalLoginInfo?> GetExternalLoginInfoAsync() => _signInManager.GetExternalLoginInfoAsync();

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent) =>
            _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);

        public Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo info) =>
            _accountRepository.AddLoginAsync(user, info);

        public Task SignInAsync(ApplicationUser user, bool isPersistent) =>
            _signInManager.SignInAsync(user, isPersistent);

        public Task SignOutAsync() => _signInManager.SignOutAsync();

        public Task<ApplicationUser?> GetTwoFactorAuthenticationUserAsync() =>
            _signInManager.GetTwoFactorAuthenticationUserAsync();

        public Task<IList<string>> GetValidTwoFactorProvidersAsync(ApplicationUser user) =>
            _accountRepository.GetValidTwoFactorProvidersAsync(user);

        public Task GenerateTwoFactorTokenAsync(ApplicationUser user, string provider) =>
            _accountRepository.GenerateTwoFactorTokenAsync(user, provider);

        public Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient) =>
            _signInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient);
    }
}
