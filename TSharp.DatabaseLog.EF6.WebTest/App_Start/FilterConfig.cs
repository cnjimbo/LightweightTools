using System.Web;
using System.Web.Mvc;

namespace TSharp.DatabaseLog.EF6.WebTest
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
