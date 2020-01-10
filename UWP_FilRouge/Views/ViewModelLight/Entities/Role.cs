using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModelLight.FichiersASPNET;
using TableAttribute = SQLite.TableAttribute;

namespace UWP_FilRouge.Views.ViewModelLight.Entities
{
    [Table("Role")]
    public class Role: EntityBase
    {
       
        private long idOrder;

        [PrimaryKey, AutoIncrement]
        public long IdOrder
        {
            get { return idOrder; }
            set
            {
                idOrder = value;
                OnPropertyChanged("IdOrder");
            }
        }


        private String quantity;

        public String Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged("Quantite");
            }
        }

       
        private List<Bicycle> bicycle;

        public List<Bicycle> Bicycle
        {
            get { return bicycle; }
            set
            {
                bicycle = value;
                OnPropertyChanged("Bicycle");
            }
        }


        private Customer customer;

        public Customer Customer
        {
            get { return customer; }
            set
            {
                Customer = value;
                OnPropertyChanged("Customere");
            }
        }

        private float sumFreeTax;

        public float SumFreeTax
        {
            get { return sumFreeTax; }
            set
            {
                sumFreeTax = value;
                OnPropertyChanged("SumFreeTax");
            }
        }

        
        private Seller seller;

        public Seller Seller
        {
            get { return seller; }
            set
            {
                seller = value;
                OnPropertyChanged("Seller");
            }
        }

        private Shop shop;

        public Shop Shop
        {
            get { return shop; }
            set
            {
                shop = value;
                OnPropertyChanged("Shop");
            }
        }


        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        public override object Copy()
        {
            Role role = new Role();
            role.Id = this.Id;
            role.Quantity = this.Quantity;

            return role;
        }
       

        public override void CopyFrom(object Order)
        {
            Role role = Order as Role;
            this.Id = role.Id;
            this.Quantity = role.Quantity;
        }

    }
}
