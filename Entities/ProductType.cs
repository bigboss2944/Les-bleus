using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Classe de base pour les types de produits du catalogue (vélos, accessoires…).
    /// Utilise le pattern TPH (Table Per Hierarchy) avec le discriminateur
    /// <c>"ProductTypeDiscriminator"</c> pour les sous-types
    /// <see cref="DeliverableProduct"/>, <see cref="InsuredProduct"/> et <see cref="ExchangeableProduct"/>.
    /// </summary>
    public class ProductType
    {
        /// <summary>Identifiant unique du type de produit (clé primaire auto-incrémentée).</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>Caractéristiques physiques (taille, poids, couleur) possédées par ce type de produit.</summary>
        public ProductCharacteristics Characteristics { get; set; } = new ProductCharacteristics();

        /// <summary>Taille (délégation vers <see cref="ProductCharacteristics.Size"/>).</summary>
        [NotMapped]
        public float Size
        {
            get => Characteristics.Size;
            set => Characteristics.Size = value;
        }

        /// <summary>Poids (délégation vers <see cref="ProductCharacteristics.Weight"/>).</summary>
        [NotMapped]
        public float Weight
        {
            get => Characteristics.Weight;
            set => Characteristics.Weight = value;
        }

        /// <summary>Couleur (délégation vers <see cref="ProductCharacteristics.Color"/>).</summary>
        [NotMapped]
        public string? Color
        {
            get => Characteristics.Color;
            set => Characteristics.Color = value;
        }

        /// <summary>Référence fournisseur du produit.</summary>
        public string? Reference { get; set; }

        /// <summary>Prix hors taxe du produit.</summary>
        public float FreeTaxPrice { get; set; }

        /// <summary>Taux de TVA applicable (en pourcentage).</summary>
        public float Tax { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Id + " " + Reference + " " + Color + " " + FreeTaxPrice + " " + Tax;
        }
    }
}
