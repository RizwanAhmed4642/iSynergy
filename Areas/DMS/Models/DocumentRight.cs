using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class DocumentRight
    {
        [Key]
        public int DocumentRightsId { get; set; }

        [Display(Name = "Write Access")]
        public bool hasWriteAccess { get; set; }

        [Display(Name = "Role")]
        public int DocumentAccessRoleId { get; set; }

        [ForeignKey("DocumentAccessRoleId")]
        public virtual DocumentAccessRole DocumentAccessRole { get; set; }

        [Display(Name = "Document")]
        public int DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        public DocumentRight()
        {
            hasWriteAccess = false;
        }
    }
}