using BudgetPlanner.Repositories;
using System.Configuration;
using System.Data;
using System.Windows;

namespace BudgetPlanner
{
    /// Uppgift
    /// and show summary of total earnings, total expenses, and net balance.
    /// It should be able to show monthly prognosis where the user kan see how next month will look like based on current month expenses and earnings.
    /// EDIT AND REMOVE EXPENSES
    /// CALCULATE MONTHLY EARNINGS BASED ON YEARLY SALARY AND WORKED HOURS
    /// 
    /// UI:
    /// IMPLEMENT STYLES, RESOURCES AND DATA TEMPLATES
    public partial class App : Application
    {

        //QUICK AND DIRTY DB TESTING
        //public App()
        //{
        //    var repo = new BudgetRepository();
        //    repo.AddTransaction(new Models.DatabaseTransaction
        //    {
        //        Amount = 1000,
        //        CategoryName = new Models.Category { CategoryName = "Salary" },
        //        Date = DateTime.Now,
        //        Description = "Monthly salary",
        //        IsIncome = true,
        //        Recurrence = Models.TransactionRecurrence.Monthly
        //    });

        //    var list = repo.GetAllTransactions();
        //    var count = list.Count();
    }
}
