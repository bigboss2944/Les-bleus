using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Seller : User
    {
        #region Attributes
        private long idSeller;
        private string category;
        private string password;
        private List<Order> orders;
        #endregion

        #region properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdSeller 
        { 
            get => idSeller; 
            set => idSeller = value; 
        }
        public string Category 
        { 
            get => category; 
            set => category = value; 
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

        public List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }   
        #endregion

        #region constructors
        public Seller()
        {
            this.Orders = new List<Order>();
        }

        //public Seller(string lastname,
        //              string firstname,
        //              string category,
        //              string password) : base(lastname, firstname)
        //{
        //    Category = category;
        //    Password = password;
        //    Orders = new List<Order>();
        //}

        #endregion
    }
}
