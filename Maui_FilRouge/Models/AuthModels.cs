namespace Maui_FilRouge.Models
{
    /// <summary>Requête de connexion envoyée à POST api/auth/login.</summary>
    public class LoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>Réponse de connexion : jeton JWT et identité du vendeur connecté.</summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string? SellerId { get; set; }
    }
}
