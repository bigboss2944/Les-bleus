namespace AspNet_FilRouge.Services
{
    /// <summary>
    /// Représente une mise à jour de statut de demande de stock en attente de synchronisation
    /// vers la base SQLite du Vendeur (mode retry hors-ligne).
    /// </summary>
    public sealed record PendingVendorStatusUpdate(
        long Id,
        int RequestId,
        string Status,
        DateTime CreatedAt,
        int RetryCount,
        string? LastError);
}
