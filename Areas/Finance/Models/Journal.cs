using System;
using System.ComponentModel.DataAnnotations;
using iSynergy.Areas.HR.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel;

namespace iSynergy.Areas.Finance.Models
{
    /// <summary>
    /// A journal is a collection of postings
    /// the overall balance of all postings in a journal will be ZERO
    /// </summary>
    public class Journal
    {
        [Key]
        public int JournalId { get; set; }
        [Required(ErrorMessage = "Please mention a date of this journal entry")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Please provides notes for the journal memo.")]
        [DataType(DataType.MultilineText)]
        public string Memo { get; set; }

        public string Reference { get; set; }

        public int PostedById { get; set; }
        [ForeignKey("PostedById")]
        public virtual Employee PostedBy { get; set; }

        [Required(ErrorMessage = "Please select a voucher type.")]
        public VoucherType VoucherType { get; set; }
    }

    public enum VoucherType
    {
        [Description("Bank Payment Voucher")]
        BPV = 1,
        [Description("Cash Payment Voucher")]
        CPV = 2,
        [Description("Bank Receipt Voucher")]
        BRV = 3,
        [Description("Cash Receipt Voucher")]
        CRV = 4,
        [Description("Journal Voucher")]
        JV = 5
    }
    
}