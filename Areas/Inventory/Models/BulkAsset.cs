using iSynergy.Areas.HR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Inventory.Models
{
    /// <summary>
    /// This class represents inventory items that are bought in bulk quantity
    /// and have a short shelf life. For example bulbs, power accessories, 
    /// kitchen and cleaning utencils etc...
    /// bulk assets normally do not have a serial number and this is what 
    /// differenitates them from hardware assets and any other types of assets.
    /// These items will be consumed after a time and should be removed from inventory.
    /// </summary>
    public class BulkAsset : Asset
    {
        [Required(ErrorMessage = "Please provide stock quantity")]
        [Display(Name = "Stock Quantity")]
        public int Quantity { get; set; }
        [Display(Name = "Stock Re-Order Level")]
        public int? ReOrderLevel { get; set; }
        public void markConsumed(DateTime MarkingDate, Employee MarkedBy, string Comments)
        {
            this.markDisposed(MarkingDate, MarkedBy, Comments);
            this.DisposalReason = (int)DisposalReasons.Consumed;
        }
        public void markExpired(DateTime MarkingDate, Employee MarkedBy, string Comments)
        {
            this.markDisposed(MarkingDate, MarkedBy, Comments);
            this.DisposalReason = (int)DisposalReasons.Expired;
        }
    }
}