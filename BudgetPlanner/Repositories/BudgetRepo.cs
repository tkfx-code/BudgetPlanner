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
            _context.Database.EnsureCreated();
        }

        public void AddTransaction(DatabaseTransaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        public void Deletetransaction(int transactionId)
        {
            using (var db = new BudgetDbContext())
            {
                var transaction = db.Transactions.Find(transactionId);
                if (transaction != null)
                {
                    db.Transactions.Remove(transaction);
                    db.SaveChanges();
                }
            }
        }

        public IEnumerable<DatabaseTransaction> GetAllTransactions()
        {
            return _context.Transactions.ToList();
        }

        //Update an existing transaction and save changes to the database
        public void UpdateTransaction(DatabaseTransaction transaction)
        {
            using (var db = new BudgetDbContext())
            {
                db.Transactions.Update(transaction);
                db.SaveChanges();
            }
        }
    }
}
