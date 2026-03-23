using Microsoft.AspNetCore.Identity;

namespace AspNet_FilRouge_Vendeur.Models
{
    public class User : IdentityUser
    {
        #region Attributs
        protected string? lastName;
        protected string? firstName;
        #endregion

        #region Properties
        public string? LastName { get => lastName; set => lastName = value; }
        public string? FirstName { get => firstName; set => firstName = value; }
        #endregion

        #region Constructors
        public User() : base()
        {
        }
        #endregion
    }
}
