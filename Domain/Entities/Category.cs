namespace home_pisos_vinilicos.Domain.Entities
{
    public class Category
    {
        public string IdCategory { get; set; }
        public string? IdSuperCategory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Category> SubCategories { get; set; }
    }
}
