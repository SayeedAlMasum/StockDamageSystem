using System.ComponentModel.DataAnnotations;

namespace StockDamageSystem.Models
{
    public class StockDamage
    {
        [Key]
        public int Id { get; set; }

        public string GodownNo { get; set; }

        public string SubItemCode { get; set; }

        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }

        public decimal AmountIn { get; set; }

        public string Currency { get; set; }

        public decimal ExchangeRate { get; set; }

        public string EmployeeName { get; set; }

        public string Comments { get; set; }

        public DateTime EntryDate { get; set; }
    }
}
