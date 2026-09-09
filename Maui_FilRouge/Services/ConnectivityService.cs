namespace Maui_FilRouge.Services
{
    /// <inheritdoc cref="IConnectivityService"/>
    public class ConnectivityService : IConnectivityService, IDisposable
    {
        public event EventHandler<bool>? ConnectivityChanged;

        public ConnectivityService()
        {
            Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
        }

        public bool IsOnline => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            ConnectivityChanged?.Invoke(this, e.NetworkAccess == NetworkAccess.Internet);
        }

        public void Dispose()
        {
            Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
        }
    }
}
