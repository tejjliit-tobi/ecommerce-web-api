using ecommerce_web_api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Stripe;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace ecommerce_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ShopController(IConfiguration configuration)
            {
            _configuration = configuration;
            }

        [HttpGet]
        [Route("ProductList")]
        public async Task<Response> GetAllProducts()
        {
            var listproducts = new List<Products>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon")))
            {
                await connection.OpenAsync();
                using (var cmd = new SqlCommand("SELECT * FROM tblProducts;", connection))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var products = new Products();
                            products.Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            products.Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            products.BrandName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            products.Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            products.Image = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                            products.ActualPrice = reader.IsDBNull(5) ? 0m : reader.GetDecimal(5);
                            products.Quantity = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                            products.SKU = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                            listproducts.Add(products);

                        }
                    }
                }
            }

            var response = new Response();
            if (listproducts.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Data found";
                response.listproducts = listproducts;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Data not found";
                response.listproducts = null;
            }

            return response;
        }

        [HttpGet]
        [Route("ProductListCart")]
        public async Task<CartResponse> GetCartList(string userId)
        {
            var cartproducts = new List<CartProducts>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon").ToString()))
            {
                var query = "SELECT P.ID, P.Name, P.BrandName, P.Description, P.Image, P.ActualPrice, P.SKU, C.AddedBy, C.DateAdded, C.Quantity FROM Cart C INNER JOIN tblProducts P ON C.ProductId = P.ID WHERE C.AddedBy = @userId;";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var products = new CartProducts
                        {
                            Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            BrandName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            Image = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            ActualPrice = reader.IsDBNull(5) ? 0m : reader.GetDecimal(5),
                            SKU = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            AddedBy = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            DateAdded = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                            Quantity = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
                        };

                        cartproducts.Add(products);
                    }
                }
            }

            var response = new CartResponse();
            if (cartproducts.Any())
            {
                response.StatusCode = 200;
                response.StatusMessage = "Data found";
                response.listproducts = cartproducts;
            }
            else
            {
                response.StatusCode = 0;
                response.StatusMessage = "Data not found";
                response.listproducts = null;
            }

            return response;
        }

        [HttpGet]
        [Route("CartList")]
        public async Task<CartResponse> CartList()
        {
            var cartproducts = new List<CartProducts>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon").ToString()))
            {
                var query = "SELECT P.ID, P.Name, P.BrandName, P.Description, P.Image, P.ActualPrice, P.SKU, C.AddedBy, C.DateAdded, C.Quantity FROM Cart C INNER JOIN tblProducts P ON C.ProductId = P.ID";
                var command = new SqlCommand(query, connection);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var products = new CartProducts
                        {
                            Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            BrandName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            Image = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            ActualPrice = reader.IsDBNull(5) ? 0m : reader.GetDecimal(5),
                            SKU = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            AddedBy = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            DateAdded = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                            Quantity = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
                        };

                        cartproducts.Add(products);
                    }
                }
            }

            var response = new CartResponse();
            if (cartproducts.Any())
            {
                response.StatusCode = 200;
                response.StatusMessage = "Data found";
                response.listproducts = cartproducts;
            }
            else
            {
                response.StatusCode = 0;
                response.StatusMessage = "Data not found";
                response.listproducts = null;
            }

            return response;
        }


        [HttpPost]
        [Route("AddItem")]
        public async Task<Response> AddItem(Products products)
        {
            Response response = new Response();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon").ToString()))
            {
                string query = "INSERT INTO tblProducts (Name, BrandName, Description, Image, ActualPrice, Quantity, SKU) " +
                               "VALUES (@Name, @BrandName, @Description, @Image, @ActualPrice, @Quantity, @SKU)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", products.Name);
                    command.Parameters.AddWithValue("@BrandName", products.BrandName);
                    command.Parameters.AddWithValue("@Description", products.Description);
                    command.Parameters.AddWithValue("@Image", products.Image);
                    command.Parameters.AddWithValue("@ActualPrice", products.ActualPrice);
                    command.Parameters.AddWithValue("@Quantity", products.Quantity);
                    command.Parameters.AddWithValue("@SKU", products.SKU);

                    try
                    {
                        await connection.OpenAsync();
                        int result = await command.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            response.StatusCode = 200;
                            response.StatusMessage = "Item added to your cart";
                        }
                        else
                        {
                            response.StatusCode = 200;
                            response.StatusMessage = "No item added";
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = 500;
                        response.StatusMessage = "Error adding item to cart";
                        // log the error here
                    }
                }
            }

            return response;
        }



        [HttpDelete]
        [Route("DeleteItem")]
        public Response DeleteItem(int id)
        {
            Response response = new Response();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon").ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM tblProducts WHERE ID = @id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        connection.Open();
                        int i = cmd.ExecuteNonQuery();
                        connection.Close();
                        if (i > 0)
                        {
                            response.StatusCode = 200;
                            response.StatusMessage = "Item deleted";
                        }
                        else
                        {
                            response.StatusCode = 200;
                            response.StatusMessage = "No item deleted";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = ex.Message;
            }

            return response;
        }

        [HttpDelete]
        [Route("DeleteCartItem")]
        public Response DeleteCartItem(string date)
        {
            Response response = new Response();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon").ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Cart WHERE DateAdded = @date", connection))
                    {
                        cmd.Parameters.AddWithValue("@date", date);

                        connection.Open();
                        int i = cmd.ExecuteNonQuery();
                        connection.Close();
                        if (i > 0)
                        {
                            response.StatusCode = 200;
                            response.StatusMessage = "Item remove";
                        }
                        else
                        {
                            response.StatusCode = 200;
                            response.StatusMessage = "No item remove";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = ex.Message;
            }

            return response;
        }


        //better code optimization
        [HttpGet]
        [Route("GetItem")]
        public Response GetItemData(int id)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon").ToString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM tblProducts WHERE ID = @id", connection);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                List<Products> listproducts = new List<Products>();
                Response response = new Response();

                if (reader.Read())
                {
                    var products = new Products();
                    products.Id = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader["Id"]);
                    products.Name = reader.IsDBNull(1) ? string.Empty : Convert.ToString(reader["Name"]);
                    products.BrandName = reader.IsDBNull(2) ? string.Empty : Convert.ToString(reader["BrandName"]);
                    products.Description = reader.IsDBNull(3) ? string.Empty : Convert.ToString(reader["Description"]);
                    products.Image = reader.IsDBNull(4) ? string.Empty : Convert.ToString(reader["Image"]);
                    products.ActualPrice = reader.IsDBNull(5) ? 0m : Convert.ToDecimal(reader["ActualPrice"]);
                    products.SKU = reader.IsDBNull(6) ? string.Empty : Convert.ToString(reader["SKU"]);
                    products.Quantity = reader.IsDBNull(7) ? 0 : Convert.ToInt32(reader["Quantity"]);
                    listproducts.Add(products);


                    response.StatusCode = 200;
                    response.StatusMessage = "Data found";
                    response.listproducts = listproducts;
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Data not found";
                    response.listproducts = null;
                }

                return response;
            }
        }


        [HttpPost]
        [Route("AddProduct")]
        public Response AddProduct(Cart cart)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon").ToString()))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Cart (ProductId, Quantity, AddedBy, DateAdded) VALUES (@ProductId, @Quantity, @AddedBy, @DateAdded)", connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", cart.Id);
                cmd.Parameters.AddWithValue("@Quantity", cart.Quantity);
                cmd.Parameters.AddWithValue("@AddedBy", cart.AddedBy);
                cmd.Parameters.AddWithValue("@DateAdded", cart.DateAdded);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return new Response
                    {
                        StatusCode = 200,
                        StatusMessage = "Item added to your cart"
                    };
                }
                else
                {
                    return new Response
                    {
                        StatusCode = 200,
                        StatusMessage = "No item added"
                    };
                }
            }
        }

        //not optimized
        [HttpPost]
        [Route("UpdateQuantity")]
        public Response UpdateQuantity(UpdateQuantity updateQuantity)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon").ToString());
            Response response = new Response();
            if (updateQuantity.ProductId > 0)
            {


                string query = "UPDATE tblProducts SET Quantity=@quantity WHERE ProductId=@id";

              
                SqlCommand cmd = new SqlCommand(query, connection);

               
                cmd.Parameters.AddWithValue("@quantity", updateQuantity.Quantity);
                cmd.Parameters.AddWithValue("@id", updateQuantity.ProductId);


                connection.Open();
                int i = cmd.ExecuteNonQuery();
                connection.Close();
                if (i > 0)
                {
                    response.StatusCode = 200;
                }
                else
                {
                    response.StatusCode = 200;
                }

            }
            else
            {
                response.StatusCode = 200;
            }

            return response;
        }

    
        [HttpPut]
        [Route("UpdateItem")]
        public Response UpdateItem(Products products)
        {

            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ShoppingCon").ToString());
            Response response = new Response();
            if (products.Id > 0)
            {
              
                using (SqlCommand cmd = new SqlCommand("UPDATE tblProducts SET Name = @Name, BrandName = @BrandName, Description = @Description, ActualPrice = @ActualPrice, Quantity = @Quantity, SKU = @SKU WHERE ID = @Id", connection))
                {
                    cmd.Parameters.AddWithValue("@Name", products.Name);
                    cmd.Parameters.AddWithValue("@BrandName", products.BrandName);
                    cmd.Parameters.AddWithValue("@Description", products.Description);
                    cmd.Parameters.AddWithValue("@ActualPrice", products.ActualPrice);
                    cmd.Parameters.AddWithValue("@Quantity", products.Quantity);
                    cmd.Parameters.AddWithValue("@SKU", products.SKU);
                    cmd.Parameters.AddWithValue("@Id", products.Id);

                    connection.Open();
                    int i = cmd.ExecuteNonQuery();
                    connection.Close();
                    if (i > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Item updated";
                    }
                    else
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "No updated";
                    }
                }


                
            }
            else
            {
                response.StatusCode = 200;
                response.StatusMessage = "No item found";
            }

            return response;
        }


    }


}
