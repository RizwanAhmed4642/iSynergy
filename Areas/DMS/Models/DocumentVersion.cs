using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class DocumentVersion
    {
        [Key]
        [Display(Name = "Version Id")]
        public int DocumentVersionId { get; set; }

        [Required(ErrorMessage = "Please specify version number")]
        [Display(Name = "Version #")]
        public int VersionNumber { get; set; }

        [Display(Name = "Download")]
        [Required(ErrorMessage = "Please provide URL for the current document version")]
        public string URL { get; set; }


        [Display(Name = "Date Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Please specify the person who added the document")]
        [Display(Name = "Archived By")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please specify the document")]
        [Display(Name = "Document")]
        public int DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }
        public DocumentVersion()
        {
            DateCreated = DateTime.Today;
        }
    }
}