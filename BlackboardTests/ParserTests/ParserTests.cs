using Blackboard.Core;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using S = System;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class ParserTests {
        
        //static private void checkException(S.Action hndl, string exp) =>
        //  Assert.AreEqual(Assert.ThrowsException<Exception>(hndl).Message, exp);

        [TestMethod]
        public void TestBasicParses_IntIntSum() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 2;",
                "in int B = 3;",
                "int C := A + B;");
            driver.CheckValue(2, "A");
            driver.CheckValue(3, "B");
            driver.CheckValue(5, "C");

            driver.SetInt(7, "A");
            driver.Evaluate();
            driver.CheckValue( 7, "A");
            driver.CheckValue( 3, "B");
            driver.CheckValue(10, "C");

            driver.SetInt(1, "B");
            driver.Evaluate();
            driver.CheckValue(7, "A");
            driver.CheckValue(1, "B");
            driver.CheckValue(8, "C");
        }

        /*
        [TestMethod]
        public void TestBasicParses_IntDoubleSum() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 2;",
                "in double B = 3.0;",
                "double C := A + B;");
            checkValue(driver, "A", 2);
            checkValue(driver, "B", 3.0);
            checkValue(driver, "C", 5.0);

            driver.SetValue("A", 7);
            driver.Evaluate();
            checkValue(driver, "A", 7);
            checkValue(driver, "B", 3.0);
            checkValue(driver, "C", 10.0);

            driver.SetValue("B", 1.23);
            driver.Evaluate();
            checkValue(driver, "A", 7);
            checkValue(driver, "B", 1.23);
            checkValue(driver, "C", 8.23);
        }

        [TestMethod]
        public void TestBasicParses_DoubleToIntAssignError() {
            Driver driver = new();
            Parser parser = new(driver);
            checkException(() => {
                parser.Read("in int A = 3.14;");
            }, "Can not assign a double to an int.");
        }

        [TestMethod]
        public void TestBasicParses_IntIntCompare() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in A = 2;",
                "in B = 3;",
                "maxA := 3;",
                "C := A <= maxA && A > B ? 1 : 0;");
            checkValue(driver, "A", 2);
            checkValue(driver, "B", 3);
            checkValue(driver, "C", 0);

            driver.SetValue("A", 7);
            driver.Evaluate();
            checkValue(driver, "C", 0);

            driver.SetValue("A", 2);
            driver.SetValue("B", -1);
            driver.Evaluate();
            checkValue(driver, "C", 1);
        }

        [TestMethod]
        public void TestBasicParses_Bitwise() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 0x0F;",
                "int shift := 1;",
                "int B := (A | 0x10) & 0x15;",
                "int C := B << shift;",
                "int D := ~C;");
            checkValue(driver, "A", 0x0F);
            checkValue(driver, "B", 0x15);
            checkValue(driver, "C", 0x2A);
            checkValue(driver, "D", -0x2B);

            driver.SetValue("A", 0x44);
            driver.Evaluate();
            checkValue(driver, "B", 0x14);
            checkValue(driver, "C", 0x28);
            checkValue(driver, "D", -0x29);
        }

        [TestMethod]
        public void TestBasicParses_SomeBooleanMath() {
            Driver driver = new(new StringWriter());
            Parser parser = new(driver);
            parser.Read(
                "in int A = 0x03;",
                "bool B := A & 0x01 != 0;",
                "bool C := A & 0x02 != 0;",
                "bool D := A & 0x04 != 0;",
                "bool E := A & 0x08 != 0;",
                "bool F := B & !C ^ (D | E);");
            checkValue(driver, "A", 0x3);
            checkValue(driver, "B", true);
            checkValue(driver, "C", true);
            checkValue(driver, "D", false);
            checkValue(driver, "E", false);
            checkValue(driver, "F", false);

            driver.SetValue("A", 0x5);
            driver.Evalate();
            checkLog(driver.Log as StringWriter,
                "Eval(1): Global.A",
                "Eval(2): BitwiseAnd(Global.A, 1)",
                "Eval(2): BitwiseAnd(Global.A, 2)",
                "Eval(2): BitwiseAnd(Global.A, 4)",
                "Eval(2): BitwiseAnd(Global.A, 8)",
                "Eval(3): NotEqual(BitwiseAnd(Global.A, 2), 0)",
                "Eval(3): NotEqual(BitwiseAnd(Global.A, 4), 0)",
                "Eval(4): Global.C",
                "Eval(4): Global.D",
                "Eval(5): Not(Global.C)",
                "Eval(5): Or(Global.D, Global.E)",
                "Eval(6): And(Global.B, Not(Global.C))",
                "Eval(7): Xor(And(Global.B, Not(Global.C)), Or(Global.D, Global.E))");
            checkValue(driver, "F", false);

            driver.Log = new StringWriter();
            driver.SetValue("A", 0x4);
            driver.Evalate();
            checkLog(driver.Log as StringWriter,
                "Eval(1): Global.A",
                "Eval(2): BitwiseAnd(Global.A, 1)",
                "Eval(2): BitwiseAnd(Global.A, 2)",
                "Eval(2): BitwiseAnd(Global.A, 4)",
                "Eval(2): BitwiseAnd(Global.A, 8)",
                "Eval(3): NotEqual(BitwiseAnd(Global.A, 1), 0)",
                "Eval(4): Global.B",
                "Eval(6): And(Global.B, Not(Global.C))",
                "Eval(7): Xor(And(Global.B, Not(Global.C)), Or(Global.D, Global.E))",
                "Eval(8): Global.F");
            checkValue(driver, "F", true);

            driver.Log = new StringWriter();
            driver.SetValue("A", 0x8);
            driver.Evalate();
            checkLog(driver.Log as StringWriter,
                "Eval(1): Global.A",
                "Eval(2): BitwiseAnd(Global.A, 1)",
                "Eval(2): BitwiseAnd(Global.A, 2)",
                "Eval(2): BitwiseAnd(Global.A, 4)",
                "Eval(2): BitwiseAnd(Global.A, 8)",
                "Eval(3): NotEqual(BitwiseAnd(Global.A, 4), 0)",
                "Eval(3): NotEqual(BitwiseAnd(Global.A, 8), 0)",
                "Eval(4): Global.D",
                "Eval(4): Global.E",
                "Eval(5): Or(Global.D, Global.E)");
            checkValue(driver, "F", true);
        }
        */
    }
}
