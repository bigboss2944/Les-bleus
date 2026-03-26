namespace Entities
{
    /// <summary>
    /// Produit livrable : variante de <see cref="ProductType"/> pouvant être expédiée au client.
    /// Le discriminateur EF Core est <c>"DeliverableProduct"</c>.
    /// </summary>
    public class DeliverableProduct : ProductType
    {
        public DeliverableProduct() { }
    }
}
