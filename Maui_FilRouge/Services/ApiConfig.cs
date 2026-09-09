namespace Maui_FilRouge.Services
{
    /// <summary>
    /// Adresse du serveur ASP.NET (portail admin) exposant l'API REST consommée par l'application.
    /// Configurable depuis l'écran de connexion et persistée localement (l'app peut être pointée
    /// vers un serveur de développement ou vers le déploiement de production).
    /// </summary>
    public static class ApiConfig
    {
        private const string PreferenceKey = "ApiBaseUrl";

        /// <summary>Valeur par défaut proposée à la première connexion (émulateur Android → hôte de développement).</summary>
        public const string DefaultBaseUrl = "http://10.0.2.2:5000/";

        public static string BaseUrl
        {
            get => Preferences.Default.Get(PreferenceKey, DefaultBaseUrl);
            set => Preferences.Default.Set(PreferenceKey, NormalizeBaseUrl(value));
        }

        private static string NormalizeBaseUrl(string url)
        {
            url = url.Trim();
            return url.EndsWith('/') ? url : url + "/";
        }
    }

    /// <summary>Exception levée lorsque l'API REST renvoie une erreur applicative (4xx/5xx avec message).</summary>
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
