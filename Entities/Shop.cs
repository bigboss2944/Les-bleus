using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Représente un magasin physique de la chaîne Les Bleus Cycles,
    /// avec ses coordonnées et les listes de vendeurs, clients, commandes et vélos rattachés.
    /// </summary>
    public class Shop
    {
        /// <summary>Identifiant unique du magasin (clé primaire auto-incrémentée).</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? ShopId { get; set; }

        /// <summary>Ville où est situé le magasin.</summary>
        public string? Town { get; set; }

        /// <summary>Code postal du magasin.</summary>
        public int PostalCode { get; set; }

        /// <summary>Adresse postale du magasin.</summary>
        public string? Address { get; set; }

        /// <summary>Nom commercial du magasin.</summary>
        public string? Name { get; set; }

        /// <summary>Numéro de téléphone du magasin.</summary>
        public string? Phone { get; set; }

        /// <summary>Adresse e-mail de contact du magasin.</summary>
        public string? Email { get; set; }

        /// <summary>URL du site web du magasin.</summary>
        public string? Website { get; set; }

        /// <summary>Commandes passées dans ce magasin.</summary>
        public List<Order> Orders { get; set; } = new List<Order>();

        /// <summary>Vendeurs affectés à ce magasin.</summary>
        public List<Seller> Sellers { get; set; } = new List<Seller>();

        /// <summary>Clients rattachés à ce magasin.</summary>
        public List<Customer> Customers { get; set; } = new List<Customer>();

        /// <summary>Vélos disponibles dans ce magasin.</summary>
        public List<Bicycle> Bicycles { get; set; } = new List<Bicycle>();
    }
}
