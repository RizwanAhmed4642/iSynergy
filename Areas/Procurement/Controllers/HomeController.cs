using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSynergy.Controllers;

namespace iSynergy.Areas.Procurement.Controllers
{
    
    public class HomeController : CustomController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}