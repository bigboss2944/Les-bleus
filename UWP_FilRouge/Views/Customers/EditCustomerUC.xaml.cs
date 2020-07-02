using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class EditCustomerUC : UserControl
    {
        private Customer customer;

        public Customer Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        public EditCustomerUC()
        {
            this.InitializeComponent();
            this.Customer = new Customer();
            this.DataContext = Customer;
        }
    }
}
