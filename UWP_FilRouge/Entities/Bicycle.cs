using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge
{
    public class Bicycle : BicyleCharacteristics
    {
        #region Attributs
        private long id;
        private string typeOfBike;//city fitness road xc 
        private string category;//kids woman man
        private string reference;
        private float freeTaxPrice;
        private bool exchangeable;
        private bool insurance;
        private bool deliverable;
        private Order order;
        private Customer customer;
        private Shop shop;
        #endregion

        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        public string TypeOfBike
        {
            get { return typeOfBike; }
            set { typeOfBike = value; }
        }

        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        public string Reference
        {
            get { return reference; }
            set { reference = value; }
        }

        public float FreeTaxPrice
        {
            get { return freeTaxPrice; }
            set { freeTaxPrice = value; }
        }

        public bool Exchangeable
        {
            get { return exchangeable; }
            set { exchangeable = value; }
        }

        public bool Insurance
        {
            get { return insurance; }
            set { insurance = value; }
        }

        public bool Deliverable
        {
            get { return deliverable; }
            set { deliverable = value; }
        }

        public Order Order
        {
            get { return order; }
            set { order = value; }
        }

        public Customer Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        public Shop Shop
        {
            get { return shop; }
            set { shop = value; }
        }
        #endregion

        #region Constructors
        /// <summary>

        /// Default constructor.

        /// </summary>

        public Bicycle()
        {
            
        }
        #endregion

        #region Functions
        public String ToString() 
            {
            return this.id + " " + this.typeOfBike + " " + this.category + " " + this.freeTaxPrice + " " + this.exchangeable + " " + this.insurance + " " + this.deliverable;
            }
        #endregion
    }
}
