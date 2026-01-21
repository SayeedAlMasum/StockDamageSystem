using System.ComponentModel.DataAnnotations;

namespace StockDamageSystem.Models
{
    public class Godown
    {
        [Key]
        public int AutoSlNo { get; set; }

        public string GodownNo { get; set; }

        public string GodownName { get; set; }
    }
}
