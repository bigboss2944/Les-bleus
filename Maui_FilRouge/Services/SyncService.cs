namespace Maui_FilRouge.Services
{
    /// <inheritdoc cref="ISyncService"/>
    public class SyncService : ISyncService, IDisposable
    {
        private static readonly TimeSpan SyncInterval = TimeSpan.FromMinutes(5);

        private readonly IApiClient _apiClient;
        private readonly ILocalCacheService _localCache;
        private readonly IConnectivityService _connectivity;
        private readonly ISessionService _session;

        private Timer? _timer;
        private bool _started;

        public bool IsSyncing { get; private set; }
        public DateTime? LastSyncedAtUtc { get; private set; }

        public event EventHandler? SyncStateChanged;

        public SyncService(IApiClient apiClient, ILocalCacheService localCache, IConnectivityService connectivity, ISessionService session)
        {
            _apiClient = apiClient;
            _localCache = localCache;
            _connectivity = connectivity;
            _session = session;
            _connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        public void Start()
        {
            if (_started) return;
            _started = true;

            _ = SyncNowAsync();
            _timer = new Timer(_ => _ = SyncNowAsync(), null, SyncInterval, SyncInterval);
        }

        private void OnConnectivityChanged(object? sender, bool isOnline)
        {
            if (isOnline)
            {
                _ = SyncNowAsync();
            }
        }

        public async Task SyncNowAsync()
        {
            if (IsSyncing || !_connectivity.IsOnline || !_session.IsAuthenticated) return;

            IsSyncing = true;
            SyncStateChanged?.Invoke(this, EventArgs.Empty);

            try
            {
                await _localCache.InitializeAsync();

                var bicycles = await _apiClient.GetAvailableBicyclesAsync();
                await _localCache.ReplaceBicyclesAsync(bicycles);

                var sellers = await _apiClient.GetSellersAsync();
                await _localCache.ReplaceSellersAsync(sellers);

                var customers = await _apiClient.GetCustomersAsync();
                await _localCache.ReplaceCustomersAsync(customers);

                var orders = await _apiClient.GetOrdersAsync(page: 1, pageSize: 50);
                await _localCache.ReplaceOrdersAsync(orders.Items);

                LastSyncedAtUtc = await _localCache.GetLastSyncedAtUtcAsync();
            }
            catch (Exception)
            {
                // Le mobile peut se retrouver hors-ligne pendant la synchronisation (perte réseau,
                // serveur inaccessible) : on abandonne silencieusement ce cycle, le prochain
                // déclenchement (timer, retour de connexion, ou action manuelle) réessaiera.
            }
            finally
            {
                IsSyncing = false;
                SyncStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            _connectivity.ConnectivityChanged -= OnConnectivityChanged;
            _timer?.Dispose();
        }
    }
}
