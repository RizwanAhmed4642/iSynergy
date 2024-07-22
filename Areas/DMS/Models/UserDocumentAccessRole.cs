using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using iSynergy.Areas.HR.Models;

namespace iSynergy.Areas.DMS.Models
{
    public class UserDocumentAccessRole
    {
        [Key]
        public int UserDocumentRoleId { get; set; }

        [Display(Name = "Access Role")]
        public int DocumentAccessRoleId { get; set; }

        [ForeignKey("DocumentAccessRoleId")]
        public virtual DocumentAccessRole DocumentAccessRole { get; set; }

        [Display(Name = "User")]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}