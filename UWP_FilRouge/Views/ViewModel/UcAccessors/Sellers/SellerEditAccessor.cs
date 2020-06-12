using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Commons;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users
{
    public class SellerEditAccessor
    {
        public Seller seller { get; set; }
        public ButtonAccessor validateButton { get; set; }
        public ButtonAccessor cancelButton { get; set; }

        public SellerEditAccessor()
        {
            this.seller = new Seller();
            this.validateButton = new ButtonAccessor();
            this.cancelButton = new ButtonAccessor();
        }
    }
}
