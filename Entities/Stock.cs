using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Représente le stock disponible pour un <see cref="ProductType"/> donné.
    /// Relation 1:1 avec <see cref="ProductType"/> via la clé étrangère <see cref="ProductTypeId"/>.
    /// </summary>
    public class Stock
    {
        /// <summary>Identifiant unique du stock (clé primaire auto-incrémentée).</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>Clé étrangère vers le type de produit associé.</summary>
        public long ProductTypeId { get; set; }

        /// <summary>Type de produit associé (peut être null si non chargé).</summary>
        public ProductType? ProductType { get; set; }

        /// <summary>Quantité en stock pour ce type de produit.</summary>
        public int Quantity { get; set; }

        /// <inheritdoc/>
        public override string ToString() =>
            $"Stock #{Id} | ProductType: {ProductTypeId} | Qté: {Quantity}";
    }
}
