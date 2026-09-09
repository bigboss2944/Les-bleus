namespace Maui_FilRouge.Services
{
    /// <summary>
    /// Détecte l'état de connexion réseau. Utilisé pour afficher le bandeau hors-ligne
    /// et bloquer la validation de commande, conformément au cahier des charges.
    /// </summary>
    public interface IConnectivityService
    {
        bool IsOnline { get; }

        event EventHandler<bool>? ConnectivityChanged;
    }
}
