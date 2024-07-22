using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSynergy.Areas.HR.Models
{
    public class Employee
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }
        [Display(Name = "Department ID")]
        public int DepartmentId { get; set; }
        [Display(Name = "Designation ID")]
        public int DesignationId { get; set; }
        [Display(Name = "Office ID")]
        public int OfficeId { get; set; }
        [Display(Name = "Manager")]
        public int  ManagerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "Joining Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? JoiningDate { get; set; }
        [Display(Name = "Employment Status")]
        public int? EmploymentStatus { get; set; }
        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReleaseDate { get; set; }
        public virtual Department Department { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual Office Office { get; set; }
    }
    public enum EmploymentStatuses
    {
        Probation,
        Permanent,
        Internship
    }
}   