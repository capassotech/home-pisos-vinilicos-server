using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace home_pisos_vinilicos.Domain.Entities
{
    [Table("SubCategory")]
    public class SubCategory
    {
        [Key]
        [Column(TypeName = "VARCHAR(50)")]
        public string? IdSubCategory { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Description { get; set; }
    }
}
