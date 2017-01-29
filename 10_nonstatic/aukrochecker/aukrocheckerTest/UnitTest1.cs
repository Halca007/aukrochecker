using System;
using System.IO.IsolatedStorage;
using aukrochecker.MiningEngine;
using NUnit.Framework;

namespace aukrocheckerTest
{
    [TestFixture]
    public class UnitTest1
    {

        public void CheckAsserts(string value)
        {
            Assert.IsNotNull(value);
        }




        [TestCase("yes")]
        public void TestMethod1(string value)
        {
           Storage myStorage = new Storage(new Core());
           CheckAsserts(value);
        }
    }
}
