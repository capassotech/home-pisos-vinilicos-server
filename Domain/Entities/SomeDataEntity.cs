using System.ComponentModel.DataAnnotations;

namespace home_pisos_vinilicos.Domain.Models
{
    public class SomeDataEntity
    {
        [Key]
        public string Id { get; set; }
        public string DataField { get; set; }
    }
}
