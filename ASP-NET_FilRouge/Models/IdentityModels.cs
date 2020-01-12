using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities;
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

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Attributes
        private DbSet<Bicycle> bicycles;
        private DbSet<Customer> customers;
        private DbSet<Seller> sellers;
        private DbSet<Order> orders;
        private DbSet<Shop> shop;
        #endregion

        #region Properties
        public DbSet<Bicycle> Bicycles
        {
            get { return bicycles; }
            set { bicycles = value; }
        }

        public DbSet<Customer> Customers
        {
            get { return customers; }
            set { customers = value; }
        }

        public DbSet<Seller> Sellers
        {
            get { return sellers; }
            set { sellers = value; }
        }

        public DbSet<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        public DbSet<Shop> Shop
        {
            get { return shop; }
            set { shop = value; }
        }
        #endregion

        #region Constructors
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        #endregion

        #region Functions
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasMany(o => o.Bicycles).WithOptional(b => b.Order);
            modelBuilder.Entity<Order>().HasRequired(o => o.Seller).WithMany(s => s.Orders);
            modelBuilder.Entity<Order>().HasRequired(o => o.Customer).WithMany(c => c.Orders);
            modelBuilder.Entity<Customer>().HasMany(c => c.Orders).WithRequired(o => o.Customer);

            modelBuilder.Entity<Customer>().HasOptional(c => c.Shop).WithMany(s => s.Customers);
            modelBuilder.Entity<Shop>().HasMany(s => s.Sellers).WithRequired(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Orders).WithRequired(s => s.Shop);

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}