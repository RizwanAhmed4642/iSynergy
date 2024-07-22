using iSynergy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Procurement.Models
{
    public class RequisitionViewModel
    {
        [Required(ErrorMessage = "Please provide requisitino title")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Title")]
        public string requisitionDescription { get; set; }
        [Required(ErrorMessage ="Atleast one requistion item must be provided.", AllowEmptyStrings = false)]
        [Display(Name = "Required Items")]
        public List<RequisitionItem> requisitionItems { get; set; }
        [Display(Name = "Quotations")]
        public List<RequisitionQuotation> requisitionQuotations { get; set; }

        [Display(Name = "Bill")]
        public RequisitionBill requisitionBill { get; set; }

        public int RequisitionId { get; set; }

    }
}