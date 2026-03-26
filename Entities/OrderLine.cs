using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Représente une ligne de commande, associant un <see cref="ProductType"/>
    /// à une <see cref="Order"/> avec la quantité commandée.
    /// </summary>
    public class OrderLine
    {
        /// <summary>Identifiant unique de la ligne de commande (clé primaire auto-incrémentée).</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>Clé étrangère vers la commande parente.</summary>
        public long OrderId { get; set; }

        /// <summary>Commande parente (peut être null si non chargée).</summary>
        public Order? Order { get; set; }

        /// <summary>Clé étrangère vers le type de produit commandé.</summary>
        public long ProductTypeId { get; set; }

        /// <summary>Type de produit commandé (peut être null si non chargé).</summary>
        public ProductType? ProductType { get; set; }

        /// <summary>Quantité commandée pour ce type de produit.</summary>
        public int Quantity { get; set; }
    }
}
