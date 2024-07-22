using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace iSynergy.Areas.Finance.Models
{
    public class AccountClass
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Class Code")]
        [Required(ErrorMessage = "Please provide a code for this account")]
        [RegularExpression(@"^[0-9]{2}$", ErrorMessage = "Invalid code format. Correct format is 00")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(2)]
        public string AccountClassId { get; set; }
        [Required(ErrorMessage = "Please provide provide a comma saperated list of all the account classes.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Class")]
        public string Title { get; set; }
        
    }
}