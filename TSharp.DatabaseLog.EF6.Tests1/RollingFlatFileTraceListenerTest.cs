// <copyright file="RollingFlatFileTraceListenerTest.cs">Copyright ©  2015</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TSharp.DatabaseLog.EF6.TraceListeners;

namespace TSharp.DatabaseLog.EF6.TraceListeners.Tests
{
    /// <summary>This class contains parameterized unit tests for RollingFlatFileTraceListener</summary>
    [PexClass(typeof(RollingFlatFileTraceListener))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class RollingFlatFileTraceListenerTest
    {
        /// <summary>Test stub for .ctor(String, String, String, Int32, String, String, RollFileExistsBehavior, RollInterval)</summary>
        [PexMethod]
        public RollingFlatFileTraceListener ConstructorTest(
            string fileName,
            string header,
            string footer,
            int rollSizeKB,
            string timeStampPattern,
            string archivedFolderPattern,
            RollFileExistsBehavior rollFileExistsBehavior,
            RollInterval rollInterval
        )
        {
            RollingFlatFileTraceListener target
               = new RollingFlatFileTraceListener(fileName, header, footer, rollSizeKB, 
                                                  timeStampPattern, archivedFolderPattern, 
                                                  rollFileExistsBehavior, rollInterval);
            return target;
            // TODO: add assertions to method RollingFlatFileTraceListenerTest.ConstructorTest(String, String, String, Int32, String, String, RollFileExistsBehavior, RollInterval)
        }
    }
}
