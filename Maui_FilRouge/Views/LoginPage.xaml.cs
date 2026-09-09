using Maui_FilRouge.Services;

namespace Maui_FilRouge.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly IApiClient _apiClient;
        private readonly ISessionService _session;
        private readonly ISyncService _syncService;

        public LoginPage(IApiClient apiClient, ISessionService session, ISyncService syncService)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _session = session;
            _syncService = syncService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            ServerUrlEntry.Text = ApiConfig.BaseUrl;

            // Session déjà valide (retour dans l'app) : on saute directement l'écran de connexion.
            await _session.InitializeAsync();
            if (_session.IsAuthenticated)
            {
                _syncService.Start();
                await Shell.Current.GoToAsync("//orders");
            }
        }

        private async void OnLoginClicked(object? sender, EventArgs e)
        {
            ErrorLabel.IsVisible = false;

            if (string.IsNullOrWhiteSpace(ServerUrlEntry.Text) ||
                string.IsNullOrWhiteSpace(UserNameEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                ShowError("Merci de renseigner l'adresse du serveur, l'identifiant et le mot de passe.");
                return;
            }

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LoginButton.IsEnabled = false;

            try
            {
                ApiConfig.BaseUrl = ServerUrlEntry.Text;

                var response = await _apiClient.LoginAsync(UserNameEntry.Text, PasswordEntry.Text);
                await _session.SaveSessionAsync(response);

                _syncService.Start();
                await Shell.Current.GoToAsync("//orders");
            }
            catch (ApiException ex)
            {
                ShowError(ex.Message);
            }
            catch (HttpRequestException)
            {
                ShowError("Impossible de contacter le serveur. Vérifiez l'adresse et votre connexion réseau.");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                LoginButton.IsEnabled = true;
            }
        }

        private void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true;
        }
    }
}
