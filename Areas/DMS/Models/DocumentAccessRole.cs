using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class DocumentAccessRole
    {
        [Key]
        [Display(Name = "Role Id")]
        public int DocumentAccessRoleId { get; set; }

        [Required(ErrorMessage = "Please provide a name for the role.")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}