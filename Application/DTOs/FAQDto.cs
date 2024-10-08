using System.ComponentModel.DataAnnotations;

namespace home_pisos_vinilicos.Application.DTOs
{
    public class FAQDto
    {
        public string? IdFAQ { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
    }
}
