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
    public class CustomerListAccessor
    {
        public ObservableCollection<Customer> customers { get; set; }
        public ListViewAccessor<Customer> listView { get; set; }
        public CustomerEditAccessor customerEdit { get; set; }
        public ButtonAccessor deleteButton { get; set; }
        public ButtonAccessor updateButton { get; set; }
        public ButtonAccessor validateButton { get; set; }
        public ButtonAccessor cancelButton { get; set; }
        public Customer customer { get; set; }

        public CustomerListAccessor()
        {
            this.customers = new ObservableCollection<Customer>(); //To fulfill the seller list
            this.listView = new ListViewAccessor<Customer>(); //To select a specific seller in the list
            this.customerEdit = new CustomerEditAccessor();
            this.deleteButton = new ButtonAccessor();// To delete a seller
            //this.updateButton = new ButtonAccessor();
            this.updateButton = new ButtonAccessor();//Refresh the list
            this.cancelButton = new ButtonAccessor();
            //this.seller = new Seller();
        }
    }
}
