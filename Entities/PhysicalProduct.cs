using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class PhysicalProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ProductTypeId { get; set; }
        public ProductType? ProductType { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
