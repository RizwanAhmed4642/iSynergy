using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Finance.Models
{
    public class LedgerViewModel
    {
        public Account Account { get; set; }
        public decimal? OpeningDebitBalance { get; set; }
        public decimal? OpeningCreditBalance { get; set; }
        public List<Posting> Postings { get; set; }
        public string CompanyLogo { get; set; }
    }
}