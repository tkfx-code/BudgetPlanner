using BudgetPlanner.Interfaces;
using BudgetPlanner.Models;
using System.Collections.ObjectModel;
using BudgetPlanner.Repositories;
using System.Linq;

namespace BudgetPlanner.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IBudgetRepo _budgetRepo;
        public ObservableCollection<DatabaseTransaction> Transactions { get; set; }
        public RelayCommand AddTransactionCommand { get; }
        public List<string> PrognosisYears { get; } = new List<string>();
        private string _selectedPrognosisYear;
        public string SelectedPrognosisYear
        {
            get => _selectedPrognosisYear;
            set
            {
                if(SetProperty(ref _selectedPrognosisYear, value))
                {
                    CalculatePrognosis();
                }
            }
        }

        public MainViewModel()
        {
            //Create repo
            _budgetRepo = new BudgetRepo();

            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < 5; i++)
            {
                PrognosisYears.Add((currentYear + i).ToString());
            }
            _selectedPrognosisYear = currentYear.ToString();

            Transactions = new ObservableCollection<DatabaseTransaction>();
            AddTransactionCommand = new RelayCommand(x => ExecuteOpenAddWindow());
            ClearDateCommand = new RelayCommand(x => SelectedDate = null);
        }

        private void ExecuteOpenAddWindow()
        { 
            var addWindow = new BudgetPlanner.Views.AddTransactionWindow();
            var addViewModel = new AddTransactionViewModel(_budgetRepo);
            addWindow.DataContext = addViewModel;
            if (addWindow.ShowDialog() == true)
            {
              // Refresh transactions after adding a new one
              _ = LoadAsync();
            }
        }

        public async Task LoadAsync()
        {
            //Update Transactions collection asynchronously
            var transactions = await Task.Run(() => _budgetRepo.GetAllTransactions());

            Transactions.Clear();
            foreach (var t in transactions)
            {
                Transactions.Add(t);
            }

            UpdateDashboard();
        }

        public async Task AddTransaction(object? parameter)
        {

        }

        private string _filterText;
        private decimal _totalBalance;
        private ObservableCollection<DatabaseTransaction> _filteredTransactions;

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (SetProperty(ref _filterText, value))
                {
                    UpdateDashboard();
                }
            }
        }

        public ObservableCollection<DatabaseTransaction> FilteredTransactions
        {
            get => _filteredTransactions;
            set => SetProperty(ref _filteredTransactions, value);
        }

        public decimal TotalBalance
        {
            get => _totalBalance;
            set
            { if (SetProperty(ref _totalBalance, value))
                {
                    OnPropertyChanged(nameof(BalanceDisplay));
                    OnPropertyChanged(nameof(IsPositiveBalance));
                }
            }
        }

        //Filter by date
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    UpdateDashboard();
                }
            }
        }
        //Clear date
        public RelayCommand ClearDateCommand { get; }


        //Display properties for positive BalanceDisplay
        public string BalanceDisplay => TotalBalance > 0 ? $"+{TotalBalance:N2} kr" : $"{TotalBalance:N2} kr";
        public bool IsPositiveBalance => TotalBalance >= 0;

        //Filter and calculate totals
        private void UpdateDashboard()
        {
            ApplyFilter();
            CalculateTotals();
        }

        //Apply filter to Transactions
        public List<string> CategoryFilters { get; } = new List<string>
        {
            "All", "Fun", "Groceries", "Healthcare", "Income", "Insurance", "Rent", "Restaurant & Cafe", "Savings", "Services", "Transport"
        };
        private string _selectedCategoryFilter = "All";
        public string SelectedCategoryFilter
        {
            get => _selectedCategoryFilter;
            set
            {
                if(SetProperty(ref _selectedCategoryFilter, value))
                {
                    UpdateDashboard();
                }
            }
        }

        private void ApplyFilter()
        {
            if (Transactions == null) return;
            var filtered = Transactions.AsEnumerable();

            //"All" Shows everything
            if (SelectedCategoryFilter != "All")
            {
                filtered = filtered.Where(t => t.CategoryName == SelectedCategoryFilter);
            }
            if (SelectedDate.HasValue)
            {
                filtered = filtered.Where(t => t.Date.Date == SelectedDate.Value.Date);
            }
            FilteredTransactions = new ObservableCollection<DatabaseTransaction>(filtered);
        }

        private void CalculateTotals()
        {
            if (FilteredTransactions == null)
            {
                TotalBalance = 0;
                return;
            }

            //Calculate whats filtered in the list
            TotalBalance = FilteredTransactions.Sum(t => t.IsIncome ? t.Amount : -t.Amount);
        }

        //Prognosis Tab specifics
        private decimal _yearlyPrognosis;
        public string YearlyPrognosisDisplay => $"{_yearlyPrognosis:N2} kr";

        private void CalculatePrognosis()
        {
            if (Transactions == null) return;

            decimal totalYearlyBalance = 0;
            if (!int.TryParse(SelectedPrognosisYear, out int targetYear)) return;

            foreach (var t in Transactions)
            {
                decimal annualAmount = 0;

                string recurrenceText = t.Recurrence.ToString().ToLower().Trim();

                //Map string DB to times per year
                switch(recurrenceText)
                {
                    case "monthly":
                        annualAmount = t.Amount * 12;
                        break;
                    case "yearly":
                        annualAmount = t.Amount;
                        break;
                    default:
                        //OneTime transactions: Did it happen this year
                        if (t.Date.Year == targetYear)
                            annualAmount = t.Amount;
                        break;

                }
                if (t.IsIncome) totalYearlyBalance += annualAmount;
                else totalYearlyBalance -= annualAmount;
            }
            _yearlyPrognosis = totalYearlyBalance;
            OnPropertyChanged(nameof(YearlyPrognosisDisplay));
            OnPropertyChanged(nameof(IsPositiveBalance));
        }
    }
}
