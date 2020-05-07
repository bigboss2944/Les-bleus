

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_FilRouge.Views.ViewModelLight
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private void CheckPassword(object sender, RoutedEventArgs e)
        {
            if (password.Text== confirm_password.Text)
            {
                //ToDatabase();
            }
        }

        private async void ToDatabase()
        {
            // Do Something
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //StorageFile myDb = await localFolder.CreateFileAsync("mydb.sqlite",
            //CreationCollisionOption.OpenIfExists);
            //using (var db = new SQLiteConnection(myDb.Path))
            //{
            //    db.CreateTable<User>();
            //    User user = new User() { FirstName = firstName.Text, Password = password.Text};
            //    db.Insert(user);
            //    //db.Update(user);
               
            //}
        }
    }
}
