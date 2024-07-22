using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iSynergy.Areas.HelpDesk.ViewModel
{
    public class ServiceRequestViewModel
    {
        public IEnumerable<SelectListItem> PriortyList { get; set; }
        public ServiceRequestViewModel()
        {
            PriortyList = Enum.GetNames(typeof(Priorty)).Select(name => new SelectListItem()
            {
                Text = name,
                Value = name
            });
        }
        public int Id { get; set; }
        public string UserId { get; set; }
     
        public string ServicesCategory { get; set; }

        public string Email { get; set; }
      
        public string Department { get; set; }
        [Required(ErrorMessage = "Please enter subject")]
        public string Subject { get; set; }
        public string ServicesCategoryId { get; set; }
        [Required(ErrorMessage = "Please Select Department")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Department")]
        public string DepartmentId { get; set; }
        public string Attachment { get; set; }
        public int HODId { get; set; }
        public DateTime CreateDate { get; set; }

        public DateTime? AssignedDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DueDate { get; set; }
        public string status { get; set; }
       
        public string Priorty { get; set; }
        public string HOD { get; set; }
        [Required(ErrorMessage = "Please Enter Description")]
        public string Description { get; set; }
        public HttpPostedFileBase Files { get; set; }
        public string AssignedEmpId { get; set; }
        public string AssignedEmpName { get; set; }
        public string HodComments { get; set; }
 
        public string EmolyeeComments { get; set; }
    }
    public enum Priorty
    {
        High,
        Low,
        Normal,

    }
    public enum status
    {
        Pending,
       Inprogress,
       RollBack,
        Done,


    }
}