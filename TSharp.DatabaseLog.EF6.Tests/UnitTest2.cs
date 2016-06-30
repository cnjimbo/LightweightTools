namespace TSharp.DatabaseLog.EF6.Tests
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            object a = 23;
            foreach (var i in Enumerable.Range(1, 20)) Console.WriteLine(RuntimeHelpers.GetHashCode(i));
        }

        [TestMethod]
        public void RandomStringGeneratorTest()
        {
            var rsg = new RandomStringGenerator();
            foreach (var s in Enumerable.Range(1, 30))
            {
                Console.WriteLine(rsg.Generate(s % 10 + 1));
            }
        }
    }
}