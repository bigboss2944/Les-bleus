using Maui_FilRouge.Models;
using Maui_FilRouge.Services;

namespace Maui_FilRouge.Views
{
    public partial class OrdersListPage : ContentPage
    {
        private readonly IApiClient _apiClient;
        private readonly ILocalCacheService _localCache;
        private readonly IConnectivityService _connectivity;
        private readonly ISyncService _syncService;
        private readonly ISessionService _session;

        public OrdersListPage(
            IApiClient apiClient,
            ILocalCacheService localCache,
            IConnectivityService connectivity,
            ISyncService syncService,
            ISessionService session)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _localCache = localCache;
            _connectivity = connectivity;
            _syncService = syncService;
            _session = session;

            _connectivity.ConnectivityChanged += OnConnectivityChanged;
            _syncService.SyncStateChanged += OnSyncStateChanged;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            UpdateOfflineBanner();
            UpdateSyncStatus();
            await LoadOrdersAsync();
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                if (_connectivity.IsOnline)
                {
                    var page = await _apiClient.GetOrdersAsync(page: 1, pageSize: 50);
                    OrdersCollectionView.ItemsSource = page.Items;
                    await _localCache.InitializeAsync();
                    await _localCache.ReplaceOrdersAsync(page.Items);
                    return;
                }
            }
            catch (Exception)
            {
                // Le serveur peut devenir injoignable même si Connectivity le juge "en ligne"
                // (ex. Wi-Fi sans accès Internet) : on retombe sur le cache local ci-dessous.
            }

            await _localCache.InitializeAsync();
            OrdersCollectionView.ItemsSource = await _localCache.GetOrdersAsync();
        }

        private void UpdateOfflineBanner() => OfflineBanner.IsVisible = !_connectivity.IsOnline;

        private void UpdateSyncStatus()
        {
            SyncIndicator.IsVisible = _syncService.IsSyncing;
            SyncIndicator.IsRunning = _syncService.IsSyncing;
            SyncStatusLabel.Text = _syncService.LastSyncedAtUtc is DateTime last
                ? $"Dernière synchronisation : {last.ToLocalTime():dd/MM HH:mm}"
                : "Pas encore synchronisé";
        }

        private void OnConnectivityChanged(object? sender, bool isOnline) =>
            MainThread.BeginInvokeOnMainThread(UpdateOfflineBanner);

        private void OnSyncStateChanged(object? sender, EventArgs e) =>
            MainThread.BeginInvokeOnMainThread(UpdateSyncStatus);

        private async void OnRefreshing(object? sender, EventArgs e)
        {
            await LoadOrdersAsync();
            if (_connectivity.IsOnline)
            {
                await _syncService.SyncNowAsync();
            }
            OrdersRefreshView.IsRefreshing = false;
        }

        private async void OnOrderSelected(object? sender, SelectionChangedEventArgs e)
        {
            OrdersCollectionView.SelectedItem = null;
            if (e.CurrentSelection.FirstOrDefault() is OrderSummary order)
            {
                await Shell.Current.GoToAsync($"{nameof(OrderDetailPage)}?id={order.IdOrder}");
            }
        }

        private async void OnNewOrderClicked(object? sender, EventArgs e)
        {
            if (!_connectivity.IsOnline)
            {
                await DisplayAlertAsync("Hors-ligne", "La création d'une commande nécessite une connexion.", "OK");
                return;
            }

            try
            {
                var created = await _apiClient.CreateOrderAsync(new CreateOrderRequest
                {
                    SellerId = _session.SellerId,
                    Discount = 0
                });
                await Shell.Current.GoToAsync($"{nameof(OrderDetailPage)}?id={created.IdOrder}");
            }
            catch (ApiException ex)
            {
                await DisplayAlertAsync("Erreur", ex.Message, "OK");
            }
        }

        private async void OnLogoutClicked(object? sender, EventArgs e)
        {
            await _session.ClearAsync();
            await Shell.Current.GoToAsync("//login");
        }
    }
}
