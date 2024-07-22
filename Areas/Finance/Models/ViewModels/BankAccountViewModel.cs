using iSynergy.Areas.HR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Finance.Models
{
    public class BankAccountViewModel
    {
        public int BankAccountId { get; set; }
        public string AccountTitleWithBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string AccountTitle { get; set; }
        public virtual Office Office { get; set; }
    }
}