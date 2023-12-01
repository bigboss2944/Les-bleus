using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AspNet_FilRouge.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base(){ }
        public ApplicationRole(string roleName) : base(roleName) { }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Attributes
        private DbSet<Bicycle> bicycles;
        private DbSet<Customer> customers;
        private DbSet<Seller> sellers;
        private DbSet<Order> orders;
        
        private DbSet<Shop> shops;
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

        

        public DbSet<Shop> Shops
        {
            get { return shops; }
            set { shops = value; }
        }
        #endregion

        #region Constructors
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            if (this.Database.CreateIfNotExists())
            {
                
            }
        }
        #endregion

        #region Functions
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasRequired(u => u.Id);
            //Order
            modelBuilder.Entity<Order>().HasMany(o => o.Bicycles).WithOptional(b => b.Order);
            modelBuilder.Entity<Order>().HasOptional(o => o.Seller).WithMany(s => s.Orders);
            modelBuilder.Entity<Order>().HasOptional(o => o.Customer).WithMany(c => c.Orders);

            //Shop
            modelBuilder.Entity<Shop>().HasMany(s => s.Customers).WithOptional(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Sellers).WithOptional(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Orders).WithOptional(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Bicycles).WithOptional(b => b.Shop);
            
            //Shop
            //modelBuilder.Entity<Role>().HasMany(r => r.Sellers).WithOptional(s => s.Role);

            base.OnModelCreating(modelBuilder);
        }
        #endregion

        public System.Data.Entity.DbSet<AspNet_FilRouge.Models.RoleViewModel> RoleViewModels { get; set; }
    }
}