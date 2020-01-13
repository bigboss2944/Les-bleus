using Entities;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Entities;
using Windows.Storage;

namespace UWP_FilRouge.Services
{
    class DatabaseService
    {
        private SQLiteConnection sqliteConnection;

        public SQLiteConnection SqliteConnection
        {
            get { return sqliteConnection; }
        }

        public TableQuery<Order> Orders
        {
            get { return this.sqliteConnection.Table<Order>(); }
        }

        public TableQuery<Customer> Customers
        {
            get { return this.sqliteConnection.Table<Customer>(); }
        }

        public TableQuery<Seller> Sellers
        {
            get { return this.sqliteConnection.Table<Seller>(); }
        }

        public TableQuery<Bicycle> Bicycles
        {
            get { return this.sqliteConnection.Table<Bicycle>(); }
        }

        public List<Order> OrdersEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Order>(); }
        }

        public List<Customer> CustomersEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Customer>(); }
        }

        public List<Bicycle> BicyclesEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Bicycle>(); }
        }

        public List<Seller> SellersEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Seller>(); }
        }

        public int Save(object item)
        {
            return this.sqliteConnection.InsertOrReplace(item);
        }

        //public void SaveWithChildren(Seller item)
        //{
        //    this.Save(item.Role);
        //    this.sqliteConnection.InsertOrReplaceWithChildren(item);
        //}

        //public void SaveWithChildren(Bicycle item)
        //{
        //    this.Save(item.Role);
        //    this.sqliteConnection.InsertOrReplaceWithChildren(item);
        //}

        //public void SaveWithChildren(Order item)
        //{
        //    this.Save(item.Role);
        //    this.sqliteConnection.InsertOrReplaceWithChildren(item);
        //}

        //public void SaveWithChildren(Customer item)
        //{
        //    this.Save(item.Role);
        //    this.sqliteConnection.InsertOrReplaceWithChildren(item);
        //}


        public DatabaseService()
        {
            Task.Factory.StartNew(async () =>
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile myDb = await localFolder.CreateFileAsync("mydb.sqlite",
                        CreationCollisionOption.OpenIfExists);
                this.sqliteConnection = new SQLiteConnection(myDb.Path);
                this.sqliteConnection.CreateTable<Order>();
                this.sqliteConnection.CreateTable<Customer>();
                this.sqliteConnection.CreateTable<Bicycle>();
                this.sqliteConnection.CreateTable<Seller>();
            });
        }
    }
}
