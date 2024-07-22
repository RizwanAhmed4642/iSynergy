using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Procurement.Models
{
    public class RequisitionBill
    {
        public int RequisitionBillId { get; set; }
        [Required(ErrorMessage = "Please upload bill(s) for requisition", AllowEmptyStrings = false)]
        public string File { get; set; }
        public int RequisitionId { get; set; }
        public virtual Requisition Requisition { get; set; }
    }
}