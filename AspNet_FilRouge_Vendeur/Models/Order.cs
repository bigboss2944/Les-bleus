using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNet_FilRouge_Vendeur.Models
{
    public enum PaymentMode
    {
        Cash,
        CreditCard,
        DebitCard,
        BankTransfer,
        Check
    }

    public class Order
    {
        #region Attributs
        private long idOrder;
        private List<Bicycle> bicycles = new();
        private List<OrderLine> orderLines = new();
        [Required]
        private Seller seller = new ();
        [Required]
        private Customer customer= new ();
        private DateTime date;
        private PaymentMode payMode;
        private float discount;
        private bool useLoyaltyPoint;
        private float tax;
        private float shippingCost;
        private bool isValidated;
        #endregion

        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdOrder { get { return idOrder; } set { idOrder = value; } }
        public List<Bicycle> Bicycles { get { return bicycles; } set { bicycles = value; } }
        public List<OrderLine> OrderLines { get { return orderLines; } set { orderLines = value; } }
        [Required]
        public Seller Seller { get { return seller; } set { seller = value; } }
        [Required]
        public Customer Customer { get { return customer; } set { customer = value; } }
        public DateTime Date { get { return date; } set { date = value; } }
        public PaymentMode PayMode { get { return payMode; } set { payMode = value; } }
        public float Discount { get { return discount; } set { discount = value; } }
        public bool UseLoyaltyPoint { get { return useLoyaltyPoint; } set { useLoyaltyPoint = value; } }
        public float Tax { get { return tax; } set { tax = value; } }
        public float ShippingCost { get { return shippingCost; } set { shippingCost = value; } }
        public bool IsValidated { get { return isValidated; } set { isValidated = value; } }
        #endregion

        #region Constructors
        public Order()
        {
            this.Bicycles = new List<Bicycle>();
            this.OrderLines = new List<OrderLine>();
        }
        #endregion

        public override string ToString()
        {
            return this.idOrder + " " + this.bicycles + " " + this.seller + " " + this.customer + " " + this.date + " " + this.payMode + " " + this.discount + " " + this.useLoyaltyPoint + " " + this.tax + " " + this.shippingCost;
        }
    }
}
