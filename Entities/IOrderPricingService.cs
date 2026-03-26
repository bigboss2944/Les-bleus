namespace Entities
{
    /// <summary>
    /// Contrat pour le calcul du montant total d'une commande,
    /// en appliquant la remise commerciale, la TVA et les frais de livraison.
    /// </summary>
    public interface IOrderPricingService
    {
        /// <summary>
        /// Calcule le montant total TTC d'une commande à partir de la liste des vélos,
        /// du taux de remise, du taux de TVA et des frais de livraison.
        /// </summary>
        /// <param name="order">La commande dont le total doit être calculé.</param>
        /// <returns>Le montant total TTC de la commande (remise + TVA + frais de port).</returns>
        float CalculateTotal(Order order);
    }
}
