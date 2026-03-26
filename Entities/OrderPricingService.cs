namespace Entities
{
    /// <summary>
    /// Implémentation du calcul de prix d'une commande.
    /// Applique la remise commerciale sur le sous-total HT, puis la TVA,
    /// et ajoute enfin les frais de livraison pour obtenir le montant TTC final.
    /// </summary>
    public class OrderPricingService : IOrderPricingService
    {
        /// <inheritdoc/>
        public float CalculateTotal(Order order)
        {
            // Sous-total HT : somme des prix unitaires HT de tous les vélos de la commande
            float subtotal = order.Bicycles?.Sum(b => b.FreeTaxPrice) ?? 0f;

            // Application de la remise commerciale (Discount est exprimé en pourcentage)
            float afterDiscount = subtotal * (1 - order.Discount / 100f);

            // Application de la TVA (Tax est exprimé en pourcentage)
            float withTax = afterDiscount * (1 + order.Tax / 100f);

            // Ajout des frais de livraison pour obtenir le total TTC
            return withTax + order.ShippingCost;
        }
    }
}
