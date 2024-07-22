using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.HR.Models
{
    public class Department
    {
        [Display(Name = "Department ID")]
        public int DepartmentId { get; set; }
        [Required]
        [Display(Name = "Department")]
        public string Name { get; set; }
        [Display(Name = "Department Head")]
        public int HodId { get; set; }

    }
}