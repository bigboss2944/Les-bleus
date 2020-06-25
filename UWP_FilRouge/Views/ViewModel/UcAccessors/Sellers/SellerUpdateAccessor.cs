using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModel;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Commons;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users
{
    public class SellerUpdateAccessor
    {
        public ObservableCollection<Seller> sellers { get; set; }
        public ListViewAccessor<Seller> listView { get; set; }
        public Seller seller { get; set; }
        //public UserControl editSellerUC { get; set; }
        public ButtonAccessor validateButton { get; set; }
        public ButtonAccessor cancelButton { get; set; }

        public SellerUpdateAccessor()
        {
            this.sellers = new ObservableCollection<Seller>();
            this.listView = new ListViewAccessor<Seller>();
            this.seller = new Seller();
            //this.editSellerUC = new UserControl();
            this.validateButton = new ButtonAccessor();
            this.cancelButton = new ButtonAccessor();
        }
    }
}
