using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.HR.Models
{
    public class Office
    {
        [Required]
        [Display(Name = "Office ID")]
        public virtual int OfficeId { get; set; }
        [Required]
        [Display(Name = "Location")]
        public virtual string Location { get; set; }
        [Display(Name = "Operations Head")]
        public int LocalOperationsHeadId { get; set; }
        [Display(Name = "Finance Head")]
        public int LocalFinanceHeadId { get; set; }
        [Display(Name = "Procurement Manager")]
        public int LocalProcurementManagerId { get; set; }
        [Display(Name = "HR Manager")]
        public int LocalHrManagerId { get; set; }
    }
}