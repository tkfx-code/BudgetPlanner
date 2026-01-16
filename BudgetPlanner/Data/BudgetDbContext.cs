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

            //Testing more explicit path
            //string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "budgetplanner.db");
            //optionsBuilder.UseSqlite($"Data Source={dbPath}");

        }
        public BudgetDbContext()
        {
        }

        //Earlier
        //Seed initial data for testing
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<DatabaseTransaction>().HasData(
        //        new DatabaseTransaction
        //        {
        //            Id = 1,
        //            Amount = 1000.00m,
        //            Date = new DateTime(2025, 1, 1),
        //            CategoryName = "Salary",
        //            Description = "Monthly Salary 2",
        //            IsIncome = true,
        //            Recurrence = TransactionRecurrence.OneTime,
        //        }
        //    );
        //}
    }
}
