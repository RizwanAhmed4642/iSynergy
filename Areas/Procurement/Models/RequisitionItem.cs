using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Procurement.Models
{
    /// <summary>
    /// this class represents a single requisition item that
    /// is included in a requistion request
    /// </summary>
    public class RequisitionItem
    {
        [Required]
        public int RequisitionItemId { get; set; }
        [Required(ErrorMessage = "Please provide item details", AllowEmptyStrings = false)]
        [Display(Name = "Item")]
        public string ItemName { get; set; }
        [Required(ErrorMessage = "Please provide item quantity", AllowEmptyStrings = false)]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal? UnitPrice { get; set; }
        [Display(Name = "Cheque/Receipt/Voucher")]
        [DataType(DataType.Upload)]
        public string File { get; set; }
        public int RequisitionId { get; set; }
        public virtual Requisition Requisition { get; set; }

    }
}