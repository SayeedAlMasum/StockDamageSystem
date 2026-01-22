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
                    
                    // Start a transaction to ensure data consistency
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in stockDamages)
                            {
                                // 1. Save stock damage record
                                using (SqlCommand cmd = new SqlCommand("SP_StockDamage_Save", con, transaction))
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

                                // 2. Deduct stock from SPECIFIC WAREHOUSE
                                using (SqlCommand cmdStock = new SqlCommand(@"
                                    UPDATE Stock 
                                    SET StockQty = StockQty - @Quantity 
                                    WHERE SubItemCode = @SubItemCode AND GodownNo = @GodownNo", con, transaction))
                                {
                                    cmdStock.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    cmdStock.Parameters.AddWithValue("@SubItemCode", item.SubItemCode);
                                    cmdStock.Parameters.AddWithValue("@GodownNo", item.GodownNo);
                                    
                                    int rowsAffected = cmdStock.ExecuteNonQuery();
                                    
                                    // If no stock record exists for this warehouse+item, create one with negative quantity
                                    if (rowsAffected == 0)
                                    {
                                        using (SqlCommand cmdInsert = new SqlCommand(@"
                                            INSERT INTO Stock (GodownNo, SubItemCode, StockQty) 
                                            VALUES (@GodownNo, @SubItemCode, -@Quantity)", con, transaction))
                                        {
                                            cmdInsert.Parameters.AddWithValue("@GodownNo", item.GodownNo);
                                            cmdInsert.Parameters.AddWithValue("@SubItemCode", item.SubItemCode);
                                            cmdInsert.Parameters.AddWithValue("@Quantity", item.Quantity);
                                            cmdInsert.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            
                            // Commit transaction if all operations succeed
                            transaction.Commit();
                            return Ok(new { success = true, message = "Stock damage records saved and warehouse-specific stock deducted successfully" });
                        }
                        catch (Exception)
                        {
                            // Rollback transaction if any error occurs
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
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
