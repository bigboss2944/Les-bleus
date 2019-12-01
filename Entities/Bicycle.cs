using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Bicycle : BicyleCharacteristics
    {
        #region Attributs
        private long id;
        //city fitness road xc 
        private string typeOfBike;
        //kids woman man
        private string category;
        private float freeTaxPrice;
        private bool exchangeable;
        private bool insurance;
        private bool deliverable;
        private List<Order> orders;
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

        //[ForeignKey()]
        public List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }
        #endregion

        #region Constructors
        /// <summary>

        /// Default constructor.

        /// </summary>

        public Bicycle()
        {
            this.Orders = new List<Order>();
        }

        //public Bicycle(float size,
        //               float weight,
        //               string color,
        //               float wheelSize,
        //               bool electric,
        //               string state,
        //               string brand,

        //               long id,
        //               string typeOfBike,
        //               string category,
        //               float freeTaxPrice,
        //               bool exchangeable,
        //               bool insurance,
        //               bool deliverable
        //               ) : base(size, weight, color, wheelSize, electric, state, brand)
        //{
        //    Id = id;
        //    TypeOfBike = typeOfBike;
        //    Category = category;
        //    FreeTaxPrice = freeTaxPrice;
        //    Exchangeable = exchangeable;
        //    Insurance = insurance;
        //    Deliverable = deliverable;
        //    Orders = new List<Order>();
        //}
        #endregion
    }
}
