using AspNet_FilRouge.Models;

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
        public EntitiesContext() : base()
        {
            
        }
        #endregion

        #region functions
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasMany(u => u.id).WithOptional(b => b.Order);

            modelBuilder.Entity<Order>().HasMany(o => o.Bicycles).WithOptional(b => b.Order);
            modelBuilder.Entity<Order>().HasOptional(o => o.Seller).WithMany(s => s.Orders);
            modelBuilder.Entity<Order>().HasOptional(o => o.Customer).WithMany(c => c.Orders);

            modelBuilder.Entity<Shop>().HasMany(s => s.Customers).WithOptional(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Sellers).WithOptional(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Orders).WithOptional(s => s.Shop);
            modelBuilder.Entity<Shop>().HasMany(s => s.Bicycles).WithOptional(b => b.Shop);

            //modelBuilder.Entity<Role>().HasMany(r => r.Sellers).WithOptional(s => s.Role);
            base.OnModelCreating(modelBuilder);
        }
        #endregion

    }
}

