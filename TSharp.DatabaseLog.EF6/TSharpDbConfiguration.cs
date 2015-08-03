using System.Data.Entity;

namespace TSharp.DatabaseLog.EF6
{
    public class TSharpDbConfiguration : DbConfiguration
    {
        public TSharpDbConfiguration()
        {
            SetDatabaseLogFormatter((context, writer) => new MSSqlDatabaseLogFormatter(context, writer));
            //DbInterception.Add(new SqlDatabaseLogFormatter(null,null));
        }
    }
}