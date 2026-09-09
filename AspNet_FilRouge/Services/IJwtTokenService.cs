using Entities;

namespace AspNet_FilRouge.Services
{
    /// <summary>
    /// Génère les jetons JWT utilisés par l'application MAUI pour s'authentifier
    /// auprès de l'API REST du portail admin.
    /// </summary>
    public interface IJwtTokenService
    {
        (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}
