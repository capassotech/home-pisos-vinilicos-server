using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace home_pisos_vinilicos_admin.Domain.Entities
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

