
namespace home_pisos_vinilicos.Application.DTOs
{
    public class CategoryDto
    {
        public string? IdCategory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsFeatured { get; set; }
        public string? IdSubCategory { get; set; }
        public List<CategoryDto>? SubCategories { get; set; }
    }
}
