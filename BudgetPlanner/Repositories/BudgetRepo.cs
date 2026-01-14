using Microsoft.EntityFrameworkCore;
using BudgetPlanner.Data;
using BudgetPlanner.Models;
using BudgetPlanner.Interfaces;

namespace BudgetPlanner.Repositories
{
    public class BudgetRepo : IBudgetRepo
    {
        public readonly BudgetDbContext _context;
        public BudgetRepo()
        {
            _context = new BudgetDbContext();
        }

        public void AddTransaction(DatabaseTransaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        public IEnumerable<DatabaseTransaction> GetAllTransactions()
        {
            return _context.Transactions.ToList();
        }
    }
}
