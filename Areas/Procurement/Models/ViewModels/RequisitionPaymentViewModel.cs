using iSynergy.Areas.Finance.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iSynergy.Areas.Procurement.Models
{
    public class RequisitionPaymentViewModel
    {
        public string userAction { get; set; }
        public List<RequisitionItem> requisitionItems { get; set; }
        public List<RequisitionQuotation> requisitionQuotations { get; set; }
        public List<RequisitionEvent> requisitionEvents { get; set; }
        [Display(Name = "Payment Mode")]
        public PaymentMode paymentMode { get; set; } // cash or bank
        public IEnumerable<SelectListItem> paymentModes { get; set; }
        [Display(Name = "Bank Account")]
        public IEnumerable<BankAccountViewModel> bankAccounts { get; set; } // this will be converted to a select list in view

        [Display(Name = "Bank Account")]
        [Required(ErrorMessage = "Please select a bank account")]
        public int bankAccountId { get; set; } // this is for posting the bank id back to controller.

        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        [Required(ErrorMessage = "Please provide notes to be displayed in financial reports.")]
        public string notes { get; set; }
        public RequisitionPayment requisitionPayment { get; set; }


        [Display(Name = "Attachment")]
        [Required(ErrorMessage = "Please upload attachment")]
        public List<HttpPostedFileBase> Files { get; set; }

        public RequisitionPaymentViewModel(int RequisitionId)
        {
            requisitionPayment = new RequisitionPayment();
            requisitionPayment.RequisitionId = RequisitionId;
            Files = new List<HttpPostedFileBase>();
        }
        public RequisitionPaymentViewModel() //MVC can call this when binding view back to controller
        {
            requisitionPayment = new RequisitionPayment();
            Files = new List<HttpPostedFileBase>();

        }
    }

    public enum PaymentMode
    {
        Cash,
        Bank
    }
}