namespace Entities
{
    /// <summary>
    /// Représente un vendeur de la boutique, rattaché à un magasin
    /// et possédant un historique de commandes traitées.
    /// </summary>
    public class Seller : User
    {
        /// <summary>Rôle métier du vendeur (distinct du rôle Identity).</summary>
        public Role? Role { get; set; }

        /// <summary>Liste des commandes traitées par ce vendeur.</summary>
        public List<Order> Orders { get; set; } = new List<Order>();

        /// <summary>Magasin auquel le vendeur est affecté (peut être null).</summary>
        public Shop? Shop { get; set; }
    }
}
