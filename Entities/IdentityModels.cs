using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Nom")]
        public string? LastName { get; set; }

        [Display(Name = "Prénom")]
        public string? FirstName { get; set; }

        [Display(Name = "Adresse")]
        public string? Address { get; set; }
    }

    public class StockRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nom du vélo")]
        public string? BicycleName { get; set; }

        [Required]
        [Display(Name = "Quantité")]
        public int Quantity { get; set; }

        [Display(Name = "Date de demande")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Display(Name = "Statut")]
        public string Status { get; set; } = "En attente";

        public string? RequestedById { get; set; }

        [Display(Name = "Demandé par")]
        public ApplicationUser? RequestedBy { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bicycle> Bicycles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<RoleViewModel> RoleViewModels { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<PhysicalProduct> PhysicalProducts { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>().HasMany(o => o.Bicycles).WithOne(b => b.Order).IsRequired(false);
            modelBuilder.Entity<Order>().HasOne(o => o.Seller).WithMany(s => s.Orders).IsRequired(false);
            modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany(c => c.Orders).IsRequired(false);

            modelBuilder.Entity<Shop>().HasMany(s => s.Customers).WithOne(s => s.Shop).IsRequired(false);
            modelBuilder.Entity<Shop>().HasMany(s => s.Sellers).WithOne(s => s.Shop).IsRequired(false);
            modelBuilder.Entity<Shop>().HasMany(s => s.Orders).WithOne(s => s.Shop).IsRequired(false);
            modelBuilder.Entity<Shop>().HasMany(s => s.Bicycles).WithOne(b => b.Shop).IsRequired(false);

            // The custom Role entity is not used in this context (ASP.NET Identity handles roles).
            // Ignoring it prevents EF Core from generating a RoleId FK column that doesn't exist in the DB.
            modelBuilder.Entity<Seller>().Ignore(s => s.Role);

            modelBuilder.Entity<ProductType>()
                .HasDiscriminator<string>("ProductTypeDiscriminator")
                .HasValue<ProductType>("ProductType")
                .HasValue<DeliverableProduct>("DeliverableProduct")
                .HasValue<InsuredProduct>("InsuredProduct")
                .HasValue<ExchangeableProduct>("ExchangeableProduct");

            modelBuilder.Entity<PhysicalProduct>().HasMany(p => p.ProductTypes).WithMany();

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
        }
    }
}
