using Microsoft.AspNetCore.Identity;

namespace Entities
{
    /// <summary>
    /// Accès aux données d'identité des comptes utilisateurs (ASP.NET Identity).
    /// </summary>
    public interface IAccountRepository
    {
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindByNameAsync(string userName);
        Task<ApplicationUser?> FindByIdAsync(string id);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> CreateAsync(ApplicationUser user);
        Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string code);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string code, string password);
        Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo info);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(ApplicationUser user);
        Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string provider);
    }
}
