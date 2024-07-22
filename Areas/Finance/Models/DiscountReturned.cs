using iSynergy.Areas.Procurement.Models;
using System.ComponentModel.DataAnnotations.Schema;


namespace iSynergy.Areas.Finance.Models
{
    public class DiscountReturned : Journal
    {
        public int RequisitionDiscountId { get; set; }

        [ForeignKey("RequisitionDiscountId")]
        public RequisitionDiscount RequisitionDiscount { get; set; }
    }
}