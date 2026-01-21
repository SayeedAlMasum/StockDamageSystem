using System.ComponentModel.DataAnnotations;

namespace StockDamageSystem.Models
{
    public class SubItemCode
    {
        [Key]
        public int AutoSlNo { get; set; }

        public string Code { get; set; }

        public string SubItemName { get; set; }

        public string Unit { get; set; }

        public decimal Weight { get; set; }
    }
}
