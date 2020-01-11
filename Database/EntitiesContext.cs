using Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Database
{
    public class EntitiesContext : DbContext
    {
        #region Attributes
        private DbSet<Bicycle> bicycles;
        private DbSet<Customer> customers;
        private DbSet<Seller> sellers;
        private DbSet<Order> orders;
        private DbSet<Role> roles;
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

        public DbSet<Role> Roles
        {
            get { return roles; }
            set { roles = value; }
        }

        public DbSet<Shop> Shops
        {
            get { return shops; }
            set { shops = value; }
        }
        #endregion

        #region Constructors
        public EntitiesContext() : base()
        {
            if (this.Database.CreateIfNotExists())
            {
                Random random = new Random();

                //Implémentation de la table Shops
                Shop shop = new Shop();
                {
                    shop.Town = "rennes";
                    shop.Postalcode = 35000;
                    shop.Adress = "intermarché longchamps";
                    shop.Nameshop = "Les bleus bike shop";
                    shop.Phone = "+33 2 99 00 00 00";
                    shop.Email = "lesbleus@bikeshop.com";
                    shop.Website = "www.lesyeuxdanslesbleus.com";
                    this.Shops.Add(shop);
                    this.SaveChanges();
                }

                //Implémentation de la table Roles
                {
                    Role role1 = new Role();
                    role1.Name = "Admin";
                    Role role2 = new Role();
                    role2.Name = "Seller";
                    this.Roles.Add(role1);
                    this.Roles.Add(role2);
                    this.SaveChanges();
                }

                // Implémentation de la table Sellers
                for (int i = 1; i < 5; i++)
                {
                    Seller Seller = new Seller();
                    Seller.Role = this.Roles.Find(random.Next(1, this.Roles.Count()));
                    Seller.FirstName = "firstName" + i;
                    Seller.LastName = "lastName" + i;
                    Seller.Password = "password" + i;
                    Seller.Shop = this.Shops.Find(1);
                    this.Sellers.Add(Seller);
                    this.SaveChanges();
                }

                // Implémentation de la table Customers
                for (int i = 1; i < 5; i++)
                {
                    Customer customer = new Customer();
                    customer.Gender = "male";
                    customer.Address = "adress" + i;
                    customer.Email = "email" + i;
                    customer.FirstName = "firstname" + i;
                    customer.LastName = "lastname" + i;
                    customer.LoyaltyPoints = 0;
                    customer.Phone = "+33 6 00 00 0" + i;
                    customer.PostalCode = 35000;
                    customer.Town = "rennes";
                    customer.Shop = this.Shops.Find(1);
                    this.Customers.Add(customer);
                    this.SaveChanges();
                }

                // Implémentation de la table Orders 
                Order order = new Order();
                {
                    order.Date = DateTime.Now;
                    order.Discount = 0.1F;
                    order.Seller = this.Sellers.Find(random.Next(1, this.Sellers.Count()));
                    order.Customer = this.Customers.Find(random.Next(1, this.Sellers.Count()));
                    order.Shop = this.Shops.Find(1);
                    order.UseLoyaltyPoint = false;
                    order.Tax = 0.2F;
                    order.ShippingCost = 0;
                    order.PayMode = "CB";
                    this.Orders.Add(order);
                    this.SaveChanges();
                }

                // Implémentation de la table Bicycles
                Bicycle bicycle = new Bicycle();
                //non achetés
                for (int i = 1; i < 5; i++)
                {
                    bicycle.Shop = this.Shops.Find(1);
                    bicycle.Order = null;
                    bicycle.Customer = null;
                    bicycle.TypeOfBike = "xc";
                    bicycle.Exchangeable = true;
                    bicycle.Insurance = true;
                    bicycle.Deliverable = true;
                    bicycle.Category = "man";
                    bicycle.Reference = "xc000"+i;
                    bicycle.Size = 1.75F + i * 0.01F;
                    bicycle.Weight = 11.5F + i * 0.5F;
                    bicycle.Color = "red and black";
                    bicycle.Confort = "full";
                    bicycle.FreeTaxPrice = 2000F - i * 100;
                    bicycle.Electric = false;
                    bicycle.State = "new";
                    bicycle.Brand = "les bleus";
                    this.Bicycles.Add(bicycle);
                    this.SaveChanges();
                }
                //acheté
                {
                    bicycle.Shop = this.Shops.Find(1);
                    bicycle.Order = this.Orders.Find(1);
                    bicycle.Customer = this.Customers.Find(random.Next(1, this.Customers.Count()));
                    bicycle.TypeOfBike = "fitness";
                    bicycle.Exchangeable = true;
                    bicycle.Insurance = true;
                    bicycle.Deliverable = true;
                    bicycle.Category = "woman";
                    bicycle.Reference = "fitness0001";
                    bicycle.Size = 1.70F;
                    bicycle.Weight = 8.5F;
                    bicycle.Color = "yellow and white";
                    bicycle.Confort = "";
                    bicycle.FreeTaxPrice = 2000F;
                    bicycle.Electric = false;
                    bicycle.State = "new";
                    bicycle.Brand = "les bleus";
                    this.Bicycles.Add(bicycle);
                    this.SaveChanges();
                }
            }
        }
        #endregion

        #region functions
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasMany(o => o.Bicycles).WithOptional(b => b.Order);
            modelBuilder.Entity<Order>().HasOptional(o => o.Seller).WithMany(s => s.Orders);
            modelBuilder.Entity<Order>().HasOptional(o => o.Customer).WithMany(c => c.Orders);

            modelBuilder.Entity<Shop>().HasMany(s => s.Customers).WithOptional(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Sellers).WithOptional(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Orders).WithOptional(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Bicycles).WithOptional(b => b.Shop);

            modelBuilder.Entity<Role>().HasMany(r => r.Sellers).WithOptional(s => s.Role);

            base.OnModelCreating(modelBuilder);
        }
        #endregion

    }
}

