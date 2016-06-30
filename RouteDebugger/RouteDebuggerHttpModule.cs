namespace RouteDebug
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Routing;

    public class RouteDebuggerHttpModule : IHttpModule
    {
        private static readonly HashSet<string> resourceExtensions = new HashSet<string>(new E());

        static RouteDebuggerHttpModule()
        {
            foreach (var p in "~/Content|~/Scripts|~/Html".Split('|')) resourceExtensions.Add(p.ToLower());
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += OnEndRequest;
            context.BeginRequest += OnBeginRequest;
        }

        public void Dispose()
        {
        }

        private static void OnBeginRequest(object sender, EventArgs e)
        {
            if (RouteTable.Routes.Count == 0 || RouteTable.Routes.Last() != DebugRoute.Singleton)
            {
                RouteTable.Routes.Add(DebugRoute.Singleton);
            }
        }

        private static void OnEndRequest(object sender, EventArgs e)
        {
            var currentpath = HttpContext.Current.Request.PhysicalPath;
            if (
                !resourceExtensions.Any(
                    x =>
                    currentpath.StartsWith(HttpContext.Current.Server.MapPath(x), StringComparison.OrdinalIgnoreCase)))
            {
                var handler = new DebugHttpHandler();
                handler.ProcessRequest(HttpContext.Current);
            }
        }

        private class E : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.ToLower().GetHashCode();
            }
        }
    }
}