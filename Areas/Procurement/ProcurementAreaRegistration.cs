﻿using System.Web.Mvc;

namespace iSynergy.Areas.Procurement
{
    public class ProcurementAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Procurement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Procurement_default",
                "Procurement/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}