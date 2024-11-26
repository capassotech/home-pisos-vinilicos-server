using home_pisos_vinilicos.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace home_pisos_vinilicos_admin.Domain.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column(TypeName = "VARCHAR(100)")]
        public string IdProduct { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public String Description { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Size { get; set; }

        [Column(TypeName = "BIT")]
        public bool IsFeatured { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public List<string> ImageUrls { get; set; } = new List<string>();

        [Column(TypeName = "VARCHAR(100)")]
        public string Cod_Art { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string PriceType { get; set; }

        [Required]
        [ForeignKey("IdCategory")]
        public string IdCategory { get; set; }
        public Category? Category { get; set; }
    }
}