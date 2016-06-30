// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace TSharp.DatabaseLog.EF6
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure.DependencyResolution;
    using System.Data.Entity.Infrastructure.Interception;
    using System.IO;

    using TSharp.TraceListeners;

    /// <summary>
    ///     A simple logger for logging SQL and other database operations to the console or a file.
    ///     A logger can be registered in code or in the application's web.config /app.config file.
    /// </summary>
    public class TSharpDatabaseLogger : IDisposable
    {
        private readonly object _lock = new object();

        private DatabaseLogFormatter _formatter;

        private Action<string> innerWriter;

        private RollingFlatFileTraceListener traceWriter;

        /// <summary>
        ///     Creates a new logger that will send log output to the console.
        /// </summary>
        public TSharpDatabaseLogger()
        {
            traceWriter = new RollingFlatFileTraceListener(
                @"App_data\Sqls\trace.csv",
                null,
                null,
                1024,
                "yyyyMMddHHmmss",
                "yyyyMMdd",
                RollFileExistsBehavior.Increment,
                RollInterval.Day);
            innerWriter = traceWriter.Write;
        }

        /// <summary>
        ///     Creates a new logger that will send log output to a file. If the file already exists then
        ///     it is overwritten.
        /// </summary>
        /// <param name="path">A path to the file to which log output will be written.</param>
        public TSharpDatabaseLogger(string path)
        {
            traceWriter = new RollingFlatFileTraceListener(
                path,
                null,
                null,
                1024,
                "HHmmssfff",
                "yyyyMMdd",
                RollFileExistsBehavior.Increment,
                RollInterval.Day);
            innerWriter = traceWriter.WriteLine;
        }

        public TSharpDatabaseLogger(TextWriter writer)
        {
            innerWriter = writer.WriteLine;
        }

        /// <summary>
        ///     Stops logging and closes the underlying file if output is being written to a file.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Stops logging and closes the underlying file if output is being written to a file.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; False to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            StopLogging();

            if (disposing && traceWriter != null)
            {
                traceWriter.Dispose();
                traceWriter = null;
            }
            if (disposing && innerWriter != null) innerWriter = null;
        }

        /// <summary>
        ///     Starts logging. This method is a no-op if logging is already started.
        /// </summary>
        public void StartLogging()
        {
            StartLogging(DbConfiguration.DependencyResolver);
        }

        /// <summary>
        ///     Stops logging. This method is a no-op if logging is not started.
        /// </summary>
        public void StopLogging()
        {
            if (_formatter != null)
            {
                DbInterception.Remove(_formatter);
                _formatter = null;
            }
        }

        private void StartLogging(IDbDependencyResolver resolver)
        {
            DebugCheck.NotNull(resolver);

            if (_formatter == null)
            {
                _formatter = resolver.GetService<Func<DbContext, Action<string>, DatabaseLogFormatter>>()(
                    null,
                    WriteThreadSafe);

                DbInterception.Add(_formatter);
            }
        }

        private void WriteThreadSafe(string value)
        {
            lock (_lock)
            {
                innerWriter(value);
            }
        }
    }
}