using System.Windows;
using BudgetPlanner.Interfaces;
using BudgetPlanner.Models;

namespace BudgetPlanner.ViewModels
{
    public class AddTransactionViewModel : ObservableObject
    {
        private readonly IBudgetRepo _budgetRepo;
        public RelayCommand SaveCommand { get; }

        public AddTransactionViewModel(IBudgetRepo budgetRepo)
        {
            _budgetRepo = budgetRepo;
            Date = DateTime.Now;

            SaveCommand = new RelayCommand(x => ExecuteAddTransaction(x));
        }

        //Amount
        private decimal _amount;
        public decimal Amount { get => _amount; set => SetProperty(ref _amount, value); }

        //Date
        private DateTime _date;
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }

        //isIncome
        private bool _isIncome;
        public bool IsIncome { get => _isIncome; set => SetProperty(ref _isIncome, value);}

        //Description
        private string _description;
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        //Categories
        public List<string> CategoryOptions { get; set; } = new List<string> {
            "Fun", "Groceries", "Healthcare", "Income", "Insurance", "Rent",
            "Restaurant & Café", "Savings", "Services", "Transport" };
        private string _selectedCategoryName;
        public string SelectedCategoryName { get => _selectedCategoryName; set => SetProperty(ref _selectedCategoryName, value); }

        //Recurrence
        private TransactionRecurrence _recurrence;
        public TransactionRecurrence Recurrence { get => _recurrence; set { if (SetProperty(ref _recurrence, value)) OnPropertyChanged(nameof(IsYearlyEnabled));}}

        //Check if Frequency is enabled
        public bool IsYearlyEnabled => Recurrence == TransactionRecurrence.Yearly;

        //Radio button binding
        public bool IsOneTime { get => Recurrence == TransactionRecurrence.OneTime; set { if (value) Recurrence = TransactionRecurrence.OneTime;}}
        public bool IsMonthly { get => Recurrence == TransactionRecurrence.Monthly; set { if (value) Recurrence = TransactionRecurrence.Monthly;}}
        public bool IsYearly { get => Recurrence == TransactionRecurrence.Yearly; set {if (value) Recurrence = TransactionRecurrence.Yearly;}}

        //Month names for dropdown
        public List<string> MonthNames { get; } = new List<string> {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"};
        private string _selectedMonth;
        public string SelectedMonth { get => _selectedMonth; set => SetProperty(ref _selectedMonth, value); }

        //Execute method
        public void ExecuteAddTransaction(object windowParam)
        {
            var newTransaction = new DatabaseTransaction
            {
                Amount = this.Amount,
                Date = this.Date,
                Description = this.Description ?? "",
                IsIncome = this.IsIncome,
                Recurrence = this.Recurrence,
                CategoryName = this.SelectedCategoryName ?? "Other",
                Month = this.Recurrence == TransactionRecurrence.Yearly ? Date.Month : null
            };

            //Save to database
            _budgetRepo.AddTransaction(newTransaction);

            //Close pop up window 
            if (windowParam is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }
    }
}
