using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using RouteDebug;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStart), "Start")]

namespace RouteDebug
{
    public class PreApplicationStart
    {
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(RouteDebuggerHttpModule));
        }
    }
}