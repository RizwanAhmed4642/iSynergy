using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iSynergy.Areas.Finance.Models;
using iSynergy.DataContexts;
using System.Data.Entity.Core.Objects;
using iSynergy.Controllers;
using iSynergy.Areas.HelpDesk.ViewModel;

namespace iSynergy.Areas.HelpDesk.Controllers
{
    public class HomeController : CustomController
    {
        private FullDb db = new FullDb();
        // GET: HelpDesk/Home
        public ActionResult Index()
        {
            CountViewModel model = new CountViewModel();
            try
            {
                string myid = "";
                if (Session["userId"].ToString() != "")
                {
                    myid = Session["userId"].ToString();
                }
           
                var hodid = Convert.ToInt32(Session["empId"].ToString());
                var _pendingRequestForMe = db.ServiceRequests.Where(x => x.HODId == hodid && x.status == status.Pending.ToString()).ToList();
                var _allRequestForMe = db.ServiceRequests.Where(x => x.HODId == hodid).ToList();
                var _myRequest = db.ServiceRequests.Where(x => x.UserId == myid).ToList();


                var _assignRequestToEmployee = db.ServiceRequests.Where(x => x.HODId == hodid && x.status == status.Inprogress.ToString()).ToList();

                var _empid = Convert.ToInt32(Session["empId"].ToString());
                var _mytasks = db.ServiceRequests.Where(x => x.AssignedEmpId == _empid && x.status == status.Inprogress.ToString()).ToList();
                var _mytasksCompleted = db.ServiceRequests.Where(x => x.AssignedEmpId == _empid && x.status == status.Done.ToString()).ToList();

                model.PendingRequestForMe = _pendingRequestForMe.Count;
                model.TotalRequestForMe = _allRequestForMe.Count;
                model.TotalRequestForMe = _allRequestForMe.Count;
                model.MyTasks = _mytasks.Count;
                model.CompletedTasks = _mytasksCompleted.Count;
                model.AssignedRequest = _assignRequestToEmployee.Count;

                model.MyRequest = _myRequest.Count;

                return View(model);
            }
            catch (Exception ex)
            {

                var Massage = ex.Message;
                return View(model);

            }
        }
      
    }
}