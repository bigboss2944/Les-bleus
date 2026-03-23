namespace AspNet_FilRouge.Models
{
    public class Seller : User
    {
        #region Attributes
        private List<Order> orders = new();
        private Shop? shop;
        #endregion

        #region Properties
        public List<Order> Orders { get { return orders; } set { orders = value; } }
        public Shop? Shop { get { return shop; } set { shop = value; } }
        #endregion

        #region Constructors
        public Seller()
        {
            this.Orders = new List<Order>();
        }
        #endregion

        public override string ToString()
        {
            return " ";
        }
    }
}
