using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class DocumentFolder
    {
        [Key]
        [Display(Name = "Folder Id")]
        public int DocumentFolderId { get; set; }
        [Required(ErrorMessage = "Please provide a name for the folder")]
        [Display(Name = "Folder Name")]
        public string Name { get; set; }
    }
}