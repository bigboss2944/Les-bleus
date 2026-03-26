using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Représente une instance physique (article en stock) d'un <see cref="ProductType"/> donné.
    /// Permet de distinguer plusieurs exemplaires d'un même modèle de produit.
    /// </summary>
    public class PhysicalProduct
    {
        /// <summary>Identifiant unique de l'article physique (clé primaire auto-incrémentée).</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>Clé étrangère vers le type de produit correspondant.</summary>
        public long ProductTypeId { get; set; }

        /// <summary>Type de produit associé (peut être null si non chargé).</summary>
        public ProductType? ProductType { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
