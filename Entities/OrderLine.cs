using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class OrderLine
    {
        #region Attributs
        private long id;
        private long orderId;
        private Order? order;
        private long productTypeId;
        private ProductType? productType;
        private int quantity;
        #endregion

        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get { return id; } set { id = value; } }
        public long OrderId { get { return orderId; } set { orderId = value; } }
        public Order? Order { get { return order; } set { order = value; } }
        public long ProductTypeId { get { return productTypeId; } set { productTypeId = value; } }
        public ProductType? ProductType { get { return productType; } set { productType = value; } }
        public int Quantity { get { return quantity; } set { quantity = value; } }
        #endregion

        public OrderLine() { }

        public override string ToString()
        {
            return this.id + " " + this.orderId + " " + this.productTypeId + " " + this.quantity;
        }
    }
}
