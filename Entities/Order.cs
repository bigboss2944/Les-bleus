using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Order
    {
        #region Attributs
        private long idOrder;
        private int quantity;
        private List<Bicycle> bicycles;
        private Seller seller;
        private Customer customer;
        private float sumFreeTax;
        private DateTime date;
        private Shop shop;
        private Payment payMode;
        private float discount;
        private int loyaltyPoint;
        private int loyaltyPointUsed;
        private int loyaltyPointEarned;
        private int totalLoyaltyPoint;
        private float tax;
        private float shippingCost;
        private float totalAmount;
        #endregion

        #region Properties 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdOrder
        {
            get { return idOrder; }
            set { idOrder = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public List<Bicycle> Bicycles
        {
            get { return bicycles; }
            set { bicycles = value; }
        }

        public Seller Seller
        {
            get { return seller; }
            set { seller = value; }
        }

        public Customer Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        public float SumFreeTax
        {
            get { return sumFreeTax; }
            set { sumFreeTax = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        [Required]
        public Shop Shop
        {
            get { return shop; }
            set { shop = value; }
        }

        [Required]
        public Payment PayMode
        {
            get { return payMode; }
            set { payMode = value; }
        }

        public float Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        public int LoyaltyPoint
        {
            get { return loyaltyPoint; }
            set { loyaltyPoint = value; }
        }

        public int LoyaltyPointUsed
        {
            get { return loyaltyPointUsed; }
            set { loyaltyPointUsed = value; }
        }

        public int LoyaltyPointEarned
        {
            get { return loyaltyPointEarned; }
            set { loyaltyPointEarned = value; }
        }

        public int TotalLoyaltyPoint
        {
            get { return totalLoyaltyPoint; }
            set { totalLoyaltyPoint = value; }
        }

        public float Tax
        {
            get { return tax; }
            set { tax = value; }
        }

        public float ShippingCost
        {
            get { return shippingCost; }
            set { shippingCost = value; }
        }

        public float TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }
        #endregion

        #region Constructors
        public Order()
        {
            this.Bicycles = new List<Bicycle>();
        }

        //public Order(long idOrder,
        //             int quantity,
        //             Seller seller,
        //             Customer customer,
        //             float sumFreeTax,
        //             DateTime date,
        //             Shop shop,
        //             Payment payMode,
        //             float discount,
        //             int loyaltyPoint,
        //             float tax,
        //             float shippingCost,
        //             float totalAmount)
        //{
        //    IdOrder = idOrder;
        //    Quantity = quantity;
        //    Seller = seller;
        //    Customer = customer;
        //    SumFreeTax = sumFreeTax;
        //    Date = date;
        //    Shop = shop;
        //    PayMode = payMode;
        //    Discount = discount;
        //    LoyaltyPoint = loyaltyPoint;
        //    Tax = tax;
        //    ShippingCost = shippingCost;
        //    TotalAmount = totalAmount;
        //    Bicycles = new List<Bicycle>();
        //}
        #endregion

        #region Functions
        // relier bicycles et quantité pour définir le montant par bicycle et ensuite le montant global
        public float SumFreeTaxCalculated()
        {
            Bicycle bicycle = new Bicycle();
            sumFreeTax = bicycle.FreeTaxPrice * quantity;
            return sumFreeTax;
        }

        // Discount sur le HT
        // TVA de 20%
        //100 loyaltyPoint = 10 euros de réduction
        //fdp
        public float TotalAmountCalculated()
        {
            totalAmount = sumFreeTax * (1F - discount) * (1F + tax) - (loyaltyPointUsed * 0.1F) + shippingCost;
            return totalAmount;
        }
        #endregion

        public int LoyaltyPointCalculated()
        {
            totalLoyaltyPoint = (loyaltyPoint - loyaltyPointUsed) + loyaltyPointEarned;
            return totalLoyaltyPoint;
        }
    }
}
