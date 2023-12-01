using System.Collections.Generic;

namespace Entities
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

        public enum RoleRight
        {
            basic,
            medium,
            admin
        }
        #endregion
    }
}