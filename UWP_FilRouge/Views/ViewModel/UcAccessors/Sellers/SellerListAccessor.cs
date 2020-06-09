using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Commons;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users
{
    public class SellerListAccessor
    {
        public ObservableCollection<Seller> sellers { get; set; }
        public ListViewAccessor<Seller> listView { get; set; }

        public SellerListAccessor()
        {
            this.sellers = new ObservableCollection<Seller>();
            this.listView = new ListViewAccessor<Seller>();
        }
    }
}
