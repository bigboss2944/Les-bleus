using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UWP_FilRouge
{
    [Table("Shop")]
    public class Shop
    {
        #region Attributs
        private long? shopId;
        private string town;
        private int postalCode;
        private string adress;
        private string nameShop;
        private string phone;
        private string email;
        private string website;
        private Shop subShop;
        private Seller subSeller;
        private Customer subCustomer;
        private Bicycle subBicycle;

        //private List<Order> orders;
        //private List<Seller> sellers;
        //private List<Customer> customers;
        //private List<Bicycle> bicycles;




        #endregion

        #region Properties
        [PrimaryKey, AutoIncrement]
        public long? ShopId
        {
            get { return shopId; }
            set { shopId = value; }
        }

        public string Town
        {
            get { return town; }
            set { town = value; }
        }

        public int Postalcode
        {
            get { return postalCode; }
            set { postalCode = value; }
        }

        public string Adress
        {
            get { return adress; }
            set { adress = value; }
        }

        public string Nameshop
        {
            get { return nameShop; }
            set { nameShop = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Website
        {
            get { return website; }
            set { website = value; }
        }
 

        [ManyToOne]
        public Shop SubShop { get => subShop; set => subShop = value; }

        [ForeignKey(typeof(Shop))]
        public int SubShopId { get; set; }

        [ManyToOne]
        public Seller SubSeller { get => subSeller; set => subSeller = value; }

        [ManyToOne]
        public Customer SubCustomer { get => subCustomer; set => subCustomer = value; }

        [ManyToOne]
        public Bicycle SubBicycle { get => subBicycle; set => subBicycle = value; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Shop()
        {
            //this.Orders = new List<Order>();
            //this.Sellers = new List<Seller>();
            //this.Customers = new List<Customer>();
            //this.Bicycles = new  List<Bicycle>();
        }
        #endregion
    }
}
