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

    public class EntitiesContext : IdentityDbContext<ApplicationUser>
    {
        public static EntitiesContext Create()
        {
            return new EntitiesContext();
        }

        #region Properties
        public System.Data.Entity.DbSet<Entities.Bicycle> Bicycles { get; set; }

        public System.Data.Entity.DbSet<Entities.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<Entities.Order> Orders { get; set; }

        public System.Data.Entity.DbSet<Entities.Seller> Sellers { get; set; }

        public System.Data.Entity.DbSet<Entities.Shop> Shops { get; set; }
        #endregion

        #region Constructors
        public EntitiesContext()
          : base("DefaultConnection", throwIfV1Schema: false)
        {
            try
            {
                if (this.Database.CreateIfNotExists())
                {
                    // Implémentation de la db Bicycles
                    Random random = new Random();
                    for (int i = 1; i < 5; i++)
                    {
                        Bicycle bicycle = new Bicycle();
                        bicycle.Order = this.Orders.Find(random.Next(1, this.Orders.Count()));
                        bicycle.TypeOfBike = "";
                        bicycle.Exchangeable = true;
                        bicycle.Insurance = true;
                        bicycle.Deliverable = true;
                        bicycle.Category = "";
                        bicycle.Reference = "";
                        bicycle.Size = 1.75F + i * 0.01F;
                        bicycle.Weight = 11.5F + i * 0.5F;
                        bicycle.Color = "";
                        bicycle.Confort = "";
                        bicycle.FreeTaxPrice = 2000F - i * 100;
                        bicycle.Electric = true;
                        bicycle.State = "";
                        bicycle.Brand = "";
                        this.Bicycles.Add(bicycle);
                        this.SaveChanges();
                    }

                    // Implémentation de la db Sellers
                    for (int i = 1; i < 5; i++)
                    {
                        Seller Seller = new Seller();
                        Seller.Category = "seller" + i;
                        Seller.FirstName = "firstName" + i;
                        Seller.LastName = "lastName" + i;
                        Seller.Password = "password" + i;
                        this.Sellers.Add(Seller);
                        this.SaveChanges();
                    }

                    // Implémentation de la Db customers
                    for (int i = 1; i < 5; i++)
                    {
                        Customer customer = new Customer();
                        customer.Address = "adress" + i;
                        customer.Email = "email" + i;
                        customer.FirstName = "firstname" + i;
                        customer.LastName = "lastname" + i;
                        customer.LoyaltyPoints = 0;
                        customer.Orders = null;
                        customer.Phone = "+33..." + i;
                        customer.PostalCode = 35000;
                        customer.Town = "rennes";
                        this.Customers.Add(customer);
                        this.SaveChanges();
                    }

                    //Instanciation
                    Shop shop1 = new Shop();

                    // Implémentation de la db Orders 
                    for (int i = 1; i < 5; i++)
                    {
                        Seller seller1 = new Seller();
                        Customer customer1 = new Customer();

                        Order order = new Order();
                        order.Date = DateTime.Now;
                        order.Discount = 0.1F;
                        order.Seller = seller1;
                        order.Customer = customer1;
                        order.Shop = shop1;
                        order.UseLoyaltyPoint = null;
                        //order.LoyaltyPoint = 5 + i;
                        //order.LoyaltyPointUsed = 0;
                        //order.LoyaltyPointEarned = 0;
                        //order.TotalLoyaltyPoint = order.LoyaltyPointCalculated();
                        //order.SumFreeTax = order.SumFreeTaxCalculated();
                        order.Tax = 0.2F;
                        order.ShippingCost = 0;
                        order.PayMode = "CB";
                        //order.TotalAmount = order.TotalAmountCalculated();
                        this.Orders.Add(order);
                        this.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region functions
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasMany(o => o.Bicycles).WithOptional(b => b.Order);
            modelBuilder.Entity<Order>().HasRequired(o => o.Seller).WithMany(s => s.Orders);
            modelBuilder.Entity<Order>().HasRequired(o => o.Customer).WithMany(c => c.Orders);
            modelBuilder.Entity<Customer>().HasMany(c => c.Orders).WithRequired(o => o.Customer);

            modelBuilder.Entity<Customer>().HasOptional(c => c.Shop).WithMany(s => s.Customers);
            //modelBuilder.Entity<Shop>().HasMany(s => s.Sellers).WithRequired(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Orders).WithRequired(s => s.Shop);

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}