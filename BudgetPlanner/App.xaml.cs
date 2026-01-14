using BudgetPlanner.Repositories;
using System.Configuration;
using System.Data;
using System.Windows;

namespace BudgetPlanner
{
    /// Uppgift
    /// User should be able to register earnings and expenses, categorized by type (e.g., food, rent, entertainment).
    /// and show summary of total earnings, total expenses, and net balance.
    /// It should be able to show monthly prognosis where the user kan see how next month will look like based on current month expenses and earnings.
 
    /// 2. Use databinding to connect UI to ViewModel
    /// 3. Possibility to add, edit and remove expensés and earnings
    /// - Every post should be in a category
    /// -- Expenses 
    /// -- Earnings
    /// 4. Registration of expense should support recurring expenses (e.g., monthly rent) and one time purchases
    /// 5. Show sumaries and monthly prognosis
    /// - Should include reoccuring expenses and earnings
    /// 6. Calculate monthly earning depending on yearly salary and worked hours
    /// 
    /// UI:
    /// Use styles, resources and data templates for a nice and consistent look and feel
    /// 
    /// Use MVVM structure before starting the implementation
    /// Test calculations separately before you connect them to UI
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
