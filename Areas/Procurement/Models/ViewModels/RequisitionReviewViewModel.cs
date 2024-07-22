using iSynergy.DataContexts;
using iSynergy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Procurement.Models
{
    public class RequisitionReviewViewModel
    {
        public bool readOnlyAccess { get; set; }
        public string guid { get; set; }
        public bool isOperationsHeadReviewing { get; set; }
        public bool isCooReviewing { get; set; }
        public bool isInitiatorReviewing { get; set; }
        public bool isFinanceManagerReviewing { get; set; }

        public bool isProcurementManagerReviewing { get; set; }

        public Requisition requisition { get; set; }
        [Display(Name = "Items")]
        public IEnumerable<RequisitionItem> requisitionItems { get; set; }
        [Display(Name = "Quotations")]
        public IEnumerable<RequisitionQuotation> requisitionQuotations { get; set; }
        [Required(ErrorMessage = "Please select a quotation")]
        [Display(Name = "Quotation")]
        public int selectedQuotationId { get; set; }

        [Display(Name = "Approval History")]
        public IEnumerable<RequisitionEvent> requisitionEvents { get; set; }
        [Display(Name = "Bill(s)")]
        public IEnumerable<RequisitionBill> requisitionBills { get; set; }

        [Display(Name = "Bill(s)")]
        [Required(ErrorMessage = "Please upload requisition bill(s)")]
        public List<HttpPostedFileBase> Bills { get; set; }
        public RequisitionEvent newEvent { get; set; }
        public EmployeeIdtoNameDictonary employeeIdToNameConverter { get; set; }

        public RequisitionReviewViewModel()
        {
            Bills = new List<HttpPostedFileBase>();
        }
    }
    public class EmployeeIdtoNameDictonary : Dictionary<int, string>
    {
        public void Add(int empId, CompanyDb companyDb)
        {

            string empName = companyDb.Employees.
                                Where(e => e.EmployeeId == empId)
                                .First().Name;
            if (!base.ContainsKey(empId))
            {
                base.Add(empId, empName);
            }

        }
    }
}