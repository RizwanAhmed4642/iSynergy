using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSynergy.Areas.Finance.Models
{
    public class AccountGroup
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Group Code")]
        [Required(ErrorMessage = "Please provide a code for this account")]
        [RegularExpression(@"^[0-9]{2}-[0-9]{2}$", ErrorMessage = "Invalid code format. Correct format is 00-00")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(5)]
        public string AccountGroupId { get; set; }
        [Required(ErrorMessage = "Please provide privde a comma saperated list of all the groups in this class.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Group")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Please select a class for this account group.")]
        [Display(Name = "Class")]
        [RegularExpression(@"^[0-9]{2}$", ErrorMessage = "Invalid code format. Correct format is 00")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(2)]
        public string AccountClassId { get; set; }


        [ForeignKey("AccountClassId")]
        public virtual AccountClass AccountClass { get; set; }

    }

}