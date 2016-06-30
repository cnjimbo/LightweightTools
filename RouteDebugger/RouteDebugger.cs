namespace RouteDebug
{
    using System.Web.Routing;

    public static class RouteDebugger
    {
        public static void RewriteRoutesForTesting(RouteCollection routes)
        {
            using (routes.GetReadLock())
            {
                var foundDebugRoute = false;
                foreach (var routeBase in routes)
                {
                    var route = routeBase as Route;
                    if (route != null)
                    {
                        route.RouteHandler = new DebugRouteHandler();
                    }

                    if (route == DebugRoute.Singleton) foundDebugRoute = true;
                }
                if (!foundDebugRoute)
                {
                    routes.Add(DebugRoute.Singleton);
                }
            }
        }
    }
}