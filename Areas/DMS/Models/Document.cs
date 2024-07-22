using iSynergy.Areas.HR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.DMS.Models
{
    public class Document
    {
        [Key]
        [Display(Name = "Document Id")]
        public int DocumentId { get; set; }

        [Display(Name = "Category")]
        public int DocumentCategoryId { get; set; }

        [ForeignKey("DocumentCategoryId")]
        public virtual DocumentCategory DocumentCategory { get; set; }

        [Display(Name = "Checked Out")]
        public bool isCheckedOut { get; set; }

        [Display(Name = "Locked")]
        public bool isLocked { get; set; }

        [Display(Name = "Locked By")]
        public int lockedBy { get; set; }

        [Display(Name = "Checked Out By")]
        public int checkedOutBy { get; set; }

        public Document()
        {
            isCheckedOut = false;
            isLocked = false;
        }
    }

}