using System.ComponentModel.DataAnnotations;

namespace StockDamageSystem.Models
{
    public class Currency
    {
     
        public string CurrencyName { get; set; }

        public decimal ConversionRate { get; set; }
    }
}
