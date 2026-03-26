using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Représente une commande passée par un client ou créée par un vendeur,
    /// regroupant les vélos commandés, les lignes de commande, les conditions
    /// tarifaires (remise, taxe, frais de port) et l'état de validation.
    /// </summary>
    public class Order
    {
        /// <summary>Identifiant unique de la commande (clé primaire auto-incrémentée).</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdOrder { get; set; }

        /// <summary>Liste des vélos associés à cette commande.</summary>
        public List<Bicycle> Bicycles { get; set; } = new List<Bicycle>();

        /// <summary>Lignes de commande (produits du catalogue).</summary>
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

        /// <summary>Vendeur ayant créé la commande (peut être null).</summary>
        public Seller? Seller { get; set; }

        /// <summary>Client destinataire de la commande (peut être null).</summary>
        public Customer? Customer { get; set; }

        /// <summary>Date de création de la commande.</summary>
        public DateTime Date { get; set; }

        /// <summary>Magasin associé à la commande (peut être null).</summary>
        public Shop? Shop { get; set; }

        /// <summary>Mode de paiement (ex. : carte, virement, espèces).</summary>
        public string? PayMode { get; set; }

        /// <summary>Remise commerciale appliquée à la commande (en pourcentage).</summary>
        public float Discount { get; set; }

        /// <summary>Indique si les points de fidélité du client sont utilisés.</summary>
        public bool UseLoyaltyPoint { get; set; }

        /// <summary>Taux de TVA appliqué à la commande (en pourcentage).</summary>
        public float Tax { get; set; }

        /// <summary>Frais de livraison de la commande.</summary>
        public float ShippingCost { get; set; }

        /// <summary>Indique si la commande a été validée (stock déduit).</summary>
        public bool IsValidated { get; set; }
    }
}
