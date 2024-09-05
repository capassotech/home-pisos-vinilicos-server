namespace home_pisos_vinilicos.Domain.Entities
{
    public class Cart
    {
        public string IdCart { get; set; }
        public Order order { get; set; }
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
