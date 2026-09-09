namespace Maui_FilRouge.Services
{
    /// <summary>
    /// Synchronise périodiquement le cache local depuis l'API REST (lecture uniquement).
    /// Reprend l'intervalle de 5 minutes déjà utilisé côté AspNet_FilRouge_Vendeur, plus une
    /// synchronisation immédiate à chaque retour de connexion et un déclenchement manuel.
    /// </summary>
    public interface ISyncService
    {
        bool IsSyncing { get; }
        DateTime? LastSyncedAtUtc { get; }

        event EventHandler? SyncStateChanged;

        void Start();
        Task SyncNowAsync();
    }
}
