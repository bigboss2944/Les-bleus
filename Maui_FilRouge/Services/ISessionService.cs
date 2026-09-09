using Maui_FilRouge.Models;

namespace Maui_FilRouge.Services
{
    /// <summary>
    /// Gère la session du vendeur connecté : jeton JWT (stockage sécurisé) et
    /// informations d'identité (préférences). Source unique de vérité pour savoir
    /// si l'utilisateur est authentifié au démarrage de l'application.
    /// </summary>
    public interface ISessionService
    {
        bool IsAuthenticated { get; }
        string? Token { get; }
        string? UserName { get; }
        string? SellerId { get; }
        IReadOnlyList<string> Roles { get; }

        Task InitializeAsync();
        Task SaveSessionAsync(LoginResponse response);
        Task ClearAsync();
    }
}
