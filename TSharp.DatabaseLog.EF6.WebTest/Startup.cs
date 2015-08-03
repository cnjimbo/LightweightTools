using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TSharp.DatabaseLog.EF6.WebTest.Startup))]
namespace TSharp.DatabaseLog.EF6.WebTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
