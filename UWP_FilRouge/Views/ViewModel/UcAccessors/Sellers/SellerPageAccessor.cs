using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors
{
    public class SellerPageAccessor
    {
        public SellerEditAccessor sellerEdit { get; set; }
        public SellerListAccessor sellerList { get; set; }
        public SellerShowAccessor sellerShow { get; set; }

        public SellerPageAccessor()
        {
            this.sellerEdit = new SellerEditAccessor();
            this.sellerList = new SellerListAccessor();
            this.sellerShow = new SellerShowAccessor();
        }
    }
}
