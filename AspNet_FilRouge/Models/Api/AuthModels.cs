using System.ComponentModel.DataAnnotations;

namespace AspNet_FilRouge.Models.Api
{
    /// <summary>
    /// Requête de connexion envoyée par l'application MAUI.
    /// </summary>
    public class MobileLoginRequest
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Réponse de connexion : jeton JWT et informations minimales sur le vendeur connecté.
    /// </summary>
    public class MobileLoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string[] Roles { get; set; } = Array.Empty<string>();

        /// <summary>Identifiant du profil Seller correspondant (null si l'utilisateur n'est pas vendeur).</summary>
        public string? SellerId { get; set; }
    }
}
