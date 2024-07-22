using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class DocumentCategory
    {
        [Key]
        [Display(Name = "Category Id")]
        public int DocumentCategoryId { get; set; }
        [Required(ErrorMessage = "Please provide a name for the category")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Display(Name = "Folder")]
        public int DocuementFolderId { get; set; }


        [ForeignKey("DocuementFolderId")]
        public virtual DocumentFolder DocumentFolder { get; set; }


        
    }


    
}