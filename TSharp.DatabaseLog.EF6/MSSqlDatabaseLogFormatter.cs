#region Using

#endregion

namespace TSharp.DatabaseLog.EF6
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Threading.Tasks;

    using Enumerable = System.Linq.Enumerable;

    public class MsSqlDatabaseLogFormatter : DatabaseLogFormatter
    {
        public MsSqlDatabaseLogFormatter(DbContext context, Action<string> writer)
            : base(context, writer)
        {
        }

        public bool IsLogConnection => MSSqlDbConfiguration.IsLogConnection;

        public bool IsLogCommand
        {
            get
            {
                return MSSqlDbConfiguration.IsLogCommand;
            }
        }

        public bool IsLogTransaction
        {
            get
            {
                return MSSqlDbConfiguration.IsLogTransaction;
            }
        }

        public long LogCommandLimitedMilliseconds
        {
            get
            {
                return MSSqlDbConfiguration.LogCommandLimitedMilliseconds;
            }
        }

        public override void BeginningTransaction(
            DbConnection connection,
            BeginTransactionInterceptionContext interceptionContext)
        {
            base.BeginningTransaction(connection, interceptionContext);
        }

        public override void BeganTransaction(
            DbConnection connection,
            BeginTransactionInterceptionContext interceptionContext)
        {
            if (!IsLogTransaction) return;
            if (Context == null || interceptionContext.DbContexts.Contains(Context, ReferenceEquals))
            {
                var sb = new StringBuilder();
                if (interceptionContext.Exception != null)
                {
                    sb.Append(
                        Strings.TransactionStartErrorLog(
                            DateTimeOffset.Now,
                            interceptionContext.Exception.Message,
                            Environment.NewLine));
                }
                else
                {
                    sb.Append(Strings.TransactionStartedLog(DateTimeOffset.Now, Environment.NewLine));
                }
                Write(sb.ToString());
            }
        }

        public override void EnlistingTransaction(
            DbConnection connection,
            EnlistTransactionInterceptionContext interceptionContext)
        {
            base.EnlistingTransaction(connection, interceptionContext);
        }

        public override void EnlistedTransaction(
            DbConnection connection,
            EnlistTransactionInterceptionContext interceptionContext)
        {
            base.EnlistedTransaction(connection, interceptionContext);
        }

        public override void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            base.Opening(connection, interceptionContext);
        }

        public override void Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            base.Closing(connection, interceptionContext);
        }

        public override void StateGot(
            DbConnection connection,
            DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {
            base.StateGot(connection, interceptionContext);
        }

        public override void ConnectionGetting(
            DbTransaction transaction,
            DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {
            base.ConnectionGetting(transaction, interceptionContext);
        }

        public override void ConnectionGot(
            DbTransaction transaction,
            DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {
            base.ConnectionGot(transaction, interceptionContext);
        }

        public override void IsolationLevelGetting(
            DbTransaction transaction,
            DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
        {
            base.IsolationLevelGetting(transaction, interceptionContext);
        }

        public override void IsolationLevelGot(
            DbTransaction transaction,
            DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
        {
            base.IsolationLevelGot(transaction, interceptionContext);
        }

        public override void Committing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            base.Committing(transaction, interceptionContext);
        }

        public override void Committed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            if (!IsLogTransaction) return;
            if (Context == null || interceptionContext.DbContexts.Contains(Context, ReferenceEquals))
            {
                var sb = new StringBuilder();
                if (interceptionContext.Exception != null)
                {
                    sb.Append(
                        Strings.TransactionCommitErrorLog(
                            DateTimeOffset.Now,
                            interceptionContext.Exception.Message,
                            Environment.NewLine));
                }
                else
                {
                    sb.Append(Strings.TransactionCommittedLog(DateTimeOffset.Now, Environment.NewLine));
                }

                Write(sb.ToString());
            }
        }

        public override void Disposing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            base.Disposing(transaction, interceptionContext);
        }

        public override void Disposed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            base.Disposed(transaction, interceptionContext);
        }

        public override void RollingBack(
            DbTransaction transaction,
            DbTransactionInterceptionContext interceptionContext)
        {
            base.RollingBack(transaction, interceptionContext);
        }

        public override void RolledBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            base.RolledBack(transaction, interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            base.NonQueryExecuting(command, interceptionContext);
        }

        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            base.NonQueryExecuted(command, interceptionContext);
        }

        public override void ReaderExecuting(
            DbCommand command,
            DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            base.ReaderExecuting(command, interceptionContext);
        }

        public override void ReaderExecuted(
            DbCommand command,
            DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            base.ReaderExecuted(command, interceptionContext);
        }

        public override void ScalarExecuting(
            DbCommand command,
            DbCommandInterceptionContext<object> interceptionContext)
        {
            base.ScalarExecuting(command, interceptionContext);
        }

        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            base.ScalarExecuted(command, interceptionContext);
        }

        public override void Executing<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext)
        {
            //base.Executing(command, interceptionContext);
        }

        public override void Executed<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext)
        {
            base.Executed(command, interceptionContext);
        }

        public override void LogCommand<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext)
        {
            base.LogCommand(command, interceptionContext);
        }

        public override void LogParameter<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext,
            DbParameter parameter)
        {
            base.LogParameter(command, interceptionContext, parameter);
        }

        public override void LogResult<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (!IsLogCommand) return;
            if (LogCommandLimitedMilliseconds > 0 && LogCommandLimitedMilliseconds > Stopwatch.ElapsedMilliseconds) return;
            var sb = GetLogCommand(command, interceptionContext);
            if (interceptionContext.Exception != null)
            {
                sb.Append(
                    Strings.CommandLogFailed(
                        Stopwatch.ElapsedMilliseconds,
                        interceptionContext.Exception.Message,
                        Environment.NewLine));
            }
            else if (interceptionContext.TaskStatus.HasFlag(TaskStatus.Canceled))
            {
                sb.Append(Strings.CommandLogCanceled(Stopwatch.ElapsedMilliseconds, Environment.NewLine));
            }
            else
            {
                var result = interceptionContext.Result;
                var resultString = (object)result == null
                                       ? "null"
                                       : result is DbDataReader ? result.GetType().Name : result.ToString();
                sb.Append(Strings.CommandLogComplete(Stopwatch.ElapsedMilliseconds, resultString, Environment.NewLine));
            }
            sb.Append("GO");
            sb.Append(Environment.NewLine);

            Write(sb.ToString());
        }

        public override void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            if (!IsLogConnection) return;
            if (Context == null || interceptionContext.DbContexts.Contains(Context, ReferenceEquals))
            {
                var sb = new StringBuilder();
                if (interceptionContext.Exception != null)
                {
                    sb.Append(
                        interceptionContext.IsAsync
                            ? Strings.ConnectionOpenErrorLogAsync(
                                DateTimeOffset.Now,
                                interceptionContext.Exception.Message,
                                Environment.NewLine)
                            : Strings.ConnectionOpenErrorLog(
                                DateTimeOffset.Now,
                                interceptionContext.Exception.Message,
                                Environment.NewLine));
                }
                else if (interceptionContext.TaskStatus.HasFlag(TaskStatus.Canceled))
                {
                    sb.Append(Strings.ConnectionOpenCanceledLog(DateTimeOffset.Now, Environment.NewLine));
                }
                else
                {
                    sb.Append(
                        interceptionContext.IsAsync
                            ? Strings.ConnectionOpenedLogAsync(DateTimeOffset.Now, Environment.NewLine)
                            : Strings.ConnectionOpenedLog(DateTimeOffset.Now, Environment.NewLine));
                }
                Write(sb.ToString());
            }
        }

        public override void ServerVersionGetting(
            DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            base.ServerVersionGetting(connection, interceptionContext);
        }

        public override void ServerVersionGot(
            DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            base.ServerVersionGot(connection, interceptionContext);
        }

        public override void StateGetting(
            DbConnection connection,
            DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {
            base.StateGetting(connection, interceptionContext);
        }

        public override void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            if (!IsLogConnection) return;
            if (Context == null || interceptionContext.DbContexts.Contains(Context, ReferenceEquals))
            {
                var sb = new StringBuilder();
                if (interceptionContext.Exception != null)
                {
                    sb.Append(
                        Strings.ConnectionCloseErrorLog(
                            DateTimeOffset.Now,
                            interceptionContext.Exception.Message,
                            Environment.NewLine));
                }
                else
                {
                    sb.Append(Strings.ConnectionClosedLog(DateTimeOffset.Now, Environment.NewLine));
                }
                Write(sb.ToString());
            }
        }

        public override void ConnectionStringGetting(
            DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            base.ConnectionStringGetting(connection, interceptionContext);
        }

        public override void ConnectionStringGot(
            DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            base.ConnectionStringGot(connection, interceptionContext);
        }

        public override void ConnectionStringSetting(
            DbConnection connection,
            DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {
            base.ConnectionStringSetting(connection, interceptionContext);
        }

        public override void ConnectionStringSet(
            DbConnection connection,
            DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {
            base.ConnectionStringSet(connection, interceptionContext);
        }

        public override void ConnectionTimeoutGetting(
            DbConnection connection,
            DbConnectionInterceptionContext<int> interceptionContext)
        {
            base.ConnectionTimeoutGetting(connection, interceptionContext);
        }

        public override void ConnectionTimeoutGot(
            DbConnection connection,
            DbConnectionInterceptionContext<int> interceptionContext)
        {
            base.ConnectionTimeoutGot(connection, interceptionContext);
        }

        public override void DatabaseGetting(
            DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            base.DatabaseGetting(connection, interceptionContext);
        }

        public override void DatabaseGot(
            DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            base.DatabaseGot(connection, interceptionContext);
        }

        public override void DataSourceGetting(
            DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            base.DataSourceGetting(connection, interceptionContext);
        }

        public override void DataSourceGot(
            DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            base.DataSourceGot(connection, interceptionContext);
        }

        public override void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            base.Disposing(connection, interceptionContext);
        }

        public override void Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            base.Disposed(connection, interceptionContext);
        }

        #region private method, class

        public StringBuilder GetLogCommand<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext)
        {
            var sb = new StringBuilder(Environment.NewLine);
            foreach (var parameter in Enumerable.OfType<DbParameter>(command.Parameters))
            {
                sb.Append(GetLogParameter(command, interceptionContext, parameter));
                sb.Append(Environment.NewLine);
            }

            if (command.CommandType == CommandType.Text)
            {
                var commandText = command.CommandText ?? "<null>";
                if (commandText.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                {
                    sb.Append(commandText);
                }
                else
                {
                    sb.Append(commandText);
                    sb.Append(Environment.NewLine);
                }
            }
            else if (command.CommandType == CommandType.TableDirect)
            {
                sb.Append("-- TableDirect");
                sb.Append(Environment.NewLine);
                var commandText = command.CommandText ?? "<null>";
                if (commandText.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                {
                    sb.Append(commandText);
                }
                else
                {
                    sb.Append(commandText);
                    sb.Append(Environment.NewLine);
                }
            }
            else if (command.CommandType == CommandType.StoredProcedure)
            {
                var outstring = new StringBuilder().AppendLine();
                outstring.Append("EXECUTE ");
                outstring.Append(command.CommandText ?? "<null>");

                foreach (var parameter in Enumerable.OfType<DbParameter>(command.Parameters))
                {
                    outstring.AppendLine()
                        .Append(GetParameterName(parameter))
                        .Append("=")
                        .Append(GetParameterName(parameter))
                        .Append(",");
                }
                //if(out)
                sb.Append(outstring.ToString().TrimEnd(','));
                sb.Append(Environment.NewLine);
            }

            sb.Append(
                interceptionContext.IsAsync
                    ? Strings.CommandLogAsync(DateTimeOffset.Now, Environment.NewLine)
                    : Strings.CommandLogNonAsync(DateTimeOffset.Now, Environment.NewLine));
            return sb;
        }

        /// <summary>
        ///     Logs the parameter.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="interceptionContext">The interception context.</param>
        /// <param name="parameter">The parameter.</param>
        public StringBuilder GetLogParameter<TResult>(
            DbCommand command,
            DbCommandInterceptionContext<TResult> interceptionContext,
            DbParameter p)
        {
            var parameter = p as SqlParameter;
            if (parameter == null) throw new Exception("当前数据库不是MS-Sql Server无法记录参数信息。");
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
                            parameter.SqlDbType,
                            parameter.Size > 4000 ? "max" : "" + parameter.Size);
                        break;
                    default:
                        sbDeclare.AppendFormat(
                            CultureInfo.InvariantCulture,
                            "DECLARE {0} {1}({2})",
                            GetParameterName(parameter),
                            parameter.SqlDbType,
                            parameter.Size);
                        break;
                }
            }
            else
            {
                sbDeclare.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "DECLARE {0} {1}",
                    GetParameterName(parameter),
                    parameter.SqlDbType);

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

            sbDeclare.AppendFormat(CultureInfo.InvariantCulture, " = {0}", GetValueText(parameter));

            //  sbDeclare.AppendLine();

            return sbDeclare;
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
                return "N'" + value + "'";
            }
            if (db.SqlDbType == SqlDbType.DateTime || db.SqlDbType == SqlDbType.DateTime2
                || db.SqlDbType == SqlDbType.DateTimeOffset)
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
                return string.Format(
                    "--Failed to open connection asynchronously at {0} with error: {1}{2}",
                    now,
                    message,
                    newLine);
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

        #endregion
    }
}