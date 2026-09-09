using System.Globalization;
using Maui_FilRouge.Models;

namespace Maui_FilRouge.Services
{
    /// <inheritdoc cref="ISessionService"/>
    public class SessionService : ISessionService
    {
        private const string TokenKey = "AuthToken";
        private const string UserNameKey = "AuthUserName";
        private const string SellerIdKey = "AuthSellerId";
        private const string RolesKey = "AuthRoles";
        private const string ExpiresAtKey = "AuthExpiresAtUtc";

        private DateTime _expiresAtUtc = DateTime.MinValue;

        public bool IsAuthenticated => !string.IsNullOrEmpty(Token) && DateTime.UtcNow < _expiresAtUtc;

        public string? Token { get; private set; }
        public string? UserName { get; private set; }
        public string? SellerId { get; private set; }
        public IReadOnlyList<string> Roles { get; private set; } = Array.Empty<string>();

        public async Task InitializeAsync()
        {
            Token = await SecureStorage.Default.GetAsync(TokenKey);
            UserName = Preferences.Default.Get(UserNameKey, (string?)null);
            SellerId = Preferences.Default.Get(SellerIdKey, (string?)null);

            var rolesCsv = Preferences.Default.Get(RolesKey, string.Empty);
            Roles = string.IsNullOrEmpty(rolesCsv)
                ? Array.Empty<string>()
                : rolesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var expiresAtRaw = Preferences.Default.Get(ExpiresAtKey, string.Empty);
            _expiresAtUtc = DateTime.TryParse(expiresAtRaw, null, DateTimeStyles.RoundtripKind, out var parsed)
                ? parsed
                : DateTime.MinValue;
        }

        public async Task SaveSessionAsync(LoginResponse response)
        {
            Token = response.Token;
            UserName = response.UserName;
            SellerId = response.SellerId;
            Roles = response.Roles;
            _expiresAtUtc = response.ExpiresAtUtc;

            await SecureStorage.Default.SetAsync(TokenKey, response.Token);
            Preferences.Default.Set(UserNameKey, response.UserName);
            Preferences.Default.Set(SellerIdKey, response.SellerId);
            Preferences.Default.Set(RolesKey, string.Join(',', response.Roles));
            Preferences.Default.Set(ExpiresAtKey, response.ExpiresAtUtc.ToString("o"));
        }

        public Task ClearAsync()
        {
            Token = null;
            UserName = null;
            SellerId = null;
            Roles = Array.Empty<string>();
            _expiresAtUtc = DateTime.MinValue;

            SecureStorage.Default.Remove(TokenKey);
            Preferences.Default.Remove(UserNameKey);
            Preferences.Default.Remove(SellerIdKey);
            Preferences.Default.Remove(RolesKey);
            Preferences.Default.Remove(ExpiresAtKey);

            return Task.CompletedTask;
        }
    }
}
