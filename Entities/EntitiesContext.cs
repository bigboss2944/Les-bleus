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
        public DbSet<ProductType> ProductTypes { get; set; } = null!;
        public DbSet<PhysicalProduct> PhysicalProducts { get; set; } = null!;
        public DbSet<Stock> Stocks { get; set; } = null!;
        public DbSet<OrderLine> OrderLines { get; set; } = null!;

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

            modelBuilder.Entity<ProductType>()
                .HasDiscriminator<string>("ProductTypeDiscriminator")
                .HasValue<ProductType>("ProductType")
                .HasValue<DeliverableProduct>("DeliverableProduct")
                .HasValue<InsuredProduct>("InsuredProduct")
                .HasValue<ExchangeableProduct>("ExchangeableProduct");

            modelBuilder.Entity<ProductType>()
                .OwnsOne(pt => pt.Characteristics, characteristics =>
                {
                    characteristics.Property(c => c.Size).HasColumnName("Size");
                    characteristics.Property(c => c.Weight).HasColumnName("Weight");
                    characteristics.Property(c => c.Color).HasColumnName("Color");
                });

            modelBuilder.Entity<PhysicalProduct>()
                .HasOne(p => p.ProductType)
                .WithMany()
                .HasForeignKey(p => p.ProductTypeId)
                .IsRequired();

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.ProductType)
                .WithOne()
                .HasForeignKey<Stock>(s => s.ProductTypeId)
                .IsRequired(false);

            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLines)
                .HasForeignKey(ol => ol.OrderId)
                .IsRequired(false);

            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.ProductType)
                .WithMany()
                .HasForeignKey(ol => ol.ProductTypeId)
                .IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
