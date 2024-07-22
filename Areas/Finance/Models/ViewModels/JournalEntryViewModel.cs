using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace iSynergy.Areas.Finance.Models
{

    public class JournalEntryViewModel
    {

        [Required(ErrorMessage = "Please select a date.")]
        [DataType(DataType.Date)]

        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Please provides notes for the journal memo.")]
        [DataType(DataType.MultilineText)]
        public string Memo { get; set; }

        public List<Posting> Postings { get; set; }

        public string Reference { get; set; }
        public int JournalId { get; set; }

        [Required(ErrorMessage = "Please select a voucher type.")]
        [Display(Name = "Voucher Type")]
        public VoucherType? VoucherType { get; set; }

    }
}