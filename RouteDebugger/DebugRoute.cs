namespace RouteDebug
{
    using System.Web.Routing;

    public class DebugRoute : Route
    {
        private DebugRoute()
            : base("{*catchall}", new DebugRouteHandler())
        {
        }

        public static DebugRoute Singleton { get; } = new DebugRoute();
    }
}