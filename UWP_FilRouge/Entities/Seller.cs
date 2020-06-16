using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge
{
    [Table("Seller")]
    public class Seller : User
    {
        private Shop subShop;
        private Seller subSeller;
        //public Action<Seller> OnRemoveCallback { get; set; }
        #region Attributes

        //private List<Order> orders;
        //private Shop shop;


        private enum RoleRight
        {
            basic,
            medium,
            admin
        }

        #endregion

        #region properties

        //[ManyToOne]      // Many to one relationship with Stock

        //public Order Order
        //{
        //    get; set;
        //}



        [ManyToOne]
        public Shop SubShop { get => subShop; set => subShop = value; }
        #endregion

        #region constructors
        public Seller() : base()
        {

        }

        [ManyToOne]
        public Seller SubSeller { get => subSeller; set => subSeller = value; }
        #endregion

        

        public void OnRemove()
        {
            //OnRemoveCallback(this);
        }

        #region Functions
        //public String ToString()
        //{
        //    //return base.Id + " " + this.password + " " + this.orders + " " + this.shop + " ";
        //}
        #endregion
    }
}