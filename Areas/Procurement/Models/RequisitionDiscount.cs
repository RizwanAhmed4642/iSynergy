using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Procurement.Models
{
    public class RequisitionDiscount
    {
        [Key]
        public int RequisitionDiscountId { get; set; }

        [Required(ErrorMessage = "Please provide the discount amount")]
        public decimal Amount { get; set; }
        public bool isCollectedByFinance { get; set; }

        [Required(ErrorMessage = "Please select the requisition for which this discount has been availed.")]
        [Display(Name = "Requisition")]
        public int RequisitionId { get; set; }


        [ForeignKey("RequisitionId")]
        public virtual Requisition Requistion { get; set; }

        public int ReceivedById { get; set; }
        public int OfficeId { get; set; }
        public int DepositedById { get; set; }

    }
}