namespace Entities
{
    /// <summary>
    /// Produit échangeable : variante de <see cref="ProductType"/> pouvant faire l'objet d'un échange.
    /// Le discriminateur EF Core est <c>"ExchangeableProduct"</c>.
    /// </summary>
    public class ExchangeableProduct : ProductType
    {
        public ExchangeableProduct() { }
    }
}
