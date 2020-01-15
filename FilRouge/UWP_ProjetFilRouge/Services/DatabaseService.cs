using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_ProjetFilRouge.Entities;
using Windows.Storage;

namespace UWP_ProjetFilRouge.Services
{
    public class DatabaseService
    {
        private SQLiteConnection sqliteConnection;

        public SQLiteConnection SqliteConnection
        {
            get { return sqliteConnection; }
        }

        public TableQuery<Role> Roles
        {
            get { return this.sqliteConnection.Table<Role>(); }
        }

        public TableQuery<User> Users
        {
            get { return this.sqliteConnection.Table<User>(); }
        }

        public List<Role> RolesEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<Role>(); }
        }

        public List<User> UsersEager
        {
            get { return this.sqliteConnection.GetAllWithChildren<User>(); }
        }

        public int Save(object item)
        {
            return this.sqliteConnection.InsertOrReplace(item);
        }

        public void SaveWithChildren(User item)
        {
            this.Save(item.Role);
            this.sqliteConnection.InsertOrReplaceWithChildren(item);
        }

        public DatabaseService()
        {
            Task.Factory.StartNew(async () =>
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile myDb = await localFolder.CreateFileAsync("mydb.sqlite",
                        CreationCollisionOption.OpenIfExists);
                this.sqliteConnection = new SQLiteConnection(myDb.Path);
                this.sqliteConnection.CreateTable<Role>();
                this.sqliteConnection.CreateTable<User>();

            });
        }
    }
}
