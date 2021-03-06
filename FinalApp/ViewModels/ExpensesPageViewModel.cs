﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FinalApp.Models;
using FinalApp.Services;
using Microcharts;
using SkiaSharp;
using Xamarin.Forms;

namespace FinalApp.ViewModels {

    public class ExpensesGroupedList : List<UserExpense> {
        public string ExpensesCategoryId { get; set; }

        public ExpensesGroupedList() { }

        public ExpensesGroupedList(string expensesCategoryId) {
            ExpensesCategoryId = expensesCategoryId;
        }

        public ExpensesGroupedList(string expensesCategoryId, List<UserExpense> expenses) {
            ExpensesCategoryId = expensesCategoryId;
            this.Clear();
            this.AddRange(expenses);
        }
    }

    public class ExpensesPageViewModel : BindableObject {

        protected readonly BindableProperty ExpensesChartProperty =
            BindableProperty.Create(nameof(ExpensesChart), typeof(Chart), typeof(ExpensesPageViewModel), null);

        protected readonly BindableProperty UserExpensesProperty =
            BindableProperty.Create(nameof(UserExpenses), typeof(IEnumerable<UserExpense>), typeof(ExpensesPageViewModel), new List<UserExpense>());

        protected readonly BindableProperty GroupedUserExpensesProperty =
            BindableProperty.Create(nameof(GroupedUserExpenses), typeof(List<ExpensesGroupedList>), typeof(ExpensesPageViewModel), new List<ExpensesGroupedList>());

        protected readonly BindableProperty ExpensesBalanceProperty =
            BindableProperty.Create(nameof(ExpensesBalance), typeof(string), typeof(ExpensesPageViewModel), "-");

        protected readonly BindableProperty AverageExpenseProperty =
            BindableProperty.Create(nameof(AverageExpense), typeof(string), typeof(ExpensesPageViewModel), "-");

        protected readonly BindableProperty BusiestDayProperty =
            BindableProperty.Create(nameof(BusiestDay), typeof(string), typeof(ExpensesPageViewModel), "-");

        protected readonly BindableProperty DateRangeDescriptionProperty =
            BindableProperty.Create(nameof(DateRangeDescription), typeof(string), typeof(ExpensesPageViewModel), "This month");
            
        public Chart ExpensesChart {
            get => (Chart)GetValue(ExpensesChartProperty);
            set => SetValue(ExpensesChartProperty, value);
        }

        private SKColor[] chartColors = new SKColor[] {
            SKColors.Red,
            SKColors.Blue,
            SKColors.Green,
            SKColors.Yellow,
            SKColors.Orange,
            SKColors.Firebrick,
            SKColors.MidnightBlue,
            SKColors.Turquoise,
            SKColors.Purple,
            SKColors.CornflowerBlue,
            SKColors.PaleVioletRed,
            SKColors.LimeGreen,
            SKColors.DarkSeaGreen
        };

        public IEnumerable<UserExpense> UserExpenses {
            get => (IEnumerable<UserExpense>)GetValue(UserExpensesProperty);
            set => SetValue(UserExpensesProperty, value);
        }

        public List<ExpensesGroupedList> GroupedUserExpenses {
            get => (List<ExpensesGroupedList>)GetValue(GroupedUserExpensesProperty);
            set => SetValue(GroupedUserExpensesProperty, value);
        }

        public string ExpensesBalance {
            get => (string)GetValue(ExpensesBalanceProperty);
            set => SetValue(ExpensesBalanceProperty, value);
        }

        public string AverageExpense {
            get => (string)GetValue(AverageExpenseProperty);
            set => SetValue(AverageExpenseProperty, value);
        }

        public string BusiestDay {
            get => (string)GetValue(BusiestDayProperty);
            set => SetValue(BusiestDayProperty, value);
        }

        public string DateRangeDescription {
            get => (string)GetValue(DateRangeDescriptionProperty);
            set => SetValue(DateRangeDescriptionProperty, value);
        }

        private IUserDataRepository repository;
        private IEnumerable<Category> userCategories;
        private List<UserIncome> userIncomes;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ExpensesPageViewModel(IUserDataRepository repository) {
            this.repository = repository;
            Update();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);
            if (propertyName == UserExpensesProperty.PropertyName) {
                UpdateOverview();
                UpdateExpensesChart();
            }
        }

        private async void UpdateOverview() {

            // Balance
            double expensesSum = UserExpenses.Sum((exp) => exp.Amount);
            ExpensesBalance = string.Format("{0:C}", Math.Abs(expensesSum));

            // Average
            AverageExpense = string.Format("- {0:C}", expensesSum / (double)UserExpenses.Count());

            // Busiest day
            var groupedExpenses = UserExpenses.GroupBy((exp) => {
                DateTimeOffset date = exp.StartDate;
                if (exp.ExpireDate != default(DateTimeOffset)) {
                    date = exp.ExpireDate;
                }
                return date.DayOfWeek;
            });

            DayOfWeek busiestDay = DayOfWeek.Monday;
            double busiestDayTotal = double.MinValue;
            foreach (var group in groupedExpenses) {
                var totalInGroup = group.Sum((exp) => exp.Amount);
                if (totalInGroup > busiestDayTotal) {
                    busiestDayTotal = totalInGroup;
                    busiestDay = group.Key;
                }
            }

            BusiestDay = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(busiestDay);
        }

        // private async void UpdateOverviewOptimized() {
        //     double balance = (await repository.GetUserIncomes().Sum((inc) => inc.Amount)) - UserExpenses.Sum((exp) => exp.Amount);
        //     ExpensesBalance = string.Format("{0} {1:C}", balance == 0 ? "" : (balance > 0 ? "+" : "-"), balance);
        // }

        private void UpdateExpensesChart() {

            int colorIndex = 0;
            Dictionary<string, SKColor> assignedColors = new Dictionary<string, SKColor>();
            ExpensesChart = new PieChart {
                AnimationDuration = TimeSpan.FromMilliseconds(600.0),
                Entries = UserExpenses.GroupBy((arg) => arg.CategoryId).Select((arg) => {
                    SKColor color;
                    var categoryId = arg.First().CategoryId ?? "";
                    if (assignedColors.ContainsKey(categoryId)) {
                        color = assignedColors[categoryId];
                    } else {
                        color = chartColors[colorIndex % (chartColors.Length - 1)];
                        assignedColors[categoryId] = color;
                        colorIndex++;
                    }

                    return new ChartEntry((float) arg.First().Amount) {
                        Color = color,
                        TextColor = CategoryIdToSKColor(arg.First().CategoryId),
                        Label = CategoryIdToString(arg.First().CategoryId),
                        ValueLabel = arg.First().Amount.ToString("F1")
                    };
                }),
                LabelTextSize = 18.0f,
                BackgroundColor = SKColors.Transparent
            };

            GroupedUserExpenses = UserExpenses
                .GroupBy((expense) => expense.CategoryId)
                .Select((group) => new ExpensesGroupedList(group.Key, group.ToList()))
                .ToList();
        }

        private string CategoryIdToString(string id) {
            var category = userCategories.FirstOrDefault((arg) => arg.Id == id);
            if (category != null) {
                return category.DisplayName;
            }
            return "";
        }

        private SKColor CategoryIdToSKColor(string id) {
            switch (id) {
                case "1": return SKColors.Green;
                case "2": return SKColors.Blue;
                case "3": return SKColors.DeepPink;
                default: return SKColors.Red;
            }
        }

        public async Task Update() {
            StartDate = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, 0, 0, 0);
            EndDate = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, 23, 59, 59);
            await Update(StartDate, EndDate);
            DateRangeDescription = "This month";
        }

        public async Task Update(DateTime startDate, DateTime endDate) {
            StartDate = startDate;
            EndDate = endDate;

            userCategories = await repository.GetUserCategories();

            UserExpenses = await repository.GetUserExpenses(startDate, endDate);

            foreach (UserExpense expense in UserExpenses) {
                expense.UserCategory = userCategories.FirstOrDefault((category) => category.Id == expense.CategoryId);
            }
            UpdateOverview();
            UpdateExpensesChart();

            DateRangeDescription = string.Format("{0} - {1}", startDate.ToShortDateString(), endDate.ToShortDateString());
        }


    }
}
