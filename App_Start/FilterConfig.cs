using System.Web.Mvc;

namespace iSynergy
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute()); // all controllers require login
            //filters.Add(new CheckLicenses()); // all controllers need to check license before they execute. 
            filters.Add(new HandleErrorAttribute());

        }
    }
}
