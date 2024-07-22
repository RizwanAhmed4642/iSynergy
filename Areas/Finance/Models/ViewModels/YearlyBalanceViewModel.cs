using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Finance.Models
{
    public class YearlyBalanceViewModel
    {
        public AccountClass AccountClass { get; set; }
        public AccountGroup AccountGroup { get; set; }
        public AccountSubGroup AccountSubGroup { get; set; }
        public decimal? Balance { get; set; }
        public int Year { get; set; }
    }
}