using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.HR.Models
{
    public class Designation
    {
        [Required]
        [Display(Name = "Designation ID")]
        public virtual int DesignationId { get; set; }
        [Required]
        [Display(Name = "Designation")]
        public virtual string Title { get; set; }
    }
}