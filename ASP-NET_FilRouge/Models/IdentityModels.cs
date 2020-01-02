using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ASP_NET_FilRouge.Models
{
    // Vous pouvez ajouter des données de profil pour l'utilisateur en ajoutant d'autres propriétés à votre classe ApplicationUser. Pour en savoir plus, consultez https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Notez qu'authenticationType doit correspondre à l'élément défini dans CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Ajouter les revendications personnalisées de l’utilisateur ici
            return userIdentity;
        }
    }

    public class EntitiesContext : IdentityDbContext<ApplicationUser>
    {
        public EntitiesContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static EntitiesContext Create()
        {
            return new EntitiesContext();
        }

        public System.Data.Entity.DbSet<Entities.Bicycle> Bicycles { get; set; }

        public System.Data.Entity.DbSet<Entities.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<Entities.Login> Logins { get; set; }

        public System.Data.Entity.DbSet<Entities.Order> Orders { get; set; }

        public System.Data.Entity.DbSet<Entities.Seller> Sellers { get; set; }

        public System.Data.Entity.DbSet<Entities.Shop> Shops { get; set; }
    }
}