using iSynergy.Areas.HR.Models;
using System.ComponentModel.DataAnnotations;

namespace iSynergy.Areas.Finance.Models
{
    public class BankAccount
    {
        [Required]
        public virtual int BankAccountId { get; set; }
        [Required(ErrorMessage = "Please provide account number")]
        [Display(Name = "Account Number")]
        public virtual string AccountNumber { get; set; }
        [Required(ErrorMessage = "Please provide account title")]
        [Display(Name = "Account Title")]
        public string AccountTitle { get; set; }
        [Required(ErrorMessage = "Please select an office")]
        [Display(Name = "Office")]
        public int OfficeId { get; set; }
        [Required(ErrorMessage = "Please select a bank")]
        [Display(Name = "Bank")]
        public virtual int BankId { get; set; }

        public virtual Office Office { get; set; }
        public virtual Bank Bank { get; set; }
    }
}