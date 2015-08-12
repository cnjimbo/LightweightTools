using System.Data.Entity;

namespace TSharp.DatabaseLog.EF6
{
    public class MSSqlDbConfiguration : DbConfiguration
    {
        public static bool IsLogConnection
        {
            get;
            set;
        }

        public static bool IsLogCommand
        {
            get;
            set;
        }

        public static bool IsLogTransaction { get; set; }

        public static long LogCommandLimitedMilliseconds { get; set; }
         

        public MSSqlDbConfiguration()
        {
            SetDatabaseLogFormatter((context, writer) => new MSSqlDatabaseLogFormatter(context, writer));
        }
    }
}