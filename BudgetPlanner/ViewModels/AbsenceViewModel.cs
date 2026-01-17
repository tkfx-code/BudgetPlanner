using BudgetPlanner.Interfaces;
using BudgetPlanner.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace BudgetPlanner.ViewModels
{
    class AbsenceViewModel : ObservableObject
    {
        private readonly IBudgetRepo _budgetRepo;
        public RelayCommand SaveCommand { get; }
        private bool _isSickness = true;
        private DateTime _startDate = DateTime.Now;
        private DateTime _endDate = DateTime.Now;

        public AbsenceViewModel(IBudgetRepo budgetRepo)
        {
            _budgetRepo = budgetRepo;
            Date = DateTime.Now;

            SaveCommand = new RelayCommand(x => ExecuteAddAbsence(x));
        }

        private void ExecuteAddAbsence(object windowParam)
        {
            decimal calculatedAmount = CalculateAbsenceAmount();
            //place deducation for next month
            DateTime nextMonth = StartDate.AddMonths(1);
            DateTime paymentDate = new DateTime(nextMonth.Year, nextMonth.Month, 25);

            var newAbsence = new DatabaseTransaction
            {
                Amount = calculatedAmount,
                Date = paymentDate,
                Description = $"Absence: {StartDate.ToShortDateString()} - {EndDate.ToShortDateString()}",
                IsIncome = false,
                CategoryName = IsSickness ? "Sickness" : "VAB",
                IsLocked = true,
                Recurrence = TransactionRecurrence.OneTime
            };

            //Save to database
            _budgetRepo.AddTransaction(newAbsence);

            //Close pop up window 
            if (windowParam is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        //Amount
        private decimal _amount;
        public decimal Amount { get => _amount; set => SetProperty(ref _amount, value); }

        //Date
        private DateTime _date;
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        //isIncome
        private bool _isIncome;
        public bool IsIncome { get => _isIncome; set => SetProperty(ref _isIncome, value); }

        //Description
        private string _description;
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        //Categories
        public List<string> CategoryOptions { get; set; } = new List<string> {
            "Sickness", "VAB" };
        private string _selectedCategoryName;
        public string SelectedCategoryName { get => _selectedCategoryName; set => SetProperty(ref _selectedCategoryName, value); }

        //Recurrence
        private TransactionRecurrence _recurrence;

        //isLocked
        private bool _isLocked;
        public bool IsLocked { get => _isLocked; set => SetProperty(ref _isLocked, value); }

        public bool IsSickness
        {
            get => _isSickness;
            set { if (SetProperty(ref _isSickness, value)) OnPropertyChanged(nameof(IsVab)); }
        }
        public bool IsVab
        {
            get => !IsSickness;
            set { IsSickness = !value; }
        }

        private decimal CalculateAbsenceAmount()
        {
            decimal salary = Amount;
            int workDays = GetBusinessDays(StartDate, EndDate);
            if (workDays == 0) return 0;

            decimal annual = salary * 12;
            decimal weekly = annual / 52;
            decimal hourly = annual / (52 * 40);

            if (IsSickness)
            {
                //20% deduction for sickness
                decimal deductionPerDay = hourly * 8 * 0.2m;
                //qualDay = "karens"
                decimal qualDay = weekly * 0.2m;

                //deduction net
                return (workDays * (hourly * 8 * 0.2m)) + qualDay;
            } else
            {
                //VAB ceiling = 410k per year 
                decimal vabAnnual = Math.Min(annual, 410000);
                decimal vabHourly = vabAnnual / (52 * 40);

                //deducation net
                decimal dailyLoss = (hourly * 8) - (vabHourly * 8 * 0.8m);
                return workDays * dailyLoss;
            }
        }

        private int GetBusinessDays(DateTime start, DateTime end)
        {
            int count = 0;
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
