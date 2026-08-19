namespace Entities
{
    /// <summary>
    /// Résultat d'une opération métier sur une commande (statut applicatif, indépendant du protocole HTTP).
    /// </summary>
    public enum OrderOperationStatus
    {
        Success,
        NotFound,
        InvalidState
    }

    /// <inheritdoc cref="OrderOperationStatus"/>
    public class OrderOperationResult
    {
        public OrderOperationStatus Status { get; init; }
        public string? Error { get; init; }
        public string? Message { get; init; }
        public float Total { get; init; }
    }
}
