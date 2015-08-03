using System.Web.Routing;

namespace RouteDebug
{
    public class DebugRoute : Route
    {
        private static readonly DebugRoute singleton = new DebugRoute();

        private DebugRoute()
            : base("{*catchall}", new DebugRouteHandler())
        {
        }

        public static DebugRoute Singleton
        {
            get { return singleton; }
        }
    }
}