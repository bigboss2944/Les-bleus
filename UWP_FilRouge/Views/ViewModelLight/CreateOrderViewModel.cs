using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModelLight.FichiersASPNET;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class CreateOrderViewModel: ViewModelBase
    {

        public int Quantity { get; set; }
        public List<Bicycle> Bicycle { get; set; }
        public Seller Seller { get; set; }
        public Customer Customer { get; set; }
        public float sumFreeTax { get; set; }
        public Shop Shop { get; set; }
        public DateTime Date { get; set; }
        public String PayMode { get; set; }
        private float Discount { get; set; }
        public int? UseLoyaltyPoint { get; set; }
        public float Tax { get; set; }
        public float ShippingCost { get; set; }
        public float TotalAmount { get; set; }

        public CreateOrderViewModel()
        {
         this.Quantity = 50;
         this.Customer = new Customer();
         this.Bicycle = new List<Bicycle>();
         this.Seller = new Seller();
        
        }

    }
}
