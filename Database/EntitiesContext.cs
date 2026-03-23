using AspNet_FilRouge.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Database
{
    public class EntitiesContext : DbContext
    {
        #region Attributes
        private DbSet<Bicycle> bicycles = null!;
        private DbSet<Customer> customers = null!;
        private DbSet<Seller> sellers = null!;
        private DbSet<Order> orders = null!;
        private DbSet<Shop> shops = null!;
        #endregion

        #region Properties
        public DbSet<Bicycle> Bicycles { get { return bicycles; } set { bicycles = value; } }
        public DbSet<Customer> Customers { get { return customers; } set { customers = value; } }
        public DbSet<Seller> Sellers { get { return sellers; } set { sellers = value; } }
        public DbSet<Order> Orders { get { return orders; } set { orders = value; } }
        public DbSet<Shop> Shops { get { return shops; } set { shops = value; } }
        #endregion

        #region Constructors
        public EntitiesContext() : base()
        {
        }

        public EntitiesContext(DbContextOptions<EntitiesContext> options) : base(options)
        {
        }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=EntitiesDb;Integrated Security=True");
                }
                else
                {
                    optionsBuilder.UseSqlite("Data Source=entities.db");
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasMany(o => o.Bicycles).WithOne(b => b.Order).IsRequired(false);
            modelBuilder.Entity<Order>().HasOne(o => o.Seller).WithMany(s => s.Orders).IsRequired(false);
            modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany(c => c.Orders).IsRequired(false);

            modelBuilder.Entity<Shop>().HasMany(s => s.Customers).WithOne(s => s.Shop).IsRequired(false);
            modelBuilder.Entity<Shop>().HasMany(s => s.Sellers).WithOne(s => s.Shop).IsRequired(false);
            modelBuilder.Entity<Shop>().HasMany(s => s.Orders).WithOne(s => s.Shop).IsRequired(false);
            modelBuilder.Entity<Shop>().HasMany(s => s.Bicycles).WithOne(b => b.Shop).IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
