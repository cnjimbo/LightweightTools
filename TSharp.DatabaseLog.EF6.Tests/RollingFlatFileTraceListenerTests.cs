namespace TSharp.TraceListeners.Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TSharp.DatabaseLog.EF6;
    using DatabaseLog.EF6.TraceListeners;

    [TestClass]
    public class RollingFlatFileTraceListenerTests
    {
        [TestMethod]
        public void RollingFlatFileTraceListenerTest()
        {
            var writer = new RollingFlatFileTraceListener(
                "trace.log",
                null,
                null,
                512,
                "HHmmss",
                "yyyyMMdd",
                RollFileExistsBehavior.Increment,
                RollInterval.Day);

            writer.WriteLine("rrrrrr");

            writer.Flush();
        }

        [TestMethod]
        public void RollingFlatFileTraceListenerTest1()
        {
            // Assert.Fail();
        }

        [TestMethod]
        public void WriteLineTest()
        {
            // Assert.Fail();
        }

        [TestMethod]
        public void WriteTest()
        {
            // Assert.Fail();
        }

        [TestMethod]
        public void WriteCsvLineTest()
        {
            var rsg = new RandomStringGenerator(true, true, true, true);

            var writer = new RollingFlatFileTraceListener(
                "trace.log",
                null,
                null,
                512,
                "HHmmss",
                "yyyyMMdd",
                RollFileExistsBehavior.Increment,
                RollInterval.Day);

            foreach (var i in Enumerable.Range(1, 3)) writer.WriteCsvLine(Enumerable.Range(1, 8).Select(x => rsg.Generate(3, 6)).ToArray());
            foreach (var i in Enumerable.Range(1, 3))
                writer.WriteCsvLine(
                    Enumerable.Range(1, 8)
                        .Select(x => rsg.Generate(2) + Environment.NewLine + rsg.Generate(1, 2))
                        .ToArray());
            writer.Flush();
        }
    }
}