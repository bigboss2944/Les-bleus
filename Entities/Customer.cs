using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Représente un client de la boutique, avec ses coordonnées,
    /// son historique de commandes et ses points de fidélité.
    /// </summary>
    public class Customer : User
    {
        /// <summary>Ville de résidence du client.</summary>
        public string? Town { get; set; }

        /// <summary>Code postal de l'adresse du client.</summary>
        public int PostalCode { get; set; }

        /// <summary>Adresse postale du client.</summary>
        public string? Address { get; set; }

        /// <summary>Points de fidélité accumulés par le client.</summary>
        public int LoyaltyPoints { get; set; }

        /// <summary>Numéro de téléphone du client.</summary>
        public string? Phone { get; set; }

        /// <summary>Adresse e-mail du client (surcharge de la propriété Identity).</summary>
        public new string? Email { get; set; }

        /// <summary>Magasin de référence du client (peut être null).</summary>
        public Shop? Shop { get; set; }

        /// <summary>Liste des commandes passées par le client.</summary>
        public List<Order> Orders { get; set; } = new List<Order>();

        /// <summary>Genre du client (ex. : M, F, autre).</summary>
        public string? Gender { get; set; }
    }
}
