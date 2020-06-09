using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors
{
    public class CustomerPageAccessor
    {
        public CustomerEditAccessor customerEdit { get; set; }
        public CustomerListAccessor customerList { get; set; }
        public CustomerShowAccessor customerShow { get; set; }

        public CustomerPageAccessor()
        {
            this.customerEdit = new CustomerEditAccessor();
            this.customerList = new CustomerListAccessor();
            this.customerShow = new CustomerShowAccessor();
        }
    }
}
