using Maui_FilRouge.Models;
using Maui_FilRouge.Services;

namespace Maui_FilRouge.Views
{
    [QueryProperty(nameof(OrderId), "orderId")]
    public partial class CatalogPickerPage : ContentPage
    {
        private readonly IApiClient _apiClient;

        public string OrderId { get; set; } = string.Empty;

        public CatalogPickerPage(IApiClient apiClient)
        {
            InitializeComponent();
            _apiClient = apiClient;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            try
            {
                CatalogCollectionView.ItemsSource = await _apiClient.GetAvailableBicyclesAsync();
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

        private async void OnBicycleSelected(object? sender, SelectionChangedEventArgs e)
        {
            CatalogCollectionView.SelectedItem = null;
            if (e.CurrentSelection.FirstOrDefault() is not BicycleSummary bicycle) return;

            try
            {
                await _apiClient.AddBicycleAsync(long.Parse(OrderId), bicycle.Id);
                await Shell.Current.GoToAsync("..");
            }
            catch (ApiException ex)
            {
                await DisplayAlertAsync("Erreur", ex.Message, "OK");
            }
        }
    }
}
