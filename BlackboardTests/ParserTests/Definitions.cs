using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using S = System;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Definitions {

        [TestMethod]
        public void TestBasicParses_IntIntSum() {
            Driver driver = new();
            driver.ReadCommit(
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

        [TestMethod]
        public void TestBasicParses_IntDoubleSum() {
            Driver driver = new(addFuncs: false, addConsts: false);
            driver.ReadCommit(
                "in int A = 2;",
                "in double B = 3.0;",
                "double C := A + B;");
            driver.CheckGraphString(
                "Global: Namespace{",
                "  A: Input<int>[2],",
                "  B: Input<double>[3],",
                "  C: Sum<double>(Implicit<double>(A[2]), B[3])",
                "}");

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
            driver.ReadCommit(
                "in int A = 2;",
                "double B := A;",
                "string C := B;");

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
            driver.ReadCommit(
                "in A = 2;",
                "in B = 3;",
                "maxA := 3;",
                "C := A <= maxA && A > B ? 1 : 0;");

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
            driver.ReadCommit(
                "in int A = 0x0F;",
                "int shift := 1;",
                "int B := (A | 0x10) & 0x15;",
                "int C := B << shift;",
                "int D := ~C;");
            driver.CheckPending("asdf");

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
            driver.ReadCommit(
                "in int A = 0x03;",
                "bool B := A & 0x01 != 0;",
                "bool C := A & 0x02 != 0;",
                "bool D := A & 0x04 != 0;",
                "bool E := A & 0x08 != 0;",
                "bool F := B & !C ^ (D | E);");

            driver.CheckValue(0x3, "A");
            driver.CheckValue(true, "B");
            driver.CheckValue(true, "C");
            driver.CheckValue(false, "D");
            driver.CheckValue(false, "E");
            driver.CheckValue(false, "F");

            driver.SetInt(0x5, "A");
            driver.CheckEvaluate(
                "Start(Pending: 6)",
                "  Eval(0): A: Input<int>[5]",
                "  Eval(1): BitwiseAnd<int>[1](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[4](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(2): B: NotEqual<bool>[True](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(2): C: NotEqual<bool>[False](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(2): D: NotEqual<bool>[True](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(2): E: NotEqual<bool>[False](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(3): Not<bool>[True](C)",
                "  Eval(3): Or<bool>[True](D, E)",
                "  Eval(4): And<bool>[True](B, Not<bool>(C))",
                "  Eval(5): F: Xor<bool>[False](And<bool>(B, Not<bool>), Or<bool>(D, E))",
                "End(Provoked: 0)");
            driver.CheckValue(false, "F");

            driver.SetInt(0x4, "A");
            driver.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): A: Input<int>[4]",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[4](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(2): B: NotEqual<bool>[False](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(4): And<bool>[False](B, Not<bool>(C))",
                "  Eval(5): F: Xor<bool>[True](And<bool>(B, Not<bool>), Or<bool>(D, E))",
                "End(Provoked: 0)");
            driver.CheckValue(true, "F");

            driver.SetInt(0x8, "A");
            driver.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): A: Input<int>[8]",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[8](A, Literal<int>)",
                "  Eval(2): D: NotEqual<bool>[False](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(2): E: NotEqual<bool>[True](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(3): Or<bool>[True](D, E)",
                "End(Provoked: 0)");
            driver.CheckValue(true, "F");

            driver.SetInt(0xF, "A");
            driver.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): A: Input<int>[15]",
                "  Eval(1): BitwiseAnd<int>[1](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[2](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[4](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[8](A, Literal<int>)",
                "  Eval(2): B: NotEqual<bool>[True](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(2): C: NotEqual<bool>[True](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(2): D: NotEqual<bool>[True](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                // E was not checked because BitwiseAnd<int>[8] didn't change.
                "  Eval(3): Not<bool>[False](C)",
                "  Eval(3): Or<bool>[True](D, E)",
                "  Eval(4): And<bool>[False](B, Not<bool>(C))",
                "End(Provoked: 0)");
            driver.CheckValue(true, "F");

            driver.SetInt(0x5, "A");
            driver.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): A: Input<int>[5]",
                "  Eval(1): BitwiseAnd<int>[1](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[4](A, Literal<int>)",
                "  Eval(1): BitwiseAnd<int>[0](A, Literal<int>)",
                "  Eval(2): C: NotEqual<bool>[False](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(2): E: NotEqual<bool>[False](BitwiseAnd<int>(A, Literal<int>), Literal<int>)",
                "  Eval(3): Not<bool>[True](C)",
                "  Eval(3): Or<bool>[True](D, E)",
                "  Eval(4): And<bool>[True](B, Not<bool>(C))",
                "  Eval(5): F: Xor<bool>[False](And<bool>(B, Not<bool>), Or<bool>(D, E))",
                "End(Provoked: 0)");
            driver.CheckValue(false, "F");
        }

        [TestMethod]
        public void TestBasicParses_Trigger() {
            Driver driver = new();
            driver.ReadCommit(
                "in trigger A;",
                "in trigger B = true;",
                "C := A | B;",
                "D := A & B;",
                "E := C ^ D;");

            driver.Provoke("A");
            driver.CheckProvoked(true, "A");
            driver.CheckProvoked(true, "B"); // this was created provoked
            driver.CheckEvaluate(
                "Start(Pending: 5)",
                "  Eval(0): A: Input<trigger>[provoked]",
                "  Eval(0): B: Input<trigger>[provoked]",
                "  Eval(1): C: Any<trigger>[provoked](A, B)",
                "  Eval(1): D: All<trigger>[provoked](A, B)",
                "  Eval(2): E: OnlyOne<trigger>(C, D)",
                "End(Provoked: 4)");

            driver.Provoke("A");
            driver.CheckProvoked(true, "A");
            driver.CheckProvoked(false, "B");
            driver.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): A: Input<trigger>[provoked]",
                "  Eval(1): C: Any<trigger>[provoked](A, B)",
                "  Eval(1): D: All<trigger>(A, B)",
                "  Eval(2): E: OnlyOne<trigger>[provoked](C, D)",
                "End(Provoked: 3)");

            driver.Provoke("B");
            driver.CheckProvoked(false, "A");
            driver.CheckProvoked(true, "B");
            driver.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): B: Input<trigger>[provoked]",
                "  Eval(1): C: Any<trigger>[provoked](A, B)",
                "  Eval(1): D: All<trigger>(A, B)",
                "  Eval(2): E: OnlyOne<trigger>[provoked](C, D)",
                "End(Provoked: 3)");

            driver.CheckProvoked(false, "A");
            driver.CheckProvoked(false, "B");
            driver.CheckEvaluate(
                "Start(Pending: 0)",
                "End(Provoked: 0)");
        }

        [TestMethod]
        public void TestBasicParses_ExplicitCasts() {
            Driver driver = new(addConsts: false);
            driver.ReadCommit(
                "in double A = 1.2;",
                "B := (int)A;",     // Explicit
                "C := (string)A;",  // Implicit
                "D := (double)A;"); // Inheritance
            driver.CheckGraphString(
                "Global: Namespace{",
                "  A: Input<double>[1.2],",
                "  B: Explicit<int>(D[1.2]),",
                "  C: Implicit<string>(D[1.2]),",
                "  D: Input<double>[1.2]",
                "}");

            driver.CheckValue( 1.2,  "A");
            driver.CheckValue( 1,    "B");
            driver.CheckValue("1.2", "C");
            driver.CheckValue( 1.2,  "D");

            driver.SetDouble(42.9, "A");
            driver.Evaluate();
            driver.CheckValue( 42.9,  "A");
            driver.CheckValue( 42,    "B");
            driver.CheckValue("42.9", "C");
            driver.CheckValue( 42.9,  "D");
        }

        [TestMethod]
        public void TestBasicParses_ProvokingTriggers() {
            Driver driver = new();
            driver.ReadCommit(
                "in trigger A;",
                "in trigger B;",
                "C := A & B;",
                "in D = 3;");
            driver.CheckProvoked(false, "A");
            driver.CheckProvoked(false, "B");
            driver.CheckProvoked(false, "C");
            driver.CheckValue(3, "D");
            driver.CheckEvaluate(
                "Start(Pending: 4)",
                "  Eval(0): A: Input<trigger>", // Evaluates because they are new
                "  Eval(0): B: Input<trigger>",
                "  Eval(0): D: Input<int>[3]",
                "  Eval(1): C: All<trigger>(A, B)",
                "End(Provoked: 0)");

            driver.ReadCommit(
                "->A;",
                "D = 5;");
            driver.CheckProvoked(true, "A");
            driver.CheckProvoked(false, "B");
            driver.CheckProvoked(false, "C");
            driver.CheckValue(5, "D");
            driver.CheckEvaluate(
                "Start(Pending: 2)",
                "  Eval(0): A: Input<trigger>[provoked]",
                "  Eval(0): D: Input<int>[5]",
                "  Eval(1): C: All<trigger>(A, B)",
                "End(Provoked: 1)");

            driver.ReadCommit(
                "D > 3 -> A;",
                "A -> B;");
            driver.CheckProvoked(true, "A");
            driver.CheckProvoked(true, "B");
            driver.CheckProvoked(false, "C");
            driver.CheckValue(5, "D");
            driver.CheckEvaluate(
                "Start(Pending: 2)",
                "  Eval(0): A: Input<trigger>[provoked]",
                "  Eval(0): B: Input<trigger>[provoked]",
                "  Eval(1): C: All<trigger>[provoked](A, B)",
                "End(Provoked: 3)");

            driver.ReadCommit(
                "false -> A;",
                "D < -1 -> B;");
            driver.CheckProvoked(false, "A");
            driver.CheckProvoked(false, "B");
            driver.CheckProvoked(false, "C");
            driver.CheckValue(5, "D");
            driver.CheckEvaluate(
                "Start(Pending: 2)",
                "  Eval(0): A: Input<trigger>",
                "  Eval(0): B: Input<trigger>",
                "End(Provoked: 0)");
        }
    }
}
