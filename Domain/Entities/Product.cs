using home_pisos_vinilicos.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace home_pisos_vinilicos_admin.Domain
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column(TypeName = "VARCHAR(50)")]
        public string IdProduct { get; set; }


        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Name { get; set; }


        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public String Description { get; set; }


        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal Size { get; set; }


        
        [Column(TypeName = "VARCHAR(50)")]
        public String Color { get; set; }


        [Required]
        [Column(TypeName = "INT")]
        public int Quantity { get; set; }

        /*

        [Required]
        public int IdCategory { get; set; }
        [ForeignKey("IdCategory")]
        public Category category { get; set; }
        */
    }

}

