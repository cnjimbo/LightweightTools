using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.CompilerServices;
using System.Linq;

namespace TSharp.DatabaseLog.EF6.Tests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {

            object a = 23;
            foreach (var i in Enumerable.Range(1, 20))
                Console.WriteLine(RuntimeHelpers.GetHashCode(i));
        }
    }
}
