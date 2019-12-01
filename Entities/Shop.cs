using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Entities
{
    public class Shop
    {
        #region Attributs
        private long idShop;
        private string town;
        private int postalCode;
        private string adress;
        private string nameShop;
        private string phone;
        private string email;
        private string website;
        #endregion

        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdShop
        {
            get { return idShop; }
            set { idShop = value; }
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
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Shop()
        {

        }

        //public Shop(long idshop,
        //            string town,
        //            int postalcode,
        //            string adress,
        //            string nameshop,
        //            string phone,
        //            string email,
        //            string website)
        //{
        //    Idshop = idshop;
        //    Town = town;
        //    Postalcode = postalcode;
        //    Adress = adress;
        //    Nameshop = nameshop;
        //    Phone = phone;
        //    Email = email;
        //    Website = website;
        //}
        #endregion
    }
}
