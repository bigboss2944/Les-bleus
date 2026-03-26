namespace Entities
{
    /// <summary>
    /// Produit assuré : variante de <see cref="ProductType"/> pour lequel une assurance est incluse.
    /// Le discriminateur EF Core est <c>"InsuredProduct"</c>.
    /// </summary>
    public class InsuredProduct : ProductType
    {
        public InsuredProduct() { }
    }
}
