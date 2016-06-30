using System.Web;

using TSharp.DatabaseLog.EF6;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStart), "Start")]

namespace TSharp.DatabaseLog.EF6
{
    using System.Data.Entity;

    public class PreApplicationStart
    {
        private static readonly TSharpDatabaseLogger logger = new TSharpDatabaseLogger();

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