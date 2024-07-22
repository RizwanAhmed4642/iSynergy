using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSynergy.Areas.HR.Models
{
    public class EmployeeDocument
    {
        public int EmployeeDocumentId { get; set; }
        [Required(ErrorMessage = "Please select an employee")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        [Required(ErrorMessage = "Please attach a document")]
        public string file { get; set; }
        [Required(ErrorMessage = "Please provide title information")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please select a date of issue.")]
        [Display(Name = "Date of issue")]
        public DateTime DateIssued { get; set; }
    }
}