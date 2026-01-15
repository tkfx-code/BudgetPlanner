using BudgetPlanner.Models;

namespace BudgetPlanner.Interfaces
{
    public interface IBudgetRepo
    {
        void AddTransaction(DatabaseTransaction transaction);
        void Deletetransaction(int transactionId);
        void UpdateTransaction(DatabaseTransaction transaction);
        IEnumerable<DatabaseTransaction> GetAllTransactions();
    }
}
