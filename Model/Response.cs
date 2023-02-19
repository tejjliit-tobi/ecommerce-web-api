using System.Collections.Generic;
namespace ecommerce_web_api.Model
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public List<Products> listproducts { get; set; }


    }
}
