using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Commons;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users
{
    public class CustomerListAccessor
    {
        public ObservableCollection<Customer> customers { get; set; }
        public ListViewAccessor<Customer> listView { get; set; }

        public CustomerListAccessor()
        {
            this.customers = new ObservableCollection<Customer>();
            this.listView = new ListViewAccessor<Customer>();
        }
    }
}
