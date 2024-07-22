using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class CategoryRight
    {
        [Key]
        public int CategoryRightsId { get; set; }

        [Display(Name = "Write Access")]
        public bool hasWriteAccess { get; set; }

        [Display(Name = "Role")]
        public int DocumentAccessRoleId { get; set; }

        [ForeignKey("DocumentAccessRoleId")]
        public virtual DocumentAccessRole DocumentAccessRole { get; set; }

        [Display(Name = "Category")]
        public int DocumentCategoryId { get; set; }

        [ForeignKey("DocumentCategoryId")]
        public virtual DocumentCategory DocumentCategory { get; set; }

        public CategoryRight()
        {
            hasWriteAccess = false;
        }
    }
}