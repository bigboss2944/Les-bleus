using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWP_FilRouge.Views.ViewModelLight
{
    public sealed partial class CreateUserUC : UserControl
    {
        public CreateUserUC()
        {
            this.InitializeComponent();
            this.TextFirstName = "";
            this.TextLastName = "";

            this.DataContext = this;
            this.Loaded += User_Loaded;
        }

        public string TextFirstName { get; set; }

        public string TextLastName { get; set; }

        private void User_Loaded(object sender, RoutedEventArgs e)
        {


        }

        /*
        public ICommand ButtonClick => new RelayCommand(() =>
        {
            String firstName;
            String lastName;
            firstName = this.TextFirstName;
            lastName = this.TextLastName;
            Debug.WriteLine(TextFirstName);
            Debug.WriteLine(TextLastName);
        });*/



        public void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("Enter Creation Page");
        }
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("Leave Creation Page");
        }


    }
}