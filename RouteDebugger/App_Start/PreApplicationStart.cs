using System.Web;

using RouteDebug;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStart), "Start")]

namespace RouteDebug
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    public class PreApplicationStart
    {
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(RouteDebuggerHttpModule));
        }
    }
}