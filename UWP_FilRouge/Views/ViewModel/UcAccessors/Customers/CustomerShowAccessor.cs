using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users
{
    public class CustomerShowAccessor
    {
        public Customer customer { get; set; }

        public CustomerShowAccessor()
        {
            this.customer = new Customer();
        }
    }
}
