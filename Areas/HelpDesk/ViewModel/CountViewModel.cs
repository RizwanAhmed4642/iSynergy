using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSynergy.Areas.HelpDesk.ViewModel
{
    public class CountViewModel
    {
        public int PendingRequestForMe { get; set; }
        public int AssignedRequest { get; set; }
        public int TotalRequestForMe { get; set; }
        public int MyTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int MyRequest { get; set; }
    }
}