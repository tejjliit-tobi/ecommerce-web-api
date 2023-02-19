namespace ecommerce_web_api.Model
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal ActualPrice { get; set; }
        public int Quantity { get; set; }
        public string SKU { get; set; }

    }
}
