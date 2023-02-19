using System.Collections.Generic;
namespace ecommerce_web_api.Model
{
    public class CartResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<CartProducts> listproducts { get; set; }

    }
}
