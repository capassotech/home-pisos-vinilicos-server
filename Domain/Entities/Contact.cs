namespace home_pisos_vinilicos.Domain.Entities
{
    public class Contact
    {
        public string IdContact { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public decimal Phone { get; set; }
        public List<SocialNetwork> RRSS { get; set; }
    }
}
