using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users
{
    public class SellerShowAccessor
    {
        public Seller seller { get; set; }

        public SellerShowAccessor()
        {
            this.seller = new Seller();
        }
    }
}
