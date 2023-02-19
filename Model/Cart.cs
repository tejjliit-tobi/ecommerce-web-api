namespace ecommerce_web_api.Model
{
    public class Cart
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string AddedBy { get; set; }
        public string DateAdded { get; set; }
    }
}
