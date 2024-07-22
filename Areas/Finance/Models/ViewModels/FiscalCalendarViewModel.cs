using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iSynergy.Areas.Finance.Models
{
    public class FiscalCalendarViewModel
    {
        [Required(ErrorMessage = "How many accounting periods you want in a fiscal year?")]
        public int Periods { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }


        [Required(ErrorMessage = "When does your fiscal year start?")]
        [DataType(DataType.Date)]
        [Display(Name = "New Start Date")]
        public DateTime NewStartDate { get; set; }

        public List<FiscalPeriod> OpenPeriods { get; set; }

        public List<FiscalPeriod> FrozenPeriods { get; set; }

        public DateTime LatestFrozenOrClosedPeriodEndDate { get; set; }

        public FiscalCalendarViewModel()
        {
            Periods = 1;
            var year = DateTime.Today.Month > 7 ? DateTime.Today.Year : DateTime.Today.Year - 1;
            var month = 7;
            var day = 1;
            StartDate = new DateTime(year, month, day);
            NewStartDate = new DateTime(year, month, day);
        }

    }
}