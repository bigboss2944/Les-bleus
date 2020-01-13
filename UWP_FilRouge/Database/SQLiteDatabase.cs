using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.FilRouge.Entities;
using Entities;
using SQLiteNetExtensions.Extensions;
using Windows.Storage;
using UWP_FilRouge.Entities;

namespace UWP_FilRouge.Database
{
    
    class SQLiteDatabase
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

        public int Save(object item)
        {
            return this.sqliteConnection.InsertOrReplace(item);
        }

        public void SaveWithChildren(User item)
        {
            //this.Save(item.Role);
            this.sqliteConnection.InsertOrReplaceWithChildren(item);
        }

        //public DatabaseService()
        //{
        //    Task.Factory.StartNew(async () =>
        //    {
        //        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        //        StorageFile myDb = await localFolder.CreateFileAsync("mydb.sqlite",
        //                CreationCollisionOption.OpenIfExists);
        //        this.sqliteConnection = new SQLiteConnection(myDb.Path);
        //        this.sqliteConnection.CreateTable<Role>();
        //        this.sqliteConnection.CreateTable<User>();
        //    });
        //}

    }
}
