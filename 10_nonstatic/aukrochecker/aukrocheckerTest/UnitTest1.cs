using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
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
        public void TestMethod1(String reference)
        {
           //test if given storage factory is ok
           Assert.IsNotNull(reference);
        }
    }

    [TestFixture]
    public class DerivUnitTest<T> : UnitTest1
    {
        private int numOfIteration;
        private Storage refToHiddenStorage;

        public DerivUnitTest(int numOfIteration)
        {
            this.numOfIteration = numOfIteration;
        }


        public int Multiplier(int a, int b)
        {
            return a * b;
        }

        // no test cases generative test
        [TestCase(12,45,Author = "Jakub", Category = "alpha", Description = "Check if multiplier works well")]
        [TestCase(12,50 ,Author = "Jakub", Category = "alpha", Description = "Check if multiplier works well for zeros")]
        void Multiply(int a, int b)
        {
            for (int i = 0; i < numOfIteration; i++)
            {
                Assert.AreEqual(a*b,Multiplier(a,b));
            }
        }


        // no test cases generative test
        [Test]
        void NotNullTest(T givenstorage)
        {
            for (int i=0; i< numOfIteration; i++) { 
                Assert.IsNotNull(givenstorage);
            }
        }

        

      

        [TestCase(ExpectedResult = 4, Description = "await test")]
        public async Task<int> TestAdd()
        {
            await Calculation();
        return 2 + 2;
        }

        public int Calculation()
        {
            return 4;
        }


        public Storage StorageFactory()
        {
            return new Storage(new Core());

        }



        // no test cases generative test
        [TestCase()]
        void NotNullTestParam(T givenstorage)
        {
            for (int i = 0; i < numOfIteration; i++)
            {
                Assert.IsNotNull(givenstorage);
            }
        }

    }
}
