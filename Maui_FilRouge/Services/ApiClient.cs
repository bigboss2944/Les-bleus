using System.Net.Http.Json;
using System.Text.Json;
using Maui_FilRouge.Models;

namespace Maui_FilRouge.Services
{
    /// <inheritdoc cref="IApiClient"/>
    public class ApiClient : IApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ISessionService _session;

        public ApiClient(HttpClient httpClient, ISessionService session)
        {
            _httpClient = httpClient;
            _session = session;
        }

        public async Task<LoginResponse> LoginAsync(string userName, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(
                BuildUri("api/auth/login"),
                new LoginRequest { UserName = userName, Password = password });

            return await ReadOrThrowAsync<LoginResponse>(response);
        }

        public async Task<PagedResult<OrderSummary>> GetOrdersAsync(int page, int pageSize)
        {
            var response = await SendAsync(HttpMethod.Get, $"api/mobile/orders?page={page}&pageSize={pageSize}");
            return await ReadOrThrowAsync<PagedResult<OrderSummary>>(response);
        }

        public async Task<OrderDetail> GetOrderAsync(long id)
        {
            var response = await SendAsync(HttpMethod.Get, $"api/mobile/orders/{id}");
            return await ReadOrThrowAsync<OrderDetail>(response);
        }

        public async Task<OrderDetail> CreateOrderAsync(CreateOrderRequest request)
        {
            var response = await SendAsync(HttpMethod.Post, "api/mobile/orders", request);
            return await ReadOrThrowAsync<OrderDetail>(response);
        }

        public async Task<float> AddBicycleAsync(long orderId, long bicycleId)
        {
            var response = await SendAsync(HttpMethod.Post, $"api/mobile/orders/{orderId}/bicycles/{bicycleId}");
            var result = await ReadOrThrowAsync<TotalResult>(response);
            return result.Total;
        }

        public async Task<float> RemoveBicycleAsync(long orderId, long bicycleId)
        {
            var response = await SendAsync(HttpMethod.Delete, $"api/mobile/orders/{orderId}/bicycles/{bicycleId}");
            var result = await ReadOrThrowAsync<TotalResult>(response);
            return result.Total;
        }

        public async Task<string> ValidateOrderAsync(long orderId)
        {
            var response = await SendAsync(HttpMethod.Post, $"api/mobile/orders/{orderId}/validate");
            var result = await ReadOrThrowAsync<TotalResult>(response);
            return result.Message ?? "Commande validée avec succès.";
        }

        public async Task<List<BicycleSummary>> GetAvailableBicyclesAsync()
        {
            var response = await SendAsync(HttpMethod.Get, "api/mobile/catalog/bicycles");
            return await ReadOrThrowAsync<List<BicycleSummary>>(response);
        }

        public async Task<List<SellerSummary>> GetSellersAsync()
        {
            var response = await SendAsync(HttpMethod.Get, "api/mobile/catalog/sellers");
            return await ReadOrThrowAsync<List<SellerSummary>>(response);
        }

        public async Task<List<CustomerSummary>> GetCustomersAsync()
        {
            var response = await SendAsync(HttpMethod.Get, "api/mobile/catalog/customers");
            return await ReadOrThrowAsync<List<CustomerSummary>>(response);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private Uri BuildUri(string relativePath) => new(new Uri(ApiConfig.BaseUrl), relativePath);

        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string relativePath, object? body = null)
        {
            using var request = new HttpRequestMessage(method, BuildUri(relativePath));
            if (!string.IsNullOrEmpty(_session.Token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
            }
            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            return await _httpClient.SendAsync(request);
        }

        private static async Task<T> ReadOrThrowAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string message = $"Erreur serveur ({(int)response.StatusCode}).";
                try
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorResult>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(error?.Error))
                        message = error!.Error!;
                }
                catch (JsonException)
                {
                    // Réponse non-JSON (ex. erreur réseau/proxy) : on garde le message générique.
                }

                throw new ApiException((int)response.StatusCode, message);
            }

            var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            return result ?? throw new ApiException((int)response.StatusCode, "Réponse vide du serveur.");
        }

        private class ErrorResult
        {
            public string? Error { get; set; }
        }

        private class TotalResult
        {
            public string? Message { get; set; }
            public float Total { get; set; }
        }
    }
}
