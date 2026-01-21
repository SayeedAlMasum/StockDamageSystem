using Microsoft.AspNetCore.Mvc;
using StockDamageSystem.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace StockDamageSystem.Controllers
{
    public class StockDamageController : Controller
    {
        private readonly IConfiguration _configuration;

        public StockDamageController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save([FromBody] List<StockDamage> stockDamages)
        {
            if (stockDamages == null || !stockDamages.Any())
            {
                return BadRequest("No data provided");
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    foreach (var item in stockDamages)
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_StockDamage_Save", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@GodownNo", item.GodownNo ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SubItemCode", item.SubItemCode ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            cmd.Parameters.AddWithValue("@Rate", item.Rate);
                            cmd.Parameters.AddWithValue("@AmountIn", item.AmountIn);
                            cmd.Parameters.AddWithValue("@Currency", item.Currency ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ExchangeRate", item.ExchangeRate);
                            cmd.Parameters.AddWithValue("@EmployeeName", item.EmployeeName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Comments", item.Comments ?? (object)DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return Ok(new { success = true, message = "Stock damage records saved successfully" });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { success = false, message = "Database error: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
