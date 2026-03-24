using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class ProductType
    {
        #region Attributs
        private long id;
        private float size;
        private float weight;
        private string? color;
        private string? reference;
        private float freeTaxPrice;
        private float tax;
        #endregion

        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get { return id; } set { id = value; } }
        public float Size { get { return size; } set { size = value; } }
        public float Weight { get { return weight; } set { weight = value; } }
        public string? Color { get { return color; } set { color = value; } }
        public string? Reference { get { return reference; } set { reference = value; } }
        public float FreeTaxPrice { get { return freeTaxPrice; } set { freeTaxPrice = value; } }
        public float Tax { get { return tax; } set { tax = value; } }
        #endregion

        public ProductType() { }

        public override string ToString()
        {
            return this.id + " " + this.reference + " " + this.color + " " + this.freeTaxPrice + " " + this.tax;
        }
    }
}
