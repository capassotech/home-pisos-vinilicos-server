using System.ComponentModel.DataAnnotations;

namespace home_pisos_vinilicos.Application.DTOs
{
    public class ProductDto
    {
        public string? IdProduct { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El precio es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El tamaño es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El tamaño debe ser mayor que cero.")]
        public decimal Size { get; set; }

        public string Color { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos uno.")]
        public int Quantity { get; set; }

        public bool IsFeatured { get; set; }

        [Required(ErrorMessage = "El modelo es requerido.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Las dimensiones son requeridas.")]
        public string Dimensions { get; set; }

        [Required(ErrorMessage = "La superficie por caja es requerida.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La superficie por caja debe ser mayor que cero.")]
        public decimal SurfacePerBox { get; set; }

        public bool RequiresUnderlay { get; set; }

        [Required(ErrorMessage = "El precio por m² es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio por m² debe ser mayor que cero.")]
        public decimal PricePerSquareMeter { get; set; }

        public string TechnicalSheet { get; set; }
    }
}
