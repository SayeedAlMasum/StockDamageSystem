using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;

[Route("Api")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly IConfiguration _config;

    public ApiController(IConfiguration config)
    {
        _config = config;
    }

    private SqlConnection GetConnection()
    {
        return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    }

    [HttpGet("GetGodown")]
    public IActionResult GetGodown()
    {
        var list = new List<object>();
        using var con = GetConnection();
        con.Open();
        
        using var cmd = new SqlCommand("SELECT GodownNo, GodownName FROM Godown", con);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            list.Add(new
            {
                godownNo = reader["GodownNo"].ToString(),
                godownName = reader["GodownName"].ToString()
            });
        }

        return Ok(list);
    }

    [HttpGet("GetItems")]
    public IActionResult GetItems()
    {
        var list = new List<object>();
        using var con = GetConnection();
        con.Open();
        
        using var cmd = new SqlCommand("SELECT SubItemCode, SubItemName FROM SubItemCode", con);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            list.Add(new
            {
                subItemCode = reader["SubItemCode"].ToString(),
                subItemName = reader["SubItemName"].ToString()
            });
        }

        return Ok(list);
    }

    [HttpGet("GetItemDetails")]
    public IActionResult GetItemDetails(string code, string godownNo = null)
    {
        using var con = GetConnection();
        con.Open();

        if (!string.IsNullOrEmpty(godownNo))
        {
            SqlCommand cmd = new SqlCommand(@"
                SELECT s.SubItemCode, s.SubItemName, s.Unit, ISNULL(st.StockQty, 0) as StockQty
                FROM SubItemCode s
                LEFT JOIN Stock st ON s.SubItemCode = st.SubItemCode AND st.GodownNo = @godownNo
                WHERE s.SubItemCode = @code", con);

            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@godownNo", godownNo);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                return Ok(new
                {
                    subItemCode = dr["SubItemCode"].ToString(),
                    unit = dr["Unit"].ToString(),
                    stock = dr["StockQty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["StockQty"])
                });
            }
        }
        else
        {
            SqlCommand cmd = new SqlCommand(@"
                SELECT SubItemCode, SubItemName, Unit
                FROM SubItemCode
                WHERE SubItemCode = @code", con);

            cmd.Parameters.AddWithValue("@code", code);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                return Ok(new
                {
                    subItemCode = dr["SubItemCode"].ToString(),
                    unit = dr["Unit"].ToString(),
                    stock = 0
                });
            }
        }

        return NotFound();
    }

    [HttpGet("GetCurrency")]
    public IActionResult GetCurrency()
    {
        var list = new List<object>();
        using var con = GetConnection();
        con.Open();
        
        using var cmd = new SqlCommand("SELECT CurrencyName, ConversionRate FROM Currency", con);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            list.Add(new
            {
                currencyName = reader["CurrencyName"].ToString(),
                conversionRate = reader["ConversionRate"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ConversionRate"])
            });
        }

        return Ok(list);
    }

    [HttpGet("GetEmployee")]
    public IActionResult GetEmployee()
    {
        var list = new List<object>();
        using var con = GetConnection();
        con.Open();
        
        using var cmd = new SqlCommand("SELECT EmployeeName FROM Employee", con);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            list.Add(new
            {
                employeeName = reader["EmployeeName"].ToString()
            });
        }

        return Ok(list);
    }
}
