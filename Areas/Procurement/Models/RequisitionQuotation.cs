using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Procurement.Models
{
    public class RequisitionQuotation
    {
        [Required]
        public int RequisitionQuotationId { get; set; }
        [Required(ErrorMessage = "Please provide supplier name", AllowEmptyStrings = false)]
        [Display(Name = "Supplier")]
        public virtual string SupplierName { get; set; }
        [Required(ErrorMessage = "Please select a scaned quotation", AllowEmptyStrings = false)]
        [Display(Name = "Quotation")]
        [DataType(DataType.Upload)]
        public virtual string File { get; set; }
        [Required(ErrorMessage = "Please provide quotation price", AllowEmptyStrings = false)]
        [Display(Name = "Quoted Price")]
        public virtual decimal QuotationPrice { get; set; }
        public int RequisitionId { get; set; }
        public virtual Requisition Requisition { get; set; }
    }
}