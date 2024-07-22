using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.HelpDesk.Models
{
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }

        public string Subject { get; set; }
        public string UserId { get; set; }
        public string ServicesCategory { get; set; }
        public int ServicesCategoryId { get; set; }
        public string Department { get; set; }
        public int DepartmentId { get; set; }
        public string Attachment { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public int AssignedEmpId { get; set; }
        public string AssignedEmpName { get; set; }
        public DateTime? DueDate { get; set; }
        public string status { get; set; }
        public string Priorty { get; set; }
        public string HOD { get; set; }
        public int HODId { get; set; }
        public string HodComments { get; set; }
        public string Description { get; set; }
        public string EmolyeeComments { get; set; }


    }
}