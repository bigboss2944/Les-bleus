
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

using Windows.Storage;

namespace DataAccessLibraryUWP
{
    public class DataAccess
    {
        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Bicycles (Id INTEGER PRIMARY KEY, " +
                    "TypeOfBike NVARCHAR(2048) NULL, " + 
                    "Category NVARCHAR(2048) NULL, "+
                    "Reference NVARCHAR(2048) NULL, " +
                    "FreeTaxPrice REAL NON NULL, " +
                    "Exchangeable BIT NON NULL, " +
                    "Insurance BIT NON NULL, " +
                    "Delevareable BIT NON NULL, " +
                    "Size REAL NON NULL, " +
                    "Weight REAL NON NULL, " +
                    "Color NVARCHAR(2048) NULL, " +
                    "WheelSize REAL NON NULL, " +
                    "Electric BIT NON NULL, " +
                    "State NVARCHAR(2048) NULL, " +
                    "Brand NVARCHAR(2048) NULL, " +
                    "Confort NVARCHAR(2048) NULL, " +
                    "Order_IdOrder INTEGER NULL, " +
                    "Shop_ShopId INTEGER NULL, " +
                    "Customer_IdCustomer INTEGER NULL )";

                String tableCommand2 = "CREATE TABLE IF NOT " +
                    "EXISTS Customers (IdCustomer INTEGER PRIMARY KEY, " +
                    "Town NVARCHAR(2048) NULL, " +
                    "PostalCode NVARCHAR(2048) NULL, " +
                    "Address NVARCHAR(2048) NULL, " +
                    "LoyaltyPoints INTEGER NON NULL, " +
                    "Phone NVARCHAR(2048) NULL, " +
                    "Email NVARCHAR(2048) NULL, " +
                    "Gender NVARCHAR(2048) NULL, " +
                    "LastName NVARCHAR(2048) NULL, " +
                    "FirstName NVARCHAR(2048) NULL, " +
                    "Shop_ShopId INTEGER NULL)";

                String tableCommand3 = "CREATE TABLE IF NOT " +
                    "EXISTS Orders (IdOrder INTEGER PRIMARY KEY, " +
                    "Date DATETIME NULL, " +
                    "PayMode NVARCHAR(2048) NULL, " +
                    "Discount REAL NULL, " +
                    "UseLoyaltyPoint BIT NON NULL, " +
                    "Tax REAL NON NULL, " +
                    "ShippingCost REAL NON NULL, " +
                    "Seller_IdSeller INTEGER NULL, " +
                    "Shop_ShopId INTEGER NULL, " +
                    "Customer_IdCustomer INTEGER NULL )";

                String tableCommand4 = "CREATE TABLE IF NOT " +
                    "EXISTS Sellers (IdSeller INTEGER PRIMARY KEY, " +
                    "Password NVARCHAR(2048) NULL, " +
                    "LastName NVARCHAR(2048) NULL, " +
                    "FirstName NVARCHAR(2048) NULL, " +
                    "Role_Id NVARCHAR(2048) NON NULL, " +
                    "Shop_ShopId INTEGER NON NULL)";

                String tableCommand5 = "CREATE TABLE IF NOT " +
                    "EXISTS Shops (ShopId INTEGER PRIMARY KEY, " +
                    "Town NVARCHAR(2048) NULL, " +
                    "Address NVARCHAR(2048) NULL, " +
                    "NameShop NVARCHAR(2048) NULL, " +
                    "Phone NVARCHAR(2048) NULL, " +
                    "Email NVARCHAR(2048) NULL, " +
                    "Website NVARCHAR(2048) NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                SqliteCommand createTable2 = new SqliteCommand(tableCommand2, db);
                SqliteCommand createTable3 = new SqliteCommand(tableCommand3, db);
                SqliteCommand createTable4 = new SqliteCommand(tableCommand4, db);
                SqliteCommand createTable5 = new SqliteCommand(tableCommand5, db);

                createTable.ExecuteReader();
                createTable2.ExecuteReader();
                createTable3.ExecuteReader();
                createTable4.ExecuteReader();
                createTable5.ExecuteReader();
            }
        }

        public static void AddData(string inputText)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", inputText);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }

        public static List<String> GetData()
        {
            List<String> entries = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT Text_Entry from MyTable", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }

                db.Close();
            }

            return entries;
        }
    }
}
