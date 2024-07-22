using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace iSynergy.Areas.Finance.Models
{
    public class IncomeStatementViewModel
    {
        [Required(ErrorMessage = "Please select To Date")]
        [Display(Name = "From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime fromDate { get; set; }


        [Required(ErrorMessage = "Please select To Date")]
        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime toDate { get; set; }
        public int startingYear { get; set; }
        public int endingYear { get; set; }
        public int getYearSpan {
            get {
                return endingYear - startingYear;
            }
        }

        public List<YearlyBalanceViewModel> Income { get; set; }
        public List<YearlyBalanceViewModel> Expense { get; set; }
        public List<YearlyBalanceViewModel> COGS { get; set; }
        public decimal getNetIncome(int year)
        {
            return (decimal)(from income in Income
                             where income.Year == year
                             select income.Balance).Sum();
        }
        public decimal getNetCOGS(int year)
        {
            return (decimal)(from cogs in COGS
                             where cogs.Year == year
                             select cogs.Balance).Sum();
        }
        public decimal getNetExpense(int year)
        {
            return (decimal)(from expense in Expense
                             where expense.Year == year
                             select expense.Balance).Sum();
        }
        public decimal getNetEarning(int year)
        {
            return getNetIncome(year) - getNetCOGS(year) - getNetExpense(year);
        }
        public decimal getGrossProfit(int year)
        {
            return getNetIncome(year) - getNetCOGS(year);
        }
        public string printedBy { get; set; }

        public IncomeStatementViewModel()
        {
            Income = new List<YearlyBalanceViewModel>();
            Expense = new List<YearlyBalanceViewModel>();
        }
        public IncomeStatementViewModel(
            DateTime fromDate,
            DateTime toDate,
            int startingYear, 
            int endingYear, 
            List<YearlyBalanceViewModel> income,
            List<YearlyBalanceViewModel> cogs,
            List<YearlyBalanceViewModel> expense
            )
        {
            this.fromDate = fromDate;
            this.toDate = toDate;
            this.startingYear = startingYear;
            this.endingYear = endingYear;
            this.Income = income;
            this.COGS = cogs;
            this.Expense = expense;
        }
    }
}