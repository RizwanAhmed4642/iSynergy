using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSynergy.Areas.Finance.Models
{
    /// <summary>
    /// a posting is a single entry in a journal. 
    /// multiple positings will make a journal
    /// </summary>
    public class Posting
    {
        [Key]
        public int PostingId { get; set; }
        [Required(ErrorMessage = "Please select an associated Journal entry for this Posting.")]
        [Display(Name = "Journal")]
        public int JournalId { get; set; }
        [ForeignKey("JournalId")]
        public virtual Journal Journal { get; set; }

        [Required(ErrorMessage = "Please provides notes for the posting memo.")]
        public string Memo { get; set; }
        [DataType(DataType.Currency)]
        public decimal? Debit { get; set; }
        [DataType(DataType.Currency)]

        public decimal? Credit { get; set; }

        [Required(ErrorMessage = "Please select an associated account with this posting.")]
        [Display(Name = "Account")]
        [RegularExpression(@"((^|, )([0-9]{2}-[0-9]{2}-[0-9]{2}-[0-9]{4}))+$", ErrorMessage = "Invalid code format. Correct format is 00-00-00-0000")]
        public string AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public string Attachment { get; set; }
    }
}