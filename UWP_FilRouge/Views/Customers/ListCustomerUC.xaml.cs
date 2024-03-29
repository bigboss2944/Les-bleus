﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP_FilRouge.Entities;
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

namespace UWP_FilRouge.Views.Customers
{
    public sealed partial class ListCustomerUC : UserControl
    {
        public ObservableCollection<Customer> CustomerList { get; set; }

        public ListCustomerUC()
        {
            this.InitializeComponent();
            this.CustomerList = new ObservableCollection<Customer>(); //To show the seller list
            this.DataContext = CustomerList;
        }
    }
}
