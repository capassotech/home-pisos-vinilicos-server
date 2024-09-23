using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace home_pisos_vinilicos.Domain.Entities
{
    public class FAQ
    {
        [Key]
        [Column(TypeName = "VARCHAR(50)")]
        public string? IdFAQ { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Question { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Answer { get; set; }
    }
}
