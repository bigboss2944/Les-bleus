using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Commons;
using Windows.UI.Xaml.Controls;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users
{
    public class SellerListAccessor
    {
        public ObservableCollection<Seller> sellers { get; set; }
        public ListViewAccessor<Seller> listView { get; set; }
        public SellerEditAccessor sellerEdit { get; set; }
        public ButtonAccessor deleteButton { get; set; }
        public ButtonAccessor updateButton { get; set; }
        public ButtonAccessor validateButton { get; set; }
        public ButtonAccessor cancelButton { get; set; }
        public Seller seller { get; set; }


        public SellerListAccessor()
        {
            this.sellers = new ObservableCollection<Seller>();
            this.listView = new ListViewAccessor<Seller>();
            this.sellerEdit = new SellerEditAccessor();
            this.deleteButton = new ButtonAccessor();
            //this.updateButton = new ButtonAccessor();
            this.updateButton = new ButtonAccessor();
            this.cancelButton = new ButtonAccessor();
            //this.seller = new Seller();
        }
    }
}
