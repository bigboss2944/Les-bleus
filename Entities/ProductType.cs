using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class ProductType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public ProductCharacteristics Characteristics { get; set; } = new ProductCharacteristics();

        [NotMapped]
        public float Size
        {
            get => Characteristics.Size;
            set => Characteristics.Size = value;
        }

        [NotMapped]
        public float Weight
        {
            get => Characteristics.Weight;
            set => Characteristics.Weight = value;
        }

        [NotMapped]
        public string? Color
        {
            get => Characteristics.Color;
            set => Characteristics.Color = value;
        }

        public string? Reference { get; set; }
        public float FreeTaxPrice { get; set; }
        public float Tax { get; set; }

        public override string ToString()
        {
            return Id + " " + Reference + " " + Color + " " + FreeTaxPrice + " " + Tax;
        }
    }
}
