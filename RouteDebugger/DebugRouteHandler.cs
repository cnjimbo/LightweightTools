namespace RouteDebug
{
    using System.Web;
    using System.Web.Routing;

    public class DebugRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new DebugHttpHandler();
        }
    }
}