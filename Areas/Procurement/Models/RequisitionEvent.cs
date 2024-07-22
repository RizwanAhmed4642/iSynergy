using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Procurement.Models
{
    public enum RequisitionStates {
        Requested, // when requisition created
        Rejected, // and sent back to requester
        ManagerApproved, // forwarded to next in-line manager
        FinalApproved, // when payment advice sent to finance
        CashIssued, // when cash issued to procurement or admin manager
        PartialCashIssued, // when paid in installments (used only in case of payroll for now)
        PartialCashReceived, // when Received in installments (used only in case of payroll for now)
        CashReceived, // when procurement or admin manager acknowledges the cash Received
        PartialDelivered, // when delivered some of the items or quanties (used only in case of payroll for now)
        Delivered, // when requisition item(s) purchased and delivered to requester
        Closed, // when requeser acnkowledges the delivery of requested items
        PendingForCoo,
        QuotationsNeeded,
        QuotationsAttached,
        Reapproval
    };
    public class RequisitionEvent
    {
        [Required]
        public int RequisitionEventId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Required]
        [Display(Name = "State")]
        public RequisitionStates EventState { get; set; }
        
        [Display(Name = "Responded By")]
        public int RespondedBy { get; set; } // employee id of the person who responded

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EventDate { get; set; }

        public int RequisitionId { get; set; }
        [Display(Name = "Pending With")]
        public int PendingWith { get; set; } // employee id of the person who will respond next

        public virtual Requisition Requisition { get; set; }
    }
}