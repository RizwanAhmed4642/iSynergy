using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSynergy.Areas.Finance.Models
{
    public class Account
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Code")]
        [Required(ErrorMessage = "Please provide a code for this account")]
        [RegularExpression(@"^[0-9]{2}-[0-9]{2}-[0-9]{2}-[0-9]{4}$", ErrorMessage = "Invalid code format. Correct format is 00-00-00-0000")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(13)]
        public string AccountId { get; set; }



        [Required(ErrorMessage = "Please provide provide a comma saperated list of all the accounts in this sub group.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Account")]
        public string Title { get; set; }



        [Required(ErrorMessage = "Please select a group this account belongs to.")]
        [Display(Name = "Sub Group")]
        [RegularExpression(@"^[0-9]{2}-[0-9]{2}-[0-9]{2}$", ErrorMessage = "Invalid code format. Correct format is 00-00-00")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(8)]
        public string AccountSubGroupId { get; set; }



        [ForeignKey("AccountSubGroupId")]
        public virtual AccountSubGroup AccountSubGroup { get; set; }



        public AccountStatus Status { get; set; }
    }

    public enum AccountStatus
    {
        Active,
        [Display(Name = "Inactive")]
        InActive
    }
}