using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class CategorySubscription
    {
        [Key]
        public int CategorySubscriptionId { get; set; }

        [Required]
        public int DocumentCategoryId { get; set; }

        [ForeignKey("DocumentCategoryId")]
        public virtual DocumentCategory DocumentCategory { get; set; }

        [Required]
        public int EmployeeId { get; set; }
    }
}