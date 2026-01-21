using System.ComponentModel.DataAnnotations;

namespace StockDamageSystem.Models
{
    public class Stock
    {
        public string SubItemCode { get; set; }

        public decimal StockQty { get; set; }
    }
}
