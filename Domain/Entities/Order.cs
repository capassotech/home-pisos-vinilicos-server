using home_pisos_vinilicos_admin.Domain;

namespace home_pisos_vinilicos.Domain.Entities
{
    public class Order
    {
        public string IdOrder { get; set; }
        public List<Product>Products { get; set; }
        public Status status { get; set; }
    }
}
