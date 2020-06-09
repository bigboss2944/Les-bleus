
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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

        

        public TableQuery<Seller> Sellers
        {
            get { return this.sqliteConnection.Table<Seller>(); }
        }

        public TableQuery<Customer> Customers
        {
            get { return this.sqliteConnection.Table<Customer>(); }
        }

        public TableQuery<Bicycle> Bicycles
        {
            get { return this.sqliteConnection.Table<Bicycle>(); }
        }

        public TableQuery<Order> Orders
        {
            get { return this.sqliteConnection.Table<Order>(); }
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

        public async void InitializeDatabase()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile mydb = await localFolder.CreateFileAsync("FilRougeUWPdatabase.sqlite",
                    CreationCollisionOption.OpenIfExists);
            Debug.WriteLine("{0}", mydb.Path);
            this.sqliteConnection = new SQLiteConnection(mydb.Path);

            sqliteConnection.CreateTable<Seller>();
            sqliteConnection.CreateTable<Bicycle>();
            sqliteConnection.CreateTable<Order>();
            sqliteConnection.CreateTable<Customer>();

            //Task.Factory.StartNew(async () =>
            //{
            //    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //    StorageFile mydb = await localFolder.CreateFileAsync("FilRougeUWPdatabase.sqlite",
            //            CreationCollisionOption.OpenIfExists);
            //    Debug.WriteLine("{0}",mydb.Path);
            //    this.sqliteConnection = new SQLiteConnection(mydb.Path);
                
            //    sqliteConnection.CreateTable<Seller>();
            //    sqliteConnection.CreateTable<Bicycle>();
            //    sqliteConnection.CreateTable<Order>();
            //    sqliteConnection.CreateTable<Customer>();
                
            //});
        }
    }
}
