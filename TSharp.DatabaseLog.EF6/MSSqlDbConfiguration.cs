namespace TSharp.DatabaseLog.EF6
{
    using System.Data.Entity;

    public class MSSqlDbConfiguration : DbConfiguration
    {
        public MSSqlDbConfiguration()
        {
            SetDatabaseLogFormatter((context, writer) => new MsSqlDatabaseLogFormatter(context, writer));
        }

        public static bool IsLogConnection { get; set; }

        public static bool IsLogCommand { get; set; }

        public static bool IsLogTransaction { get; set; }

        public static long LogCommandLimitedMilliseconds { get; set; }
    }
}