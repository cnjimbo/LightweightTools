using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Web;
using TSharp.DatabaseLog.EF6;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStart), "Start")]
namespace TSharp.DatabaseLog.EF6
{
    public class PreApplicationStart
    {
        private static TSharpDatabaseLogger logger = new TSharpDatabaseLogger();
        public static void Start()
        {
            DbConfiguration.SetConfiguration(new MSSqlDbConfiguration());
            logger.StartLogging();

        }

        public static void Stop()
        {
            logger.StopLogging();
        }
    }
}