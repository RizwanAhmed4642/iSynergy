using iSynergy.Areas.HR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Procurement.Models
{
    /// <summary>
    /// This class represents a single requistion request
    /// </summary>
    public class Requisition
    {
        [Required]
        [Display(Name = "Requisition ID")]
        public int RequisitionId { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public  DateTime creationDate { get; set; }
        [Required]
        [Display(Name = "Requisition")]
        public string Description { get; set; }
        [Display(Name = "Amount Requested")]
        public decimal TotalAmountRequested { get; set; }
        [Display(Name = "Amount Approved")]
        public decimal TotalAmountApproved { get; set; }
        [Display(Name = "Amount Issued")]
        public decimal TotalAmountIssued { get; set; }

        [Display(Name = "Purchase Amount")]
        public decimal PurchaseAmount { get; set; }

        [Display(Name = "Discount")]
        public decimal DiscountReturned { get; set; }
        [Required]
        [Display(Name = "Requested By")]
        public int EmployeeId { get; set; }
        [Display(Name = "Pending With")]
        public int pendingWith { get; set; }
        [Required]
        [Display(Name = "Status")]
        public RequisitionStates CurrentState { get; set; }
        [Display(Name = "Recurring")]
        public bool isScheduled { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}