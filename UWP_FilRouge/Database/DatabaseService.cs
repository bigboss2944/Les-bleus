
using Entities;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UWP_FilRouge.Entities;
using Windows.Storage;


namespace UWP_FilRouge.Database
{
    public class DatabaseService
    {
        private SQLiteConnection sqliteConnection;

        public SQLiteConnection SqliteConnection
        {
            get { return sqliteConnection; }
        }
        public TableQuery<Shop> Shops
        {
            get { return this.sqliteConnection.Table<Shop>(); }
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

        public TableQuery<Order> Orders
        {
            get { return this.sqliteConnection.Table<Order>(); }
        }

        public List<Shop> ShopsEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Shop>(); }
        }

        public List<Order> OrdersEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Order>(); }
        }

        public List<Bicycle> BicyclesEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Bicycle>(); }
        }

        public List<Customer> CustomersEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Customer>(); }
        }

        public List<Seller> SellersEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Seller>(); }
        }

        public int Save(object item)
        {
            return this.sqliteConnection.InsertOrReplace(item);
        }

        public void SaveWithChildren(Seller item)
        {
            this.Save(item.FirstName);
            this.sqliteConnection.InsertOrReplaceWithChildren(item);
        }

        public DatabaseService()
        {

            Task.Factory.StartNew(async () =>
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile mydb = await localFolder.CreateFileAsync("FilRougeUWPdatabase.sqlite",
                        CreationCollisionOption.OpenIfExists);
                Debug.WriteLine("{0}", mydb.Path);
                this.sqliteConnection = new SQLiteConnection(mydb.Path);

                this.sqliteConnection.CreateTable<Shop>();
                this.sqliteConnection.CreateTable<Seller>();
                this.sqliteConnection.CreateTable<Bicycle>();
                this.sqliteConnection.CreateTable<Order>();
                this.sqliteConnection.CreateTable<Customer>();

            });
        }
    }
}
