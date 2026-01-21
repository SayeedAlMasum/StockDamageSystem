using System.ComponentModel.DataAnnotations;

namespace StockDamageSystem.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }
    }
}
