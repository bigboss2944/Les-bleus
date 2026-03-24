using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class PhysicalProduct
    {
        #region Attributs
        private long id;
        private List<ProductType> productTypes = new();
        #endregion

        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get { return id; } set { id = value; } }
        public List<ProductType> ProductTypes { get { return productTypes; } set { productTypes = value; } }
        #endregion

        public PhysicalProduct()
        {
            this.ProductTypes = new List<ProductType>();
        }

        public override string ToString()
        {
            return this.id.ToString();
        }
    }
}
