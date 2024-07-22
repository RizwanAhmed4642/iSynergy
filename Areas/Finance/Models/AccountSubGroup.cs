using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Finance.Models
{
    public class AccountSubGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Sub Group Code")]
        [Required(ErrorMessage = "Please provide a code for this account")]
        [RegularExpression(@"^[0-9]{2}-[0-9]{2}-[0-9]{2}$", ErrorMessage = "Invalid code format. Correct format is 00-00-00")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(8)]
        public string AccountSubGroupId { get; set; }
        [Required(ErrorMessage = "Please provide privde a comma saperated list of all the sub groups in this group.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Sub Group")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please select a group for this sub group.")]
        [Display(Name = "Group")]
        [RegularExpression(@"^[0-9]{2}-[0-9]{2}$", ErrorMessage = "Invalid code format. Correct format is 00-00")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string AccountGroupId { get; set; }
        [ForeignKey("AccountGroupId")]
        public virtual AccountGroup AccountGroup { get; set; }
    }
}