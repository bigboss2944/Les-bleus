using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Entities
{
    /// <summary>
    /// Opérations métier liées à l'authentification et à la gestion des comptes utilisateurs.
    /// </summary>
    public interface IAccountService
    {
        Task<ApplicationUser?> FindUserForLoginAsync(string emailOrUserName);
        void SimulateTimingSafeVerification(string password);
        Task<SignInResult> PasswordSignInAsync(ApplicationUser user, string password, bool rememberMe);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user);
        Task<ApplicationUser?> FindByIdAsync(string id);
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string code);
        Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string code, string password);
        AuthenticationProperties ConfigureExternalLogin(string provider, string? redirectUrl);
        Task<ExternalLoginInfo?> GetExternalLoginInfoAsync();
        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);
        Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo info);
        Task SignInAsync(ApplicationUser user, bool isPersistent);
        Task SignOutAsync();
        Task<ApplicationUser?> GetTwoFactorAuthenticationUserAsync();
        Task<IList<string>> GetValidTwoFactorProvidersAsync(ApplicationUser user);
        Task GenerateTwoFactorTokenAsync(ApplicationUser user, string provider);
        Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient);
    }
}
