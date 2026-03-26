using Microsoft.AspNetCore.Identity;

namespace Entities
{
    /// <summary>
    /// Classe de base pour les utilisateurs de l'application (clients et vendeurs),
    /// étendant <see cref="IdentityUser"/> avec les informations d'identité civile.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>Nom de famille de l'utilisateur.</summary>
        public string? LastName { get; set; }

        /// <summary>Prénom de l'utilisateur.</summary>
        public string? FirstName { get; set; }
    }
}
