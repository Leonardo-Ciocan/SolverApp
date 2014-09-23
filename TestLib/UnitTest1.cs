using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using CalculatorApp;
namespace TestLib
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckArithmetic()
        {
            Solver solver = new Solver();
            Assert.AreEqual(15, solver.EvaluateNested("3 * (2+3)"));
            Assert.AreEqual(75, solver.EvaluateNested("15 dollars * 5 people"));
        }
    }
}
