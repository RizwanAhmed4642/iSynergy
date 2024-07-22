using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.HR.Models
{
    public class DepartmentOfficeHead
    {
        public int DepartmentOfficeHeadId { get; set; }
        public int OfficeId { get; set; }
        public int DepartmentId { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public virtual Office Office { get; set; }
        public virtual Department Department { get; set; }
    }
}