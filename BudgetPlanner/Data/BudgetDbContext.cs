using Microsoft.EntityFrameworkCore;
using BudgetPlanner.Models;

namespace BudgetPlanner.Data
{
    public class BudgetDbContext : DbContext
    {
        //My DBSets
        public DbSet<DatabaseTransaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=budgetplanner.db");
            
        }
        public BudgetDbContext()
        {
        }

        //Seed initial data for testing
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatabaseTransaction>().HasData(
                new DatabaseTransaction
                {
                    Id = 1,
                    Amount = 1000.00m,
                    Date = new DateTime(2025, 1, 1),
                    CategoryName = "Salary",
                    Description = "Monthly Salary",
                    IsIncome = true,
                    Recurrence = TransactionRecurrence.OneTime,
                }
            );
        }
    }
}
