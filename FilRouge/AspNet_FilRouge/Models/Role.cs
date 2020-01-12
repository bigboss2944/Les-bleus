using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet_FilRouge.Models
{
    public class Role
    {
        #region Attributs
        private long id;
        private string name;
        private List<Seller> sellers;

        #endregion

        #region Properties
        public List<Seller> Sellers
        {
            get { return sellers; }
            set { sellers = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public long Id
        {
            get { return id; }
            set { id = value; }
        }
        #endregion

        #region Constructors
        public Role()
        {
            this.Sellers = new List<Seller>();
        }
        #endregion
    }
}