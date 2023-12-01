using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Entities;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Commons;
using Windows.UI.Xaml.Controls;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users
{
    public class BicycleListAccessor
    {
        public ObservableCollection<Bicycle> bicycles { get; set; }
        public ListViewAccessor<Bicycle> listView { get; set; }
        //public SellerEditAccessor sellerEdit { get; set; }
        //public ButtonAccessor deleteButton { get; set; }
        //public ButtonAccessor updateButton { get; set; }
        //public ButtonAccessor validateButton { get; set; }
        //public ButtonAccessor cancelButton { get; set; }
        public Bicycle bicycle { get; set; }


        public BicycleListAccessor()
        {
            this.bicycles = new ObservableCollection<Bicycle>(); //To fulfill the seller list
            this.listView = new ListViewAccessor<Bicycle>(); //To select a specific seller in the list
            //this.sellerEdit = new SellerEditAccessor();
            //this.deleteButton = new ButtonAccessor();// To delete a seller
            //this.updateButton = new ButtonAccessor();
            //this.updateButton = new ButtonAccessor();//Refresh the list
            //this.cancelButton = new ButtonAccessor();
            //this.seller = new Seller();
        }
    }
}
