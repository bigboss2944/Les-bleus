namespace Entities
{
    /// <summary>
    /// Caractéristiques physiques d'un produit du catalogue (taille, poids, couleur).
    /// Persisté en tant que type possédé (Owned Entity) par <see cref="ProductType"/>.
    /// </summary>
    public class ProductCharacteristics
    {
        /// <summary>Taille du produit en centimètres.</summary>
        public float Size { get; set; }

        /// <summary>Poids du produit en kilogrammes.</summary>
        public float Weight { get; set; }

        /// <summary>Couleur du produit.</summary>
        public string? Color { get; set; }
    }
}
