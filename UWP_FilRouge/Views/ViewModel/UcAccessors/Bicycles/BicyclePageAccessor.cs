using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Users;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors
{
    public class BicyclePageAccessor
    {
        //public SellerEditAccessor sellerEdit { get; set; }
        public BicycleListAccessor bicycleList { get; set; }
        //public SellerUpdateAccessor sellerUpdate { get; set; }

        public BicyclePageAccessor()
        {
            //this.sellerEdit = new SellerEditAccessor(); //Allows to access created seller's informations
            this.bicycleList = new BicycleListAccessor();//Allows to access seller list informations
            //this.sellerUpdate = new SellerUpdateAccessor();//Allows to access updated seller's informations
        }
    }
}
