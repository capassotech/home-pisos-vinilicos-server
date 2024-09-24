using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace home_pisos_vinilicos.Domain.Entities
{
    [Table("Contacts")]
    public class Contact
    {
        [Key]
        [Column(TypeName = "VARCHAR(50)")]
        public string? IdContact { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal Phone { get; set; }


        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string GoogleMapsUrl { get; set; }


        [Required]
        [ForeignKey("IdSocialNetwork")]
        public string IdSocialNetwork { get; set; }
        
    }
}
