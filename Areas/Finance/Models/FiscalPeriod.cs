using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.Finance.Models
{
    public class FiscalPeriod
    {
        public int FiscalPeriodId { get; set; }
        public int FiscalYear { get; set; }
        public int Period { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public FiscalPeriodStatus Status { get; set; }
        public FiscalPeriod()
        {
            Status = FiscalPeriodStatus.Open;
        }

    }

    public enum FiscalPeriodStatus
    {
        Open,
        Frozen,
        Closed
    }
}