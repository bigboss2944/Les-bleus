namespace AspNet_FilRouge.Services
{
    /// <summary>
    /// Résout la clé de signature JWT à partir de la configuration.
    /// Une valeur de repli n'est utilisée qu'en développement/tests ; elle est
    /// interdite ailleurs pour éviter qu'un secret par défaut ne parte en production.
    /// </summary>
    public static class JwtOptionsProvider
    {
        public static string GetSigningKey(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var configuredKey = configuration["Jwt:Key"];
            if (!string.IsNullOrWhiteSpace(configuredKey))
            {
                return configuredKey;
            }

            if (!environment.IsDevelopment() && !environment.IsEnvironment("Testing"))
            {
                throw new InvalidOperationException(
                    "Jwt:Key doit être défini hors développement. Définissez la variable d'environnement Jwt__Key.");
            }

            return "dev-only-insecure-signing-key-do-not-use-in-production-1234567890";
        }
    }
}
