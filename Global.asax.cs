using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using iSynergy.Models;
using System.Diagnostics;
using Microsoft.ApplicationInsights.Extensibility;

namespace iSynergy
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //This could help improve Asp.net MVC related performance issue , this is one performance 
            //improvement that you can do is to clear all the view engines and add the one(s) that you use. 
            //say for ex:- RazorViewEngine. MVC registers 2 view engines by default Webforms and Razor 
            //view engines, so clearing and adding the ones that is used alone will improve the look up performance.
            //Referece: http://stackoverflow.com/questions/16281366/asp-net-mvc-rendering-seems-slow

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }
        /// <summary>
        /// Disables the application insights locally.
        /// </summary>
        [Conditional("DEBUG")]
        private static void DisableApplicationInsightsOnDebug()
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;
        }
    }
}
