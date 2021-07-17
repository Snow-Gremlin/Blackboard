using Blackboard.Core;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class ParserTests {

        static private void checkValue<T>(Driver driver, string name, T exp) {
            T value = driver.GetValue<T>(name);
            Assert.AreEqual(exp, value, "Checking value " + name);
        }

        [TestMethod]
        public void TestBasicParses_IntIntSum() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 2;",
                "in int B = 3;",
                "int C := A + B;");
            checkValue(driver, "A", 2);
            checkValue(driver, "B", 3);
            checkValue(driver, "C", 5);

            driver.SetValue("A", 7);
            driver.Evalate();
            checkValue(driver, "A", 7);
            checkValue(driver, "B", 3);
            checkValue(driver, "C", 10);

            driver.SetValue("B", 1);
            driver.Evalate();
            checkValue(driver, "A", 7);
            checkValue(driver, "B", 1);
            checkValue(driver, "C", 8);
        }

        [TestMethod]
        public void TestBasicParses_IntFloatSum() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 2;",
                "in float B = 3.0;",
                "float C := A + B;");
            checkValue(driver, "A", 2);
            checkValue(driver, "B", 3.0);
            checkValue(driver, "C", 5.0);

            driver.SetValue("A", 7);
            driver.Evalate();
            checkValue(driver, "A", 7);
            checkValue(driver, "B", 3.0);
            checkValue(driver, "C", 10.0);

            driver.SetValue("B", 1.23);
            driver.Evalate();
            checkValue(driver, "A", 7);
            checkValue(driver, "B", 1.23);
            checkValue(driver, "C", 8.23);
        }

        [TestMethod]
        public void TestBasicParses_FloatToIntAssignError() {
            Driver driver = new();
            Parser parser = new(driver);
            Assert.AreEqual(Assert.ThrowsException<Exception>(() => {
                parser.Read("in int A = 3.14;");
            }).Message, "float can not be assigned to int.");
        }

        [TestMethod]
        public void TestBasicParses_FloatToIntAssignError2() {
            Driver driver = new();
            Parser parser = new(driver);
            Assert.AreEqual(Assert.ThrowsException<Exception>(() => {
                parser.Read("in int A = 3.14;");
            }).Message, "float can not be assigned to int.");
        }
    }
}
