using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetPlanner.Models
{
    [Table("Transactions")]
    public class DatabaseTransaction
    {
        [Key]
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public required string CategoryName { get; set; }
        public string Description { get; set; }
        public bool IsIncome { get; set; }
        public TransactionRecurrence Recurrence { get; set; }
        public int? Month { get; set; }
    }

    public enum TransactionRecurrence
    {
        OneTime,
        Monthly,
        Yearly
    }
}
