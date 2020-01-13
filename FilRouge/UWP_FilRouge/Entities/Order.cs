using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Entities
{
    public class Order:EntityBase 
    {
        private long idOrder;
        //quantity by reference 
        private int quantity;
        private List<Bicycle> bicycles;
        private Seller seller;
        private Customer customer;
        private float sumFreeTax;
        private DateTime date;
        private Shop shop;
        private string payMode;
        private float discount;
        private int? useLoyaltyPoint;
        //private int loyaltyPoint;
        //private int loyaltyPointUsed;
        //private int loyaltyPointEarned;
        //private int totalLoyaltyPoint;
        private float tax;
        private float shippingCost;
        private float totalAmount;

        public long IdOrder
        {
            get { return idOrder; }
            set
            {
                idOrder = value;
                OnPropertyChanged("IdOrder");
            }
        }

        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        /*public List<Bicycle> Bicycles
        {
            get { return bicycles; }
            set
            {
                bicycles = value;
                OnPropertyChanged("Bicycle");
            }
        }*/

        public Seller Seller
        {
            get { return seller; }
            set
            {
                seller = value;
                OnPropertyChanged("Seller");
            }
        }

        public Customer Customer
        {
            get { return customer; }
            set
            {
                customer = value;
                OnPropertyChanged("Customer");
            }
        }

        public float SumFreeTax
        {
            get { return sumFreeTax; }
            set
            {
                sumFreeTax = value;
                OnPropertyChanged("SumFreeTax");
            }

        }

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        public Shop Shop
        {
            get { return shop; }
            set
            {
                shop = value;
                OnPropertyChanged("Shop");
            }
        }

        public string PayMode
        {
            get { return payMode; }
            set
            {
                payMode = value;
                OnPropertyChanged("PayMode");
            }
        }

        public float Discount
        {
            get { return discount; }
            set
            {
                discount = value;
                OnPropertyChanged("Discount");
            }
        }

        public int? UseLoyaltyPoint
        {
            get { return useLoyaltyPoint; }
            set
            {
                useLoyaltyPoint = value;
                OnPropertyChanged("UseLoyaltyPoint");
            }
        }

        public float Tax
        {
            get { return tax; }
            set
            {
                tax = value;
                OnPropertyChanged("Tax");
            }
        }

        public float ShippingCost
        {
            get { return shippingCost; }
            set
            {
                shippingCost = value;
                OnPropertyChanged("ShippingCost");
            }
        }

        public float TotalAmount
        {
            get { return totalAmount; }
            set
            {
                totalAmount = value;
                OnPropertyChanged("TotalAmount");
            }
        }

        public override object Copy()
        {
            Order order = new Order();
            order.Id = this.Id;
            order.Quantity = this.Quantity;
            order.Customer = this.Customer;
            order.Date = this.Date;
            order.Discount = this.Discount;
            order.Seller = this.Seller;
            order.Shop = this.Shop;
            order.SumFreeTax = this.SumFreeTax;
            order.Tax = this.Tax;
            order.TotalAmount = this.TotalAmount;
            order.UseLoyaltyPoint = this.UseLoyaltyPoint;


            {
                return order;  
            }
        }

        public override void CopyFrom(object obj)
        {
            Order order = new Order();
            order.Id = this.Id;
            order.Quantity = this.Quantity;
            order.Customer = this.Customer;
            order.Date = this.Date;
            order.Discount = this.Discount;
            order.Seller = this.Seller;
            order.Shop = this.Shop;
            order.SumFreeTax = this.SumFreeTax;
            order.Tax = this.Tax;
            order.TotalAmount = this.TotalAmount;
            order.UseLoyaltyPoint = this.UseLoyaltyPoint;

            {
                
            }
        }
    }
    
}
