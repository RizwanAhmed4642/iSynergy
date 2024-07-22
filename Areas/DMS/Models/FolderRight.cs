using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class FolderRight
    {
        [Key]
        public int FolderRightsId { get; set; }

        [Display(Name = "Write Access")]
        public bool hasWriteAccess { get; set; }

        [Display(Name = "Role")]
        public int DocumentAccessRoleId { get; set; }

        [ForeignKey("DocumentAccessRoleId")]
        public virtual DocumentAccessRole DocumentAccessRole { get; set; }

        [Display(Name = "Folder")]
        public int DocumentFolderId { get; set; }

        [ForeignKey("DocumentFolderId")]
        public virtual DocumentFolder DocumentFolder { get; set; }

        public FolderRight()
        {
            hasWriteAccess = false;
        }
    }
}