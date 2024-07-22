using iSynergy.Areas.Procurement.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSynergy.Areas.Finance.Models
{
    public class RequisitionPayment : Journal
    {
        public int RequisitionId { get; set; }


        [ForeignKey("RequisitionId")]
        public Requisition Requisition { get; set; }
    }
}