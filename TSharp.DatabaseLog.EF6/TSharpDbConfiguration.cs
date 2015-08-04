using System.Data.Entity;

namespace TSharp.DatabaseLog.EF6
{
    public class MSSqlDbConfiguration : DbConfiguration
    {
        public MSSqlDbConfiguration()
        {
            SetDatabaseLogFormatter((context, writer) => new MSSqlDatabaseLogFormatter(context, writer));
        }
    }
}