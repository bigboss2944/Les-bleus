using Maui_FilRouge.Models;
using Maui_FilRouge.Services;

namespace Maui_FilRouge.Views
{
    [QueryProperty(nameof(OrderId), "id")]
    public partial class OrderDetailPage : ContentPage
    {
        private readonly IApiClient _apiClient;
        private readonly IConnectivityService _connectivity;
        private OrderDetail? _order;

        public string OrderId { get; set; } = string.Empty;

        public OrderDetailPage(IApiClient apiClient, IConnectivityService connectivity)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _connectivity = connectivity;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            OfflineBanner.IsVisible = !_connectivity.IsOnline;

            if (!_connectivity.IsOnline)
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                BicyclesCollectionView.IsVisible = false;
                EmptyOfflineLabel.IsVisible = true;
                AddBicycleButton.IsEnabled = false;
                ValidateButton.IsEnabled = false;
                return;
            }

            EmptyOfflineLabel.IsVisible = false;
            BicyclesCollectionView.IsVisible = true;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                _order = await _apiClient.GetOrderAsync(long.Parse(OrderId));
                RenderOrder();
            }
            catch (ApiException ex)
            {
                await DisplayAlertAsync("Erreur", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private void RenderOrder()
        {
            if (_order == null) return;

            Title = $"Commande n° {_order.IdOrder}";
            TotalLabel.Text = $"Total : {_order.Total:F2} €";
            StatusLabel.Text = _order.IsValidated ? "Validée" : "En cours";
            StatusLabel.TextColor = (Color)Application.Current!.Resources[_order.IsValidated ? "Success" : "Secondary"];
            BicyclesCollectionView.ItemsSource = _order.Bicycles;

            var canEdit = !_order.IsValidated;
            AddBicycleButton.IsEnabled = canEdit;
            ValidateButton.IsVisible = canEdit;
            ValidateButton.IsEnabled = canEdit && _order.Bicycles.Count > 0;
        }

        private async void OnAddBicycleClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(CatalogPickerPage)}?orderId={OrderId}");
        }

        private async void OnRemoveBicycleClicked(object? sender, EventArgs e)
        {
            if (sender is not Button button || button.CommandParameter is not long bicycleId) return;

            if (!_connectivity.IsOnline)
            {
                await DisplayAlertAsync("Hors-ligne", "Modification impossible hors connexion.", "OK");
                return;
            }

            try
            {
                await _apiClient.RemoveBicycleAsync(long.Parse(OrderId), bicycleId);
                await LoadAsync();
            }
            catch (ApiException ex)
            {
                await DisplayAlertAsync("Erreur", ex.Message, "OK");
            }
        }

        private async void OnValidateClicked(object? sender, EventArgs e)
        {
            if (!_connectivity.IsOnline)
            {
                await DisplayAlertAsync("Hors-ligne", "La validation d'une commande nécessite une connexion.", "OK");
                return;
            }

            try
            {
                var message = await _apiClient.ValidateOrderAsync(long.Parse(OrderId));
                await DisplayAlertAsync("Commande validée", message, "OK");
                await LoadAsync();
            }
            catch (ApiException ex)
            {
                await DisplayAlertAsync("Erreur", ex.Message, "OK");
            }
        }
    }
}
