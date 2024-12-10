using System.ComponentModel.DataAnnotations;

namespace home_pisos_vinilicos.Application.DTOs
{
    public class ProductDto
    {
        public string IdProduct { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El precio es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public string Size { get; set; }
        public string Cod_Art { get; set; }
        public string PriceType { get; set; }
        public bool IsFeatured { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public string IdCategory { get; set; }
        public CategoryDto? Category { get; set; }
    }
}
