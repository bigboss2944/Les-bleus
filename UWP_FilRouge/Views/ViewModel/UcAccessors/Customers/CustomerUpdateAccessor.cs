using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Entities;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Commons;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users
{
    public class CustomerUpdateAccessor
    {
        public ObservableCollection<Customer> customers { get; set; }
        public ListViewAccessor<Customer> listView { get; set; }
        public Customer customer { get; set; }
        //public UserControl editSellerUC { get; set; }
        public ButtonAccessor validateButton { get; set; }
        public ButtonAccessor cancelButton { get; set; }

        public CustomerUpdateAccessor()
        {
            this.customers = new ObservableCollection<Customer>(); //To fulfill the seller list
            this.listView = new ListViewAccessor<Customer>(); //To select a specific seller in the list
            this.customer = new Customer(); //To update the selected seller's informations 
            //this.editSellerUC = new UserControl();
            this.validateButton = new ButtonAccessor();
            this.cancelButton = new ButtonAccessor();
        }
    }
}
