using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSynergy.Controllers;

namespace iSynergy.Areas.HR.Controllers
{
    public class HomeController : CustomController
    {
        // GET: HR/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}