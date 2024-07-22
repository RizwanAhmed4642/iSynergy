using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iSynergy.Areas.Finance.Models
{
    public class BalanceSheetViewModel
    {

        [Required(ErrorMessage = "Please select To Date")]
        [Display(Name = "As of")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime asOfDate { get; set; }
        public DateTime fromDate { get; set; }
        public int startingYear { get; set; }
        public int endingYear { get; set; }

        public int yearsSpan { get; set; }

        public List<YearlyBalanceViewModel> Assets { get; set; }
        public List<YearlyBalanceViewModel> Liabilities { get; set; }
        public List<YearlyBalanceViewModel> Equity { get; set; }

        public string printedBy { get; set; }
        public BalanceSheetViewModel()
        {
            asOfDate = DateTime.Today;
            Assets = new List<YearlyBalanceViewModel>();
            Liabilities = new List<YearlyBalanceViewModel>();
            Equity = new List<YearlyBalanceViewModel>();
        }

    }
}