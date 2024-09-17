using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace home_pisos_vinilicos.Domain.Entities
{
    [Table("Categorys")]
    public class Category
    {

        [Key]
        [Column(TypeName = "VARCHAR(50)")]
        public string? IdCategory { get; set; }

        [Required]
        public string? IdSuperCategory { get; set; }
        [ForeignKey("IdSuperCategory")]
        public Category Supercategory { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Description { get; set; }


        public List<Category> SubCategories { get; set; }
    }
}
