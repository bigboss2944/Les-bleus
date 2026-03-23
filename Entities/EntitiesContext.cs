using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class EntitiesContext : DbContext
    {
        public DbSet<Bicycle> Bicycles { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Seller> Sellers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Shop> Shops { get; set; } = null!;

        public EntitiesContext() : base()
        {
        }

        public EntitiesContext(DbContextOptions<EntitiesContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=EntitiesDb;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasMany(o => o.Bicycles).WithOne(b => b.Order).IsRequired(false);
            modelBuilder.Entity<Order>().HasOne(o => o.Seller).WithMany(s => s.Orders).IsRequired(false);
            modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany(c => c.Orders).IsRequired(false);
            modelBuilder.Entity<Customer>().HasMany(c => c.Orders).WithOne(o => o.Customer).IsRequired(false);
            modelBuilder.Entity<Customer>().HasOne(c => c.Shop).WithMany(s => s.Customers).IsRequired(false);
            modelBuilder.Entity<Shop>().HasMany(s => s.Orders).WithOne(s => s.Shop).IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
