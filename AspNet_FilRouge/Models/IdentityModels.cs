using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNet_FilRouge.Models
{
    public class ApplicationUser : IdentityUser
    {
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
        }
    }
}
