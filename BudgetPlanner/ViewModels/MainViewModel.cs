using BudgetPlanner.Interfaces;
using BudgetPlanner.Models;
using System.Collections.ObjectModel;
using BudgetPlanner.Repositories;
using System.Linq;
using System.Windows;

namespace BudgetPlanner.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IBudgetRepo _budgetRepo;
        public ObservableCollection<DatabaseTransaction> Transactions { get; set; }
        public ObservableCollection<string> PrognosisMonths { get; set; } = new ObservableCollection<string>
        {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        public RelayCommand AddTransactionCommand { get; }
        public RelayCommand AddAbsenceCommand { get; }
        public RelayCommand DeleteTransactionCommand { get; }
        public List<string> PrognosisYears { get; } = new List<string>();

        private string _selectedPrognosisMonth;
        private string _selectedPrognosisYear;

        public string SelectedPrognosisMonth
        {
            get => _selectedPrognosisMonth;
            set
            {
                _selectedPrognosisMonth = value;
                OnPropertyChanged(nameof(SelectedPrognosisMonth));
                CalculatePrognosis();
            }
        }
        public string SelectedPrognosisYear
        {
            get => _selectedPrognosisYear;
            set
            {
                _selectedPrognosisYear = value;
                OnPropertyChanged(nameof(SelectedPrognosisYear));
                CalculatePrognosis();
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
            AddAbsenceCommand = new RelayCommand(x => ExecuteOpenAbsenceWindow());
            DeleteTransactionCommand = new RelayCommand(x => ExecuteDeleteTransaction(x));
            ClearDateCommand = new RelayCommand(x => SelectedDate = null);

            SelectedPrognosisYear = DateTime.Now.Year.ToString();
            SelectedPrognosisMonth = PrognosisMonths[DateTime.Now.Month - 1];

            CalculatePrognosis();
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
        private void ExecuteOpenAbsenceWindow()
        {
            var absenceWindow = new BudgetPlanner.Views.AbsenceWindow();
            var absenceViewModel = new AbsenceViewModel(_budgetRepo);
            absenceWindow.DataContext = absenceViewModel;
            if (absenceWindow.ShowDialog() == true)
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
            CalculatePrognosis();
        }

        public async Task AddTransaction(object? parameter)
        {

        }

        private void ExecuteDeleteTransaction(object? parameter)
        {
            if (parameter is DatabaseTransaction transaction)
            {
                //Pop up for confirmation
                var result = MessageBox.Show(
                    $"Are you sure you want to delete this: {transaction.Description}?", 
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _budgetRepo.Deletetransaction(transaction.Id);
                    Transactions.Remove(transaction);
                    UpdateDashboard();
                    CalculatePrognosis();
                }
            }
        }

        public void SaveEditedTransaction(DatabaseTransaction transaction)
        {
            if (transaction!= null)
            {
                _budgetRepo.UpdateTransaction(transaction);

                UpdateDashboard();
                CalculatePrognosis();
            }
        }

        //Call when cell change is done in DataGrid
        public void OnTransactionEdited(DatabaseTransaction transaction)
        {
            if (transaction != null)
            {
                _budgetRepo.UpdateTransaction(transaction);
                UpdateDashboard();
                CalculatePrognosis();
            }
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
            //Added Sickness and VAB here to show up in filtration dropdown
            "All", "Fun", "Groceries", "Healthcare", "Income", "Insurance", "Rent", "Restaurant & Cafe", "Savings", "Services", "Sickness", "Transport", "VAB"
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

        private decimal _monthlyAbsenceImpact;
        public decimal MonthlyAbsenceImpact { get => _monthlyAbsenceImpact; set => SetProperty(ref _monthlyAbsenceImpact, value); }

        private decimal _yearlyAbsenceImpact;
        public decimal YearlyAbsenceImpact { get => _yearlyAbsenceImpact; set => SetProperty(ref _yearlyAbsenceImpact, value); }

        //Hide row for VAB/Sick if not applicable
        public bool HasMonthlyAbsence => _monthlyAbsenceImpact != 0;
        public bool HasYearlyAbsence => _yearlyAbsenceImpact != 0;

        //Prognosis Tab specifics
        private decimal _yearlyPrognosis;
        public string YearlyPrognosisDisplay => $"{_yearlyPrognosis:N2} kr";
        private decimal _monthlyPrognosis;
        public string MonthlyPrognosisDisplay => $"{_monthlyPrognosis:N2} kr";

        private void CalculatePrognosis()
        {
            if (Transactions == null || string.IsNullOrEmpty(SelectedPrognosisYear)) return;
            if (!int.TryParse(SelectedPrognosisYear, out int targetYear)) return;

            MonthlyAbsenceImpact = 0;
            YearlyAbsenceImpact = 0;
            decimal yearTotal = 0;
            decimal monthTotal = 0;

            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            //Find index of selected month where 1 is January
            int targetMonth = PrognosisMonths.IndexOf(SelectedPrognosisMonth) +1; 

            foreach (var t in Transactions)
            {
                string recurrenceText = t.Recurrence.ToString().ToLower().Trim();

                //Calculate yearly balance
                decimal annualAmount = 0;
                switch (recurrenceText)
                {
                    case "monthly":
                        annualAmount = t.Amount * 12;
                        break;
                    case "yearly":
                        annualAmount = t.Amount;
                        break;
                    default:
                        //OneTime transactions: Prognosis THIS year includes one time transactions
                        if (t.Date.Year == targetYear)
                            annualAmount = t.Amount;
                        break;

                }
                if (t.IsIncome) yearTotal += annualAmount; else yearTotal -= annualAmount;

                //Annual VAB/Sickness
                if(t.CategoryName == "Sickness" || t.CategoryName == "VAB")
                {
                    YearlyAbsenceImpact += annualAmount;
                }

                //Calculate monthly balance
                decimal monthlyAmount = 0;

                switch (recurrenceText)
                {
                    case "monthly":
                        monthlyAmount = t.Amount;
                        break;
                    //Only include if the yearly transaction happens this month
                    case "yearly":
                        if (t.Date.Month == targetMonth) monthlyAmount = t.Amount;
                        break;
                    default:
                        //OneTime transactions: Prognosis THIS year includes one time transactions
                        if (t.Date.Year == targetYear && t.Date.Month == targetMonth)
                        {
                            monthlyAmount = t.Amount;
                        }
                        break;
                }
                if (t.IsIncome) monthTotal += monthlyAmount; else monthTotal -= monthlyAmount;

                //Monthly VAB/Sickness
                if (t.CategoryName == "Sickness" || t.CategoryName == "VAB")
                {
                    MonthlyAbsenceImpact += monthlyAmount;
                }
            }

            _yearlyPrognosis = yearTotal;
            _monthlyPrognosis = monthTotal;

            OnPropertyChanged(nameof(YearlyPrognosisDisplay));
            OnPropertyChanged(nameof(MonthlyPrognosisDisplay));

            //Force update Absence as well to update UI
            OnPropertyChanged(nameof(HasYearlyAbsence));
            OnPropertyChanged(nameof(HasMonthlyAbsence));

            OnPropertyChanged(nameof(YearlyAbsenceImpact));
            OnPropertyChanged(nameof(MonthlyAbsenceImpact));
        }
    }
}
