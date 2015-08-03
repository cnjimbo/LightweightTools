using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Web;
using TSharp.DatabaseLog.EF6;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStart), "Start")]
namespace TSharp.DatabaseLog.EF6
{
    public class PreApplicationStart
    {
        public static void Start()
        {
            DbConfiguration.SetConfiguration(new TSharpDbConfiguration());
            new TSharpDatabaseLogger().StartLogging();

            //var _formatterFac = (Func<DbContext, Action<string>, DatabaseLogFormatter>)DbConfiguration.DependencyResolver.GetService(typeof(Func<DbContext, Action<string>, DatabaseLogFormatter>), null);
            //var _formatter = _formatterFac(null, null);
            //DbInterception.Add(_formatter);
        }
    }
}