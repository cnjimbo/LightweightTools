using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSharp.TraceListeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TSharp.DatabaseLog.EF6;
namespace TSharp.TraceListeners.Tests
{
    [TestClass()]
    public class RollingFlatFileTraceListenerTests
    {
        [TestMethod()]
        public void RollingFlatFileTraceListenerTest()
        {
            RollingFlatFileTraceListener writer = new RollingFlatFileTraceListener("trace.log", null, null, 512,
                "HHmmss", "yyyyMMdd", RollFileExistsBehavior.Increment, RollInterval.Day);

            writer.WriteLine("rrrrrr");

            writer.Flush();
        }

        [TestMethod()]
        public void RollingFlatFileTraceListenerTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void WriteLineTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void WriteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void WriteCsvLineTest()
        {
            RandomStringGenerator rsg = new RandomStringGenerator(true, true, true, true);

            RollingFlatFileTraceListener writer = new RollingFlatFileTraceListener("trace.log", null, null, 512,
              "HHmmss", "yyyyMMdd", RollFileExistsBehavior.Increment, RollInterval.Day);

            foreach (var i in Enumerable.Range(1, 300))
                writer.WriteCsvLine(Enumerable.Range(1, 8).Select(x => rsg.Generate(3, 6)).ToArray());

            writer.Flush();

        }
    }
}
