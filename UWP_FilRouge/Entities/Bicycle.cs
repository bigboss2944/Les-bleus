using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Entities
{
    public class Bicycle : BicycleCharacteristics
    {
        #region Attributs
        private long id;
        //city fitness road xc 
        private string typeOfBike;
        //kids woman man
        private string category;
        private string reference;
        private float freeTaxPrice;
        private bool exchangeable;
        private bool insurance;
        private bool deliverable;
        //private Order order;
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

        //[ForeignKey()]
        //public Order Order
        //{
        //    get { return order; }
        //    set { order = value; }
        //}
        #endregion

        public override object Copy()
        {
            Bicycle bicycle = new Bicycle();
            bicycle.Id = this.Id;
            bicycle.TypeOfBike = this.TypeOfBike;
            bicycle.Category = this.Category;
            bicycle.Reference = this.Reference;
            bicycle.FreeTaxPrice = this.FreeTaxPrice;
            bicycle.Exchangeable = this.Exchangeable;
            bicycle.Insurance = this.Insurance;
            bicycle.Deliverable = this.Deliverable;
            //bicycle.Order = this.Order;
            bicycle.Size = this.Size;
            bicycle.Weight = this.Weight;
            bicycle.Color = this.Color;
            bicycle.WheelSize = this.WheelSize;
            bicycle.Electric = this.Electric;
            bicycle.State = this.State;
            bicycle.Size = this.Size;
            bicycle.Brand = this.Brand;
            bicycle.Confort = this.Confort;

            return bicycle;
        }

        public override void CopyFrom(object obj)
        {
            Bicycle bicycle = obj as Bicycle;
            this.Size = bicycle.Size;
            this.Weight = bicycle.Weight;
            this.Color = bicycle.Color;
            this.WheelSize = bicycle.WheelSize;
            this.Electric = bicycle.Electric;
            this.State = bicycle.State;
            this.Size = bicycle.Size;
            this.Brand = bicycle.Brand;
            this.Confort = bicycle.Confort;
        }
    }
}
