﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace TSharp.DatabaseLog.EF6
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure.DependencyResolution;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Data.Entity.Utilities;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using TSharp.DatabaseLog.EF6;
    using TSharp.TraceListeners;

    /// <summary>
    /// A simple logger for logging SQL and other database operations to the console or a file.
    /// A logger can be registered in code or in the application's web.config /app.config file.
    /// </summary>
    public class TSharpDatabaseLogger : IDisposable
    {
        private RollingFlatFileTraceListener _writer;
        private DatabaseLogFormatter _formatter;
        private readonly object _lock = new object();

        /// <summary>
        /// Creates a new logger that will send log output to the console.
        /// </summary>
        public TSharpDatabaseLogger()
        {
            _writer = new RollingFlatFileTraceListener(@"App_data\Sqls\trace.log", null, null, 1024, "HHmmssfff", "yyyyMMdd",
              RollFileExistsBehavior.Increment, RollInterval.Day);
        }

        /// <summary>
        /// Creates a new logger that will send log output to a file. If the file already exists then
        /// it is overwritten.
        /// </summary>
        /// <param name="path">A path to the file to which log output will be written.</param>
        public TSharpDatabaseLogger(string path)
            : this(path, append: false)
        {
        }

        /// <summary>
        /// Creates a new logger that will send log output to a file.
        /// </summary>
        /// <param name="path">A path to the file to which log output will be written.</param>
        /// <param name="append">True to append data to the file if it exists; false to overwrite the file.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public TSharpDatabaseLogger(string path, bool append)
        {
            _writer = new RollingFlatFileTraceListener(@"App_data\Sqls\trace.log", null, null, 1024, "HHmmssfff", "yyyyMMdd",
                RollFileExistsBehavior.Increment, RollInterval.Day);
        }

        /// <summary>
        /// Stops logging and closes the underlying file if output is being written to a file.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Stops logging and closes the underlying file if output is being written to a file.
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; False to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            StopLogging();

            if (disposing && _writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
        }

        /// <summary>
        /// Starts logging. This method is a no-op if logging is already started.
        /// </summary>
        public void StartLogging()
        {
            StartLogging(DbConfiguration.DependencyResolver);
        }

        /// <summary>
        /// Stops logging. This method is a no-op if logging is not started.
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
                    null, WriteThreadSafe);

                DbInterception.Add(_formatter);
            }
        }

        private void WriteThreadSafe(string value)
        {
            lock (_lock)
            {
                _writer.WriteLine(value);
                _writer.Flush();
            }
        }
    }
}