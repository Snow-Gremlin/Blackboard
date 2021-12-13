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
            Slate slate = new();
            slate.ReadCommit(
                "in int A = 2;",
                "in int B = 3;",
                "int C := A + B;");

            slate.CheckValue(2, "A");
            slate.CheckValue(3, "B");
            slate.CheckValue(5, "C");

            slate.SetInt(7, "A");
            slate.Evaluate();
            slate.CheckValue( 7, "A");
            slate.CheckValue( 3, "B");
            slate.CheckValue(10, "C");

            slate.SetInt(1, "B");
            slate.Evaluate();
            slate.CheckValue(7, "A");
            slate.CheckValue(1, "B");
            slate.CheckValue(8, "C");
        }

        [TestMethod]
        public void TestBasicParses_IntDoubleSum() {
            Slate slate = new(addFuncs: false, addConsts: false);
            slate.ReadCommit(
                "in int A = 2;",
                "in double B = 3.0;",
                "double C := A + B;");
            slate.CheckGraphString(
                "Global: Namespace{",
                "  A: Input<int>[2],",
                "  B: Input<double>[3],",
                "  C: Sum<double>(Implicit<double>(A[2]), B[3])",
                "}");

            slate.CheckValue(2,   "A");
            slate.CheckValue(3.0, "B");
            slate.CheckValue(5.0, "C");

            slate.SetInt(7, "A");
            slate.Evaluate();
            slate.CheckValue( 7,   "A");
            slate.CheckValue( 3.0, "B");
            slate.CheckValue(10.0, "C");

            slate.SetDouble(1.23, "B");
            slate.Evaluate();
            slate.CheckValue(7,    "A");
            slate.CheckValue(1.23, "B");
            slate.CheckValue(8.23, "C");
        }

        [TestMethod]
        public void TestBasicParses_IntDoubleImplicitCast() {
            Slate slate = new();
            slate.ReadCommit(
                "in int A = 2;",
                "double B := A;",
                "string C := B;");

            slate.CheckValue(2,   "A");
            slate.CheckValue(2.0, "B");
            slate.CheckValue("2", "C");

            slate.SetInt(42, "A");
            slate.Evaluate();
            slate.CheckValue(42,   "A");
            slate.CheckValue(42.0, "B");
            slate.CheckValue("42", "C");
        }

        [TestMethod]
        public void TestBasicParses_IntIntCompare() {
            Slate slate = new();
            slate.ReadCommit(
                "in A = 2;",
                "in B = 3;",
                "maxA := 3;",
                "C := A <= maxA && A > B ? 1 : 0;");

            slate.CheckValue(2, "A");
            slate.CheckValue(3, "B");
            slate.CheckValue(0, "C");

            slate.SetInt(7, "A");
            slate.Evaluate();
            slate.CheckValue(0, "C");

            slate.SetInt(2, "A");
            slate.SetInt(-1, "B");
            slate.Evaluate();
            slate.CheckValue(1, "C");
        }

        [TestMethod]
        public void TestBasicParses_Bitwise() {
            Slate slate = new();
            slate.ReadCommit(
                "in int A = 0x0F;",
                "int shift := 1;",
                "int B := (A | 0x10) & 0x15;",
                "int C := B << shift;",
                "int D := ~C;");
            slate.CheckPending("asdf");

            slate.CheckValue( 0x0F, "A");
            slate.CheckValue( 0x15, "B");
            slate.CheckValue( 0x2A, "C");
            slate.CheckValue(-0x2B, "D");

            slate.SetInt(0x44, "A");
            slate.Evaluate();
            slate.CheckValue( 0x14, "B");
            slate.CheckValue( 0x28, "C");
            slate.CheckValue(-0x29, "D");
        }

        [TestMethod]
        public void TestBasicParses_SomeBooleanMath() {
            Slate slate = new();
            slate.ReadCommit(
                "in int A = 0x03;",
                "bool B := A & 0x01 != 0;",
                "bool C := A & 0x02 != 0;",
                "bool D := A & 0x04 != 0;",
                "bool E := A & 0x08 != 0;",
                "bool F := B & !C ^ (D | E);");

            slate.CheckValue(0x3, "A");
            slate.CheckValue(true, "B");
            slate.CheckValue(true, "C");
            slate.CheckValue(false, "D");
            slate.CheckValue(false, "E");
            slate.CheckValue(false, "F");

            slate.SetInt(0x5, "A");
            slate.CheckEvaluate(
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
            slate.CheckValue(false, "F");

            slate.SetInt(0x4, "A");
            slate.CheckEvaluate(
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
            slate.CheckValue(true, "F");

            slate.SetInt(0x8, "A");
            slate.CheckEvaluate(
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
            slate.CheckValue(true, "F");

            slate.SetInt(0xF, "A");
            slate.CheckEvaluate(
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
            slate.CheckValue(true, "F");

            slate.SetInt(0x5, "A");
            slate.CheckEvaluate(
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
            slate.CheckValue(false, "F");
        }

        [TestMethod]
        public void TestBasicParses_Trigger() {
            Slate slate = new();
            slate.ReadCommit(
                "in trigger A;",
                "in trigger B = true;",
                "C := A | B;",
                "D := A & B;",
                "E := C ^ D;");

            slate.Provoke("A");
            slate.CheckProvoked(true, "A");
            slate.CheckProvoked(true, "B"); // this was created provoked
            slate.CheckEvaluate(
                "Start(Pending: 5)",
                "  Eval(0): A: Input<trigger>[provoked]",
                "  Eval(0): B: Input<trigger>[provoked]",
                "  Eval(1): C: Any<trigger>[provoked](A, B)",
                "  Eval(1): D: All<trigger>[provoked](A, B)",
                "  Eval(2): E: OnlyOne<trigger>(C, D)",
                "End(Provoked: 4)");

            slate.Provoke("A");
            slate.CheckProvoked(true, "A");
            slate.CheckProvoked(false, "B");
            slate.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): A: Input<trigger>[provoked]",
                "  Eval(1): C: Any<trigger>[provoked](A, B)",
                "  Eval(1): D: All<trigger>(A, B)",
                "  Eval(2): E: OnlyOne<trigger>[provoked](C, D)",
                "End(Provoked: 3)");

            slate.Provoke("B");
            slate.CheckProvoked(false, "A");
            slate.CheckProvoked(true, "B");
            slate.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): B: Input<trigger>[provoked]",
                "  Eval(1): C: Any<trigger>[provoked](A, B)",
                "  Eval(1): D: All<trigger>(A, B)",
                "  Eval(2): E: OnlyOne<trigger>[provoked](C, D)",
                "End(Provoked: 3)");

            slate.CheckProvoked(false, "A");
            slate.CheckProvoked(false, "B");
            slate.CheckEvaluate(
                "Start(Pending: 0)",
                "End(Provoked: 0)");
        }

        [TestMethod]
        public void TestBasicParses_ExplicitCasts() {
            Slate slate = new(addConsts: false);
            slate.ReadCommit(
                "in double A = 1.2;",
                "B := (int)A;",     // Explicit
                "C := (string)A;",  // Implicit
                "D := (double)A;"); // Inheritance
            slate.CheckGraphString(
                "Global: Namespace{",
                "  A: Input<double>[1.2],",
                "  B: Explicit<int>(D[1.2]),",
                "  C: Implicit<string>(D[1.2]),",
                "  D: Input<double>[1.2]",
                "}");

            slate.CheckValue( 1.2,  "A");
            slate.CheckValue( 1,    "B");
            slate.CheckValue("1.2", "C");
            slate.CheckValue( 1.2,  "D");

            slate.SetDouble(42.9, "A");
            slate.Evaluate();
            slate.CheckValue( 42.9,  "A");
            slate.CheckValue( 42,    "B");
            slate.CheckValue("42.9", "C");
            slate.CheckValue( 42.9,  "D");
        }

        [TestMethod]
        public void TestBasicParses_ProvokingTriggers() {
            Slate slate = new();
            slate.ReadCommit(
                "in trigger A;",
                "in trigger B;",
                "C := A & B;",
                "in D = 3;");
            slate.CheckProvoked(false, "A");
            slate.CheckProvoked(false, "B");
            slate.CheckProvoked(false, "C");
            slate.CheckValue(3, "D");
            slate.CheckEvaluate(
                "Start(Pending: 4)",
                "  Eval(0): A: Input<trigger>", // Evaluates because they are new
                "  Eval(0): B: Input<trigger>",
                "  Eval(0): D: Input<int>[3]",
                "  Eval(1): C: All<trigger>(A, B)",
                "End(Provoked: 0)");

            slate.ReadCommit(
                "->A;",
                "D = 5;");
            slate.CheckProvoked(true, "A");
            slate.CheckProvoked(false, "B");
            slate.CheckProvoked(false, "C");
            slate.CheckValue(5, "D");
            slate.CheckEvaluate(
                "Start(Pending: 2)",
                "  Eval(0): A: Input<trigger>[provoked]",
                "  Eval(0): D: Input<int>[5]",
                "  Eval(1): C: All<trigger>(A, B)",
                "End(Provoked: 1)");

            slate.ReadCommit(
                "D > 3 -> A;",
                "A -> B;");
            slate.CheckProvoked(true, "A");
            slate.CheckProvoked(true, "B");
            slate.CheckProvoked(false, "C");
            slate.CheckValue(5, "D");
            slate.CheckEvaluate(
                "Start(Pending: 2)",
                "  Eval(0): A: Input<trigger>[provoked]",
                "  Eval(0): B: Input<trigger>[provoked]",
                "  Eval(1): C: All<trigger>[provoked](A, B)",
                "End(Provoked: 3)");

            slate.ReadCommit(
                "false -> A;",
                "D < -1 -> B;");
            slate.CheckProvoked(false, "A");
            slate.CheckProvoked(false, "B");
            slate.CheckProvoked(false, "C");
            slate.CheckValue(5, "D");
            slate.CheckEvaluate(
                "Start(Pending: 2)",
                "  Eval(0): A: Input<trigger>",
                "  Eval(0): B: Input<trigger>",
                "End(Provoked: 0)");
        }
    }
}
