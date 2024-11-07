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

        
        // ver si necesito el objeto con fk
        public List<Color> Colors { get; set; } = new List<Color>();


        [Required]
        [Column(TypeName = "INT")]
        public int Quantity { get; set; }


        [Column(TypeName = "BIT")]
        public bool IsFeatured { get; set; }


        [Column(TypeName = "Date")]
        public DateTime CreatedDate { get; set; }


        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public String Model { get; set; }


        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public String Dimensions { get; set; }


        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal SurfacePerBox { get; set; }


        [Column(TypeName = "BIT")]
        public bool RequiresUnderlay { get; set; }


        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal PricePerSquareMeter { get; set; }


        [Column(TypeName = "VARCHAR(50)")]
        public String TechnicalSheet { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();

        [Required]
        [ForeignKey("IdCategory")]
        public string IdCategory { get; set; }
        public Category? Category { get; set; }
    }
}