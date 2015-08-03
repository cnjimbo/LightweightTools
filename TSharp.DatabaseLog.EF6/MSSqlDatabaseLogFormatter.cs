#region Using

using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TSharp.DatabaseLog.EF6;

#endregion


namespace TSharp.DatabaseLog.EF6
{
    public class MSSqlDatabaseLogFormatter : DatabaseLogFormatter
    {
        private static ClassInstanceCounter<DbConnection> counter = new ClassInstanceCounter<DbConnection>();
    
        public MSSqlDatabaseLogFormatter(DbContext context, Action<string> writeAction)
            : base(context, writeAction)
        {
         
        } 
        public override void BeganTransaction(DbConnection connection,
            BeginTransactionInterceptionContext interceptionContext)
        {
            if (Context == null
                || interceptionContext.DbContexts.Contains(Context, ReferenceEquals))
            {
                if (interceptionContext.Exception != null)
                {
                    Write(Strings.TransactionStartErrorLog(DateTimeOffset.Now, interceptionContext.Exception.Message,
                        Environment.NewLine));
                }
                else
                {
                    Write(Strings.TransactionStartedLog(DateTimeOffset.Now, Environment.NewLine));
                }
            }
        }

        public override void LogCommand<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            Write(Environment.NewLine);
            foreach (var parameter in command.Parameters.OfType<DbParameter>())
            {
                LogParameter(command, interceptionContext, parameter);
                Write(Environment.NewLine);
            }

            if (command.CommandType == CommandType.Text)
            {
                var commandText = command.CommandText ?? "<null>";
                if (commandText.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                {
                    Write(commandText);
                }
                else
                {
                    Write(commandText);
                    Write(Environment.NewLine);
                }
            }
            else if (command.CommandType == CommandType.TableDirect)
            {
                Write("-- TableDirect");
                Write(Environment.NewLine);
                var commandText = command.CommandText ?? "<null>";
                if (commandText.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                {
                    Write(commandText);
                }
                else
                {
                    Write(commandText);
                    Write(Environment.NewLine);
                }
            }
            else if (command.CommandType == CommandType.StoredProcedure)
            {
                var outstring = new StringBuilder().AppendLine();
                outstring.Append("EXECUTE ");
                outstring.Append(command.CommandText ?? "<null>");

                foreach (var parameter in command.Parameters.OfType<DbParameter>())
                {
                    outstring.AppendLine()
                        .Append(GetParameterName(parameter))
                        .Append("=")
                        .Append(GetParameterName(parameter))
                        .Append(",");
                }
                //if(out)
                Write(outstring.ToString().TrimEnd(','));
                Write(Environment.NewLine);
            }


            Write(
                interceptionContext.IsAsync
                    ? Strings.CommandLogAsync(DateTimeOffset.Now, Environment.NewLine)
                    : Strings.CommandLogNonAsync(DateTimeOffset.Now, Environment.NewLine));
        }

        public override void LogResult<TResult>(DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (interceptionContext.Exception != null)
            {
                Write(
                    Strings.CommandLogFailed(
                        Stopwatch.ElapsedMilliseconds, interceptionContext.Exception.Message, Environment.NewLine));
            }
            else if (interceptionContext.TaskStatus.HasFlag(TaskStatus.Canceled))
            {
                Write(Strings.CommandLogCanceled(Stopwatch.ElapsedMilliseconds, Environment.NewLine));
            }
            else
            {
                var result = interceptionContext.Result;
                var resultString = (object)result == null
                    ? "null"
                    : (result is DbDataReader)
                        ? result.GetType().Name
                        : result.ToString();
                Write(Strings.CommandLogComplete(Stopwatch.ElapsedMilliseconds, resultString, Environment.NewLine));
            }
            Write("GO");
            Write(Environment.NewLine);
        }

        /// <summary>
        /// Logs the parameter.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="interceptionContext">The interception context.</param>
        /// <param name="parameter">The parameter.</param>
        public override void LogParameter<TResult>(DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext, DbParameter p)
        {
            var parameter = p as SqlParameter;
            if (parameter == null)
                throw new Exception("当前数据库不是MS-Sql Server无法记录参数信息。");
            var sbDeclare = new StringBuilder();
            // -- Name: [Value] (Type = {}, Direction = {}, IsNullable = {}, Size = {}, Precision = {} Scale = {})
            //var sbInfo = new StringBuilder();
            //sbInfo.Append("-- ")
            //    .Append(parameter.ParameterName)
            //    .Append(": '")
            //    .Append((parameter.Value == null || parameter.Value == DBNull.Value) ? "null" : parameter.Value)
            //    .Append("' (Type = ")
            //    .Append(parameter.DbType);

            if (parameter.Direction != ParameterDirection.Input)
            {
                //sbInfo.Append(", Direction = ").Append(parameter.Direction);
            }

            if (!parameter.IsNullable)
            {
                //sbInfo.Append(", IsNullable = false");
            }

            if (parameter.Size > 0)
            {
                switch (parameter.SqlDbType)
                {
                    case SqlDbType.NVarChar:
                    case SqlDbType.NChar:

                        sbDeclare.AppendFormat(
                            CultureInfo.InvariantCulture,
                            "DECLARE {0} {1}({2})",
                            GetParameterName(parameter),
                            parameter.SqlDbType, parameter.Size > 4000 ? "max" : "" + parameter.Size
                            );
                        break;
                    default:
                        sbDeclare.AppendFormat(
                            CultureInfo.InvariantCulture,
                            "DECLARE {0} {1}({2})",
                            GetParameterName(parameter),
                            parameter.SqlDbType,
                            parameter.Size
                            );
                        break;
                }
            }
            else
            {
                sbDeclare.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "DECLARE {0} {1}",
                    GetParameterName(parameter), parameter.SqlDbType);

                // GetSqlDeclareType(parameter.DbType));
            }

            if (parameter.Precision != 0)
            {
                //sbInfo.Append(", Precision = ").Append(((IDbDataParameter)parameter).Precision);
            }

            if (parameter.Scale != 0)
            {
                //sbInfo.Append(", Scale = ").Append(((IDbDataParameter)parameter).Scale);
            }

            //sbInfo.Append(")");

            sbDeclare.
                AppendFormat(CultureInfo.InvariantCulture, " = {0}", GetValueText(parameter));

            //  sbDeclare.AppendLine();

            Write(sbDeclare.ToString());
        }

        public override void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            counter.Monitor(connection);

            if (Context == null
                || interceptionContext.DbContexts.Contains(Context, ReferenceEquals))
            {
                if (interceptionContext.Exception != null)
                {
                    Write(
                        interceptionContext.IsAsync
                            ? Strings.ConnectionOpenErrorLogAsync(
                                DateTimeOffset.Now, interceptionContext.Exception.Message, Environment.NewLine)
                            : Strings.ConnectionOpenErrorLog(DateTimeOffset.Now, interceptionContext.Exception.Message,
                                Environment.NewLine));
                }
                else if (interceptionContext.TaskStatus.HasFlag(TaskStatus.Canceled))
                {
                    Write(Strings.ConnectionOpenCanceledLog(DateTimeOffset.Now, Environment.NewLine));
                }
                else
                {
                    Write(
                        interceptionContext.IsAsync
                            ? Strings.ConnectionOpenedLogAsync(DateTimeOffset.Now, Environment.NewLine)
                            : Strings.ConnectionOpenedLog(DateTimeOffset.Now, Environment.NewLine));
                }
            }
        }

        public override void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            if (Context == null
                || interceptionContext.DbContexts.Contains(Context, ReferenceEquals))
            {
                if (interceptionContext.Exception != null)
                {
                    Write(Strings.ConnectionCloseErrorLog(DateTimeOffset.Now, interceptionContext.Exception.Message,
                        Environment.NewLine));
                }
                else
                {
                    Write(Strings.ConnectionClosedLog(DateTimeOffset.Now, Environment.NewLine));
                }
            }

            var list = counter.GetAllInstance();

            var n = "" + list.Sum(d => (d != null && d.State != ConnectionState.Closed) ? 1 : 0);
            Write(string.Format("--Actived connection count at {0}:  is {1}{2}", DateTimeOffset.Now, n, Environment.NewLine));

        }

        public override void Committed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            if (Context == null
                || interceptionContext.DbContexts.Contains(Context, ReferenceEquals))
            {
                if (interceptionContext.Exception != null)
                {
                    Write(Strings.TransactionCommitErrorLog(DateTimeOffset.Now, interceptionContext.Exception.Message,
                        Environment.NewLine));
                }
                else
                {
                    Write(Strings.TransactionCommittedLog(DateTimeOffset.Now, Environment.NewLine));
                }
            }
        }

        private string GetParameterName(DbParameter par)
        {
            return par.ParameterName.StartsWith("@") ? par.ParameterName : "@" + par.ParameterName;
        }

        private static string GetValueText(SqlParameter db)
        {
            var value = db.Value;
            if (value == null || value is DBNull || value == DBNull.Value)
            {
                return "null";
            }
            if (value is string || value is Guid)
            {
                return "'" + value + "'";
            }
            if (db.SqlDbType == SqlDbType.DateTime || db.SqlDbType == SqlDbType.DateTime2 ||
                db.SqlDbType == SqlDbType.DateTimeOffset)
            {
                return string.Format("'{0}'", value);
            }
            if (db.SqlDbType == SqlDbType.Date)
            {
                return string.Format("'{0:yyyy-MM-dd}'", value);
            }
            if (db.SqlDbType == SqlDbType.Bit)
            {
                return string.Empty + (true.Equals(value) ? 1 : 0);
            }
            if (db.SqlDbType == SqlDbType.Binary || db.SqlDbType == SqlDbType.VarBinary)
            {
                var bts = value as byte[];
                var sb = new StringBuilder("0x");
                Debug.Assert(bts != null, "bts != null");
                foreach (var bt in bts)
                {
                    sb.AppendFormat("{0:X2}", bt);
                }

                return sb.ToString();
            }
            return value.ToString();
        }

        private class Strings
        {
            internal static string CommandLogFailed(long elapsedMilliseconds, string errorMessage, string newLine)
            {
                return string.Format("-- 耗时{0}毫秒,失败：{1} {2}", elapsedMilliseconds, errorMessage, newLine);
                //throw new NotImplementedException();
            }

            public static string CommandLogCanceled(long elapsedMilliseconds, string newLine)
            {
                return string.Format("-- 耗时{0}毫秒,命令被取消。{1} ", elapsedMilliseconds, newLine);
            }

            public static string CommandLogComplete(long elapsedMilliseconds, string resultString, string newLine)
            {
                return string.Format("-- 耗时{0}毫秒,命令完成。结果：{1} {2} ", elapsedMilliseconds, resultString, newLine);
            }

            public static string CommandLogAsync(DateTimeOffset now, string newLine)
            {
                return string.Format("-- 命令异步开始执行：{0}。 {1} ", now, newLine);
            }

            public static string CommandLogNonAsync(DateTimeOffset now, string newLine)
            {
                return string.Format("-- 命令同步开始执行：{0}。 {1} ", now, newLine);
            }

            public static string ConnectionOpenErrorLogAsync(DateTimeOffset now, string message, string newLine)
            {
                return string.Format("--Failed to open connection asynchronously at {0} with error: {1}{2}", now,
                    message, newLine);
            }

            public static string ConnectionOpenErrorLog(DateTimeOffset now, string message, string newLine)
            {
                return string.Format("--Failed to open connection at {0} with error: {1}{2}", now, message, newLine);
            }

            public static string ConnectionOpenedLog(DateTimeOffset now, string newLine)
            {
                return string.Format("--Opened connection at {0}{1}", now, newLine);
            }

            public static string ConnectionOpenCanceledLog(DateTimeOffset now, string newLine)
            {
                return string.Format("--Cancelled open connection at {0}{1}", now, newLine);
            }

            public static string ConnectionOpenedLogAsync(DateTimeOffset now, string newLine)
            {
                return string.Format("--Opened connection asynchronously at {0}{1}", now, newLine);
            }

            internal static string ConnectionCloseErrorLog(DateTimeOffset dateTimeOffset, string p1, string p2)
            {
                return string.Format("--Failed to close connection at {0} with error: {1}{2}", dateTimeOffset, p1, p2);
            }

            internal static string ConnectionClosedLog(DateTimeOffset dateTimeOffset, string p)
            {
                return string.Format("--Closed connection at {0}{1}", dateTimeOffset, p);
            }

            public static string TransactionCommitErrorLog(DateTimeOffset now, string message, string newLine)
            {
                return string.Format("--Failed to commit transaction at {0} with error: {1}{2}", now, message, newLine);
            }

            public static string TransactionCommittedLog(DateTimeOffset now, string newLine)
            {
                return string.Format("--Committed transaction at {0}{1}", now, newLine);
            }

            public static string TransactionStartErrorLog(DateTimeOffset now, string message, string newLine)
            {
                return string.Format("--Failed to start transaction at {0} with error: {1}{2}", now, message, newLine);
            }

            public static string TransactionStartedLog(DateTimeOffset now, string newLine)
            {
                return string.Format("--Started transaction at {0}{1}", now, newLine);
            }
        }
    }
}