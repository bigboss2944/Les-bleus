using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Représente un vélo du catalogue, avec ses caractéristiques commerciales
    /// (prix, quantité en stock, options) et physiques héritées de <see cref="BicycleCharacteristics"/>.
    /// </summary>
    public class Bicycle : BicycleCharacteristics
    {
        /// <summary>Identifiant unique du vélo (clé primaire auto-incrémentée).</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>Type de vélo (ex. : route, VTT, ville, etc.).</summary>
        public string? TypeOfBike { get; set; }

        /// <summary>Catégorie du vélo (ex. : adulte, enfant, électrique, etc.).</summary>
        public string? Category { get; set; }

        /// <summary>Référence fournisseur du modèle.</summary>
        public string? Reference { get; set; }

        /// <summary>Prix hors taxe du vélo.</summary>
        public float FreeTaxPrice { get; set; }

        /// <summary>Taux de TVA applicable (en pourcentage).</summary>
        public float Tax { get; set; }

        /// <summary>Quantité disponible en stock (1 par défaut à la création).</summary>
        public int Quantity { get; set; } = 1;

        /// <summary>Indique si le vélo est échangeable.</summary>
        public bool Exchangeable { get; set; }

        /// <summary>Indique si une assurance est incluse.</summary>
        public bool Insurance { get; set; }

        /// <summary>Indique si le vélo peut être livré.</summary>
        public bool Deliverable { get; set; }

        /// <summary>Commande à laquelle ce vélo est associé (peut être null).</summary>
        public Order? Order { get; set; }

        /// <summary>Client propriétaire de ce vélo (peut être null).</summary>
        public Customer? Customer { get; set; }

        /// <summary>Magasin auquel ce vélo est rattaché (peut être null).</summary>
        public Shop? Shop { get; set; }

        /// <inheritdoc/>
        public override string ToString() =>
            $"Bicycle #{Id} | {TypeOfBike} | {Category} | {FreeTaxPrice:F2}€ HT | Qté: {Quantity}";
    }
}
