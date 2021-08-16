﻿using Blackboard.Core;
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
        public void TestBasicParses_TypedInput() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 2, B = 3;",
                "in bool C = true;",
                "",
                "namespace D {",
                "   in double E = 3.14;",
                "   in int F, G;",
                "   in bool H;",
                "   in double I;",
                "}");
            driver.CheckValue(2,     "A");
            driver.CheckValue(3,     "B");
            driver.CheckValue(true,  "C");
            driver.CheckValue(3.14,  "D", "E");
            driver.CheckValue(0,     "D", "F");
            driver.CheckValue(0,     "D", "G");
            driver.CheckValue(false, "D", "H");
            driver.CheckValue(0.0,   "D", "I");
        }

        [TestMethod]
        public void TestBasicParses_VarInput() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in A = 2, B = 3;",
                "in C = true;",
                "",
                "namespace D {",
                "   in E = 3.14;",
                "}");
            driver.CheckValue(2, "A");
            driver.CheckValue(3, "B");
            driver.CheckValue(true, "C");
            driver.CheckValue(3.14, "D", "E");
        }

        [TestMethod]
        public void TestBasicParses_DoubleLiteral() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in double A = 3.0;",
                "in double B = 0.003;",
                "in double C = .003;",
                "in double D = 3.0e-3;",
                "in double E = 3e-3;",
                "in double F = .3e-2;",
                "in double G = 0.3e-2;",
                "in double H = 3;",
                "in double I = 0;",
                "in double J = 0.0;",
                "in double K = 1.0;",
                "in double L = 0e-5;",
                "in double M = 28.0;");
            driver.CheckValue(3.0,   "A");
            driver.CheckValue(0.003, "B");
            driver.CheckValue(0.003, "C");
            driver.CheckValue(0.003, "E");
            driver.CheckValue(0.003, "F");
            driver.CheckValue(0.003, "G");
            driver.CheckValue(3.0,   "H");
            driver.CheckValue(0.0,   "I");
            driver.CheckValue(0.0,   "J");
            driver.CheckValue(1.0,   "K");
            driver.CheckValue(0.0,   "L");
            driver.CheckValue(28.0,  "M");
        }

        [TestMethod]
        public void TestBasicParses_LiteralMath() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in double A = 3.0 + 0.07 * 2;",
                "in double B = floor(A), C = round(A), D = round(A, 1);",
                "in double E = (B ** C) / 2;",
                "in double F = -E + -3;");
            driver.CheckValue(  3.14, "A");
            driver.CheckValue(  3.0,  "B");
            driver.CheckValue(  3.0,  "C");
            driver.CheckValue(  3.1,  "D");
            driver.CheckValue( 13.5,  "E");
            driver.CheckValue(-16.5,  "F");
        }

        [TestMethod]
        public void TestBasicParses_ModRemAndStrings() {
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.math.ieeeremainder?view=net-5.0
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in string A = (  3.0 %%  2.0) + ', ' + (  3.0 %  2.0);",
                "in string B = (  4.0 %%  2.0) + ', ' + (  4.0 %  2.0);",
                "in string C = ( 10.0 %%  3.0) + ', ' + ( 10.0 %  3.0);",
                "in string D = ( 11.0 %%  3.0) + ', ' + ( 11.0 %  3.0);",
                "in string E = ( 27.0 %%  4.0) + ', ' + ( 27.0 %  4.0);",
                "in string F = ( 28.0 %%  5.0) + ', ' + ( 28.0 %  5.0);",
                "in string G = ( 17.8 %%  4.0) + ', ' + ( 17.8 %  4.0);",
                "in string H = ( 17.8 %%  4.1) + ', ' + ( 17.8 %  4.1);",
                "in string I = (-16.3 %%  4.1) + ', ' + (-16.3 %  4.1);",
                "in string J = ( 17.8 %% -4.1) + ', ' + ( 17.8 % -4.1);",
                "in string K = (-17.8 %% -4.1) + ', ' + (-17.8 % -4.1);");
            driver.CheckValue("-1, 1", "A");
            driver.CheckValue("0, 0",  "B");
            driver.CheckValue("1, 1",  "C");
            driver.CheckValue("-1, 2", "D");
            driver.CheckValue("-1, 3", "E");
            driver.CheckValue("-2, 3", "F");
            driver.CheckValue("1.8000000000000007, 1.8000000000000007",   "G");
            driver.CheckValue("1.4000000000000021, 1.4000000000000021",   "H");
            driver.CheckValue("0.09999999999999787, -4.000000000000002",  "I");
            driver.CheckValue("1.4000000000000021, 1.4000000000000021",   "J");
            driver.CheckValue("-1.4000000000000021, -1.4000000000000021", "K");
        }

        /*
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
        */

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
