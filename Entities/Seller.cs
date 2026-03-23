namespace Entities
{
    public class Seller : User
    {
        #region Attributes
        private List<Order> orders = new();
        private Shop? shop;
        private Role? role;
        #endregion

        #region Properties
        public Role? Role { get { return role; } set { role = value; } }
        public List<Order> Orders { get { return orders; } set { orders = value; } }
        public Shop? Shop { get { return shop; } set { shop = value; } }
        #endregion

        public Seller()
        {
            this.Orders = new List<Order>();
        }

        public override string ToString()
        {
            return " ";
        }
    }
}
