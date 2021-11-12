﻿using Blackboard.Core;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using S = System;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class ParserTests {

        static private void checkException(S.Action hndl, params string[] exp) {
            S.Exception ex = Assert.ThrowsException<Exception>(hndl);
            List<string> messages = new();
            while (ex != null) {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            Assert.AreEqual(exp.Join("\n"), messages.Join("\n"));
        }

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
            parser.Commit();

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
                "   in var F = 0, G = 0.0, H = false;",
                "}");
            parser.Commit();

            driver.CheckValue(2,     "A");
            driver.CheckValue(3,     "B");
            driver.CheckValue(true,  "C");
            driver.CheckValue(3.14,  "D", "E");
            driver.CheckValue(0,     "D", "F");
            driver.CheckValue(0.0,   "D", "G");
            driver.CheckValue(false, "D", "H");
        }

        [TestMethod]
        public void TestBasicParses_DoubleLiteral() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in double A = 3.0;",
                "in double B = 0.003;",
                "in double C = 3.0e-3;",
                "in double D = 3e-3;",
                "in double E = 0.3e-2;",
                "in double F = 3;",
                "in double G = 0;",
                "in double H = 0.0;",
                "in double I = 1.0;",
                "in double J = 0e-5;",
                "in double K = 28.0;");
            parser.Commit();

            driver.CheckValue(3.0,   "A");
            driver.CheckValue(0.003, "B");
            driver.CheckValue(0.003, "C");
            driver.CheckValue(0.003, "D");
            driver.CheckValue(0.003, "E");
            driver.CheckValue(3.0,   "F");
            driver.CheckValue(0.0,   "G");
            driver.CheckValue(0.0,   "H");
            driver.CheckValue(1.0,   "I");
            driver.CheckValue(0.0,   "J");
            driver.CheckValue(28.0,  "K");
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
            parser.Commit();

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
            parser.Commit();

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

        [TestMethod]
        public void TestBasicParses_Assignment() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 2, B = 5;",
                "in int C = A = 8;",
                "B = A = 14;",
                "A = 6;");
            parser.Commit();

            driver.CheckValue( 6, "A");
            driver.CheckValue(14, "B");
            driver.CheckValue( 8, "C");
        }

        [TestMethod]
        public void TestBasicParses_NamespaceAssignment() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "namespace X {",
                "   in int a = 2;",
                "   in int b = 3;",
                "   in int c = 4;",
                "   namespace Y {",
                "      in int d = 5;",
                "      in int e = 6;",
                "      in int f = 7;",
                "      c = 444;",
                "      f = 777;",
                "   }",
                "   Y.e = 666;",
                "   b = 333;",
                "}",
                "X.Y.d = 555;",
                "X.a = 222;");
            parser.Commit();

            driver.CheckValue(222, "X", "a");
            driver.CheckValue(333, "X", "b");
            driver.CheckValue(444, "X", "c");
            driver.CheckValue(555, "X", "Y", "d");
            driver.CheckValue(666, "X", "Y", "e");
            driver.CheckValue(777, "X", "Y", "f");
        }

        [TestMethod]
        public void TestBasicParses_DoubleToIntAssignError() {
            Driver driver = new();
            Parser parser = new(driver);
            checkException(() => {
                parser.Read("in int A = 3.14;");
            }, "Error occurred while parsing input code.",
               "May not assign the value to that type of input.",
               "[Location: Unnamed:1, 15, 15]",
               "[Input Type: int]",
               "[Value Type: double]");
        }

        [TestMethod]
        public void TestBasicParses_IntIntSum() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 2;",
                "in int B = 3;",
                "int C := A + B;");
            parser.Commit();

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

        [TestMethod]
        public void TestBasicParses_IntDoubleSum() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 2;",
                "in double B = 3.0;",
                "double C := A + B;");
            parser.Commit();

            driver.CheckValue(2,   "A");
            driver.CheckValue(3.0, "B");
            driver.CheckValue(5.0, "C");

            driver.SetInt(7, "A");
            driver.Evaluate();
            driver.CheckValue( 7,   "A");
            driver.CheckValue( 3.0, "B");
            driver.CheckValue(10.0, "C");

            driver.SetDouble(1.23, "B");
            driver.Evaluate();
            driver.CheckValue(7,    "A");
            driver.CheckValue(1.23, "B");
            driver.CheckValue(8.23, "C");
        }

        [TestMethod]
        public void TestBasicParses_IntDoubleImplicitCast() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 2;",
                "double B := A;",
                "string C := B;");
            parser.Commit();

            driver.CheckValue(2,   "A");
            driver.CheckValue(2.0, "B");
            driver.CheckValue("2", "C");

            driver.SetInt(42, "A");
            driver.Evaluate();
            driver.CheckValue(42,   "A");
            driver.CheckValue(42.0, "B");
            driver.CheckValue("42", "C");
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
            parser.Commit();

            driver.CheckValue(2, "A");
            driver.CheckValue(3, "B");
            driver.CheckValue(0, "C");

            driver.SetInt(7, "A");
            driver.Evaluate();
            driver.CheckValue(0, "C");

            driver.SetInt(2, "A");
            driver.SetInt(-1, "B");
            driver.Evaluate();
            driver.CheckValue(1, "C");
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
            parser.Commit();

            driver.CheckValue( 0x0F, "A");
            driver.CheckValue( 0x15, "B");
            driver.CheckValue( 0x2A, "C");
            driver.CheckValue(-0x2B, "D");

            driver.SetInt(0x44, "A");
            driver.Evaluate();
            driver.CheckValue( 0x14, "B");
            driver.CheckValue( 0x28, "C");
            driver.CheckValue(-0x29, "D");
        }

        [TestMethod]
        public void TestBasicParses_SomeBooleanMath() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in int A = 0x03;",
                "bool B := A & 0x01 != 0;",
                "bool C := A & 0x02 != 0;",
                "bool D := A & 0x04 != 0;",
                "bool E := A & 0x08 != 0;",
                "bool F := B & !C ^ (D | E);");
            parser.Commit();

            driver.CheckValue(0x3, "A");
            driver.CheckValue(true, "B");
            driver.CheckValue(true, "C");
            driver.CheckValue(false, "D");
            driver.CheckValue(false, "E");
            driver.CheckValue(false, "F");

            driver.SetInt(0x5, "A");
            driver.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): Input<int>[5]",
                "  Eval(1): BitwiseAnd<int>(Input<int>[5], Literal<int>[1])",
                "  Eval(1): BitwiseAnd<int>(Input<int>[5], Literal<int>[2])",
                "  Eval(1): BitwiseAnd<int>(Input<int>[5], Literal<int>[4])",
                "  Eval(1): BitwiseAnd<int>(Input<int>[5], Literal<int>[8])",
                "  Eval(2): NotEqual<bool>(BitwiseAnd<int>(Input<int>[5], Literal<int>[2]), Literal<int>[0])",
                "  Eval(2): NotEqual<bool>(BitwiseAnd<int>(Input<int>[5], Literal<int>[4]), Literal<int>[0])",
                "  Eval(3): Not<bool>(NotEqual<bool>(BitwiseAnd<int>[0], Literal<int>[0]))",
                "  Eval(3): Or<bool>(NotEqual<bool>(BitwiseAnd<int>[4], Literal<int>[0]), NotEqual<bool>(BitwiseAnd<int>[0], Literal<int>[0]))",
                "  Eval(4): And<bool>(NotEqual<bool>(BitwiseAnd<int>[1], Literal<int>[0]), Not<bool>(NotEqual<bool>[False]))",
                "  Eval(5): Xor<bool>(And<bool>(NotEqual<bool>[True], Not<bool>[True]), Or<bool>(NotEqual<bool>[True], NotEqual<bool>[False]))",
                "End(Provoked: 0)");
            driver.CheckValue(false, "F");

            driver.SetInt(0x4, "A");
            driver.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): Input<int>[4]",
                "  Eval(1): BitwiseAnd<int>(Input<int>[4], Literal<int>[1])",
                "  Eval(1): BitwiseAnd<int>(Input<int>[4], Literal<int>[2])",
                "  Eval(1): BitwiseAnd<int>(Input<int>[4], Literal<int>[4])",
                "  Eval(1): BitwiseAnd<int>(Input<int>[4], Literal<int>[8])",
                "  Eval(2): NotEqual<bool>(BitwiseAnd<int>(Input<int>[4], Literal<int>[1]), Literal<int>[0])",
                "  Eval(4): And<bool>(NotEqual<bool>(BitwiseAnd<int>[0], Literal<int>[0]), Not<bool>(NotEqual<bool>[False]))",
                "  Eval(5): Xor<bool>(And<bool>(NotEqual<bool>[False], Not<bool>[True]), Or<bool>(NotEqual<bool>[True], NotEqual<bool>[False]))",
                "End(Provoked: 0)");
            driver.CheckValue(true, "F");

            driver.SetInt(0x8, "A");
            driver.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): Input<int>[8]",
                "  Eval(1): BitwiseAnd<int>(Input<int>[8], Literal<int>[1])",
                "  Eval(1): BitwiseAnd<int>(Input<int>[8], Literal<int>[2])",
                "  Eval(1): BitwiseAnd<int>(Input<int>[8], Literal<int>[4])",
                "  Eval(1): BitwiseAnd<int>(Input<int>[8], Literal<int>[8])",
                "  Eval(2): NotEqual<bool>(BitwiseAnd<int>(Input<int>[8], Literal<int>[4]), Literal<int>[0])",
                "  Eval(2): NotEqual<bool>(BitwiseAnd<int>(Input<int>[8], Literal<int>[8]), Literal<int>[0])",
                "  Eval(3): Or<bool>(NotEqual<bool>(BitwiseAnd<int>[0], Literal<int>[0]), NotEqual<bool>(BitwiseAnd<int>[8], Literal<int>[0]))",
                "End(Provoked: 0)");
            driver.CheckValue(true, "F");
        }

        [TestMethod]
        public void TestBasicParses_Trigger() {
            Driver driver = new();
            Parser parser = new(driver);
            parser.Read(
                "in trigger A;",
                "in trigger B = true;",
                "C := A | B;",
                "D := !A & B;");
            parser.Commit();

            driver.Provoke("A");
            driver.CheckEvaluate();

        }
    }
}
