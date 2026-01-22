using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockDamageSystem.Models
{
    public class Stock
    {
        [Key, Column(Order = 0)]
        public string GodownNo { get; set; }

        [Key, Column(Order = 1)]
        public string SubItemCode { get; set; }

        public decimal StockQty { get; set; }
    }
}
