using iSynergy.Areas.Finance.Models;
using iSynergy.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Finance.Models
{
    public class GeneralLedgerInquiryViewModel
    {

        [Display(Name = "From Account")]
        [RegularExpression(@"((^|, )([0-9]{2}|[0-9]{2}-[0-9]{2}|[0-9]{2}-[0-9]{2}-[0-9]{2}|[0-9]{2}-[0-9]{2}-[0-9]{2}-[0-9]{4}))+$", ErrorMessage = "Invalid code format. Correct format is 00 or 00-00 or 00-00-00 or 00-00-00-0000")]
        public string fromAccountId { get; set; }


        [Display(Name = "To Account")]
        [RegularExpression(@"((^|, )([0-9]{2}|[0-9]{2}-[0-9]{2}|[0-9]{2}-[0-9]{2}-[0-9]{2}|[0-9]{2}-[0-9]{2}-[0-9]{2}-[0-9]{4}))+$", ErrorMessage = "Invalid code format. Correct format is 00 or 00-00 or 00-00-00 or 00-00-00-0000")]
        public string toAccountId { get; set; }
        public string printedBy { get; set; }

        [Required(ErrorMessage = "Please select From Date")]
        [Display(Name = "From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }
        [Required(ErrorMessage = "Please select To Date")]
        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public List<LedgerViewModel> Ledgers { get; set; }

        public string fiscalYear { get; set; }

        public GeneralLedgerInquiryViewModel()
        {
            var fiscalCalendar = Shared.FinanceOperations.getFiscalCalendarModal();
            Ledgers = new List<LedgerViewModel>();
            FromDate = fiscalCalendar.StartDate;
            ToDate = DateTime.Today;
            var startDateYear = fiscalCalendar.StartDate.Year;
            var endDateYear = fiscalCalendar.StartDate.AddMonths(12).Year;
            fiscalYear = string.Format("{0} - {1}", startDateYear, endDateYear);

        }
        
    }
}