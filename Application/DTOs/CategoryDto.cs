using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Application.DTOs
{
    public class CategoryDto
    {
        public string? IdCategory { get; set; }
        public string? IdSuperCategory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Category> SubCategories { get; set; }
    }
}
