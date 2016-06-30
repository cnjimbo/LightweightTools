using Microsoft.Owin;

using TSharp.DatabaseLog.EF6.WebTest;

[assembly: OwinStartup(typeof(Startup))]

namespace TSharp.DatabaseLog.EF6.WebTest
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}