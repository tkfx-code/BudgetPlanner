using BudgetPlanner.Models;

namespace BudgetPlanner.Interfaces
{
    public interface IBudgetRepo
    {
        void AddTransaction(DatabaseTransaction transaction);
        IEnumerable<DatabaseTransaction> GetAllTransactions();
    }
}
