using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using LangtonLoop;

namespace UnitTestLibrary
{
    [TestClass]
    public class UnitTestRule
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
