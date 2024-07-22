using iSynergy.Areas.HR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Inventory.Models
{
    /// <summary>
    /// this a base class for all different types of inventory assets (hardware, computers and bulk assets etc...)
    /// ths calss cannot be initialized
    /// this calss provides the most basic and common attributes to be used for all 
    /// inventory items.
    /// </summary>
    public abstract class Asset
    {
        public int AssetId { get; set; }
        [Required(ErrorMessage = "Please provide asset name")]
        [Display(Name = "Asset Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select an office")]
        [Display(Name = "Property of")]
        public int OfficeId { get; set; }
        public virtual Office Office { get; set; }

        [Required(ErrorMessage = "Please select a custodian")]
        [Display(Name = "Custodian")]
        public int CustodianId { get; set; }

        [ForeignKey("CustodianId")]
        public virtual Employee Employee { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        // -- disposal information --
        public bool Disposed { get; set; }
        public int? DisposalReason { get; set; }
        [DataType(DataType.MultilineText)]
        public string DisposalComments { get; set; }
        public DateTime? DisposalDate { get; set; }
        public int? DisposedById { get; set; }

        [ForeignKey("DisposedById")]
        public virtual Employee DisposedBy { get; set; }
        public Asset()
        {
            Disposed = false;
        }
        public void markDisposed(DateTime MarkingDate, Employee MarkedBy, string Comments)
        {
            Disposed = true;
            DisposedById = MarkedBy.EmployeeId;
            DisposalDate = MarkingDate;
            DisposalComments = Comments;
        }
        public void markOutOfOrder(DateTime MarkingDate, Employee MarkedBy, string Comments)
        {
            markDisposed(MarkingDate, MarkedBy, Comments);
            this.DisposalReason = (int)DisposalReasons.OutOfOrder;
        }
        public void markBroken(DateTime MarkingDate, Employee MarkedBy, string Comments)
        {
            markDisposed(MarkingDate, MarkedBy, Comments);
            this.DisposalReason = (int)DisposalReasons.Broken;
        }
        public void markLost(DateTime MarkingDate, Employee MarkedBy, string Comments)
        {
            markDisposed(MarkingDate, MarkedBy, Comments);
            this.DisposalReason = (int)DisposalReasons.Lost;
        }
    }

    public enum DisposalReasons
    {
        Consumed,
        Expired,
        OutOfOrder, 
        Broken,
        Lost
    }
}