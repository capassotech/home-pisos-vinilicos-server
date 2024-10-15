using home_pisos_vinilicos.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace home_pisos_vinilicos_admin.Domain.Entities
{
    [Table("Colors")]
    public class Color
    {
        [Key]
        [Column(TypeName = "VARCHAR(100)")]
        public string IdColor { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Name { get; set; }

        [Required]
        [ForeignKey("IdCategory")]
        public string IdProduct { get; set; }
        public Product? Product { get; set; }
    }
}