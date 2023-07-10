using Blackboard.Core;
using Blackboard.Core.Extensions;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests;

[TestClass]
public class Definitions {

    [TestMethod]
    public void TestBasicParses_IntIntSum() {
        Slate slate = new();
        slate.Read(
            "in int A = 2;",
            "in int B = 3;",
            "int C := A + B;").
            Perform();

        slate.CheckValue(2, "A");
        slate.CheckValue(3, "B");
        slate.CheckValue(5, "C");

        slate.SetInt(7, "A");
        slate.PerformEvaluation();
        slate.CheckValue( 7, "A");
        slate.CheckValue( 3, "B");
        slate.CheckValue(10, "C");

        slate.SetInt(1, "B");
        slate.PerformEvaluation();
        slate.CheckValue(7, "A");
        slate.CheckValue(1, "B");
        slate.CheckValue(8, "C");
    }

    [TestMethod]
    public void TestBasicParses_IntDoubleSum() {
        Slate slate = new(addFuncs: false, addConsts: false);
        slate.Read(
            "in int A = 2;",
            "in double B = 3.0;",
            "double C := A + B;").
            Perform();
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Input<int>[2],",
            "  B: Input<double>[3],",
            "  C: Sum<double>[5](Implicit<double>(A[2]), B[3])",
            "}");

        slate.CheckValue(2,   "A");
        slate.CheckValue(3.0, "B");
        slate.CheckValue(5.0, "C");

        slate.SetInt(7, "A");
        slate.PerformEvaluation();
        slate.CheckValue( 7,   "A");
        slate.CheckValue( 3.0, "B");
        slate.CheckValue(10.0, "C");

        slate.SetDouble(1.23, "B");
        slate.PerformEvaluation();
        slate.CheckValue(7,    "A");
        slate.CheckValue(1.23, "B");
        slate.CheckValue(8.23, "C");
    }

    [TestMethod]
    public void TestBasicParses_IntDoubleImplicitCast() {
        Slate slate = new();
        slate.Read(
            "in int A = 2;",
            "double B := A;",
            "string C := B;").
            Perform();

        slate.CheckValue(2,   "A");
        slate.CheckValue(2.0, "B");
        slate.CheckValue("2", "C");

        slate.SetInt(42, "A");
        slate.PerformEvaluation();
        slate.CheckValue(42,   "A");
        slate.CheckValue(42.0, "B");
        slate.CheckValue("42", "C");
    }

    [TestMethod]
    public void TestBasicParses_IntIntCompare() {
        Slate slate = new();
        slate.Read(
            "in A = 2;",
            "in B = 3;",
            "maxA := 3;",
            "C := A <= maxA && A > B ? 1 : 0;").
            Perform();

        slate.CheckValue(2, "A");
        slate.CheckValue(3, "B");
        slate.CheckValue(0, "C");

        slate.SetInt(7, "A");
        slate.PerformEvaluation();
        slate.CheckValue(0, "C");

        slate.SetInt(2, "A");
        slate.SetInt(-1, "B");
        slate.PerformEvaluation();
        slate.CheckValue(1, "C");
    }

    [TestMethod]
    public void TestBasicParses_Bitwise() {
        Slate slate = new();
        slate.Read(
            "in int A = 0x0F;",
            "int shift := 1;",
            "int B := (A | 0x10) & 0x15;",
            "var C := B << shift;",
            "D := ~C;").
            Perform();

        slate.CheckValue( 0x0F, "A");
        slate.CheckValue( 0x15, "B");
        slate.CheckValue( 0x2A, "C");
        slate.CheckValue(-0x2B, "D");

        slate.SetInt(0x44, "A");
        slate.PerformEvaluation();
        slate.CheckValue( 0x14, "B");
        slate.CheckValue( 0x28, "C");
        slate.CheckValue(-0x29, "D");
    }

    [TestMethod]
    public void TestBasicParses_Bitwise_GroupDefine() {
        Slate slate = new();
        slate.Read(
            "in int A = 0x0F;",
            "define {",
            "   int shift = 1;",
            "   int B = (A | 0x10) & 0x15;",
            "   var C = B << shift;",
            "   D = ~C;",
            "}").
            Perform();

        slate.CheckValue( 0x0F, "A");
        slate.CheckValue( 0x15, "B");
        slate.CheckValue( 0x2A, "C");
        slate.CheckValue(-0x2B, "D");

        slate.SetInt(0x44, "A");
        slate.PerformEvaluation();
        slate.CheckValue( 0x14, "B");
        slate.CheckValue( 0x28, "C");
        slate.CheckValue(-0x29, "D");
    }
    
    [TestMethod]
    public void TestBasicParses_Bitwise_GroupDefineWithNamespace() {
        Slate slate = new();
        slate.Read(
            "namespace X {",
            "   in int A = 0x0F;",
            "   define {",
            "      int shift = 1;",
            "      namespace Y {",
            "         int B = (A | 0x10) & 0x15;",
            "         var C = B << shift;",
            "         D = ~C;",
            "      }",
            "   }",
            "}").
            Perform();

        slate.CheckValue( 0x0F, "X", "A");
        slate.CheckValue( 0x15, "X", "Y", "B");
        slate.CheckValue( 0x2A, "X", "Y", "C");
        slate.CheckValue(-0x2B, "X", "Y", "D");

        slate.SetInt(0x44, "X", "A");
        slate.PerformEvaluation();
        slate.CheckValue( 0x14, "X", "Y", "B");
        slate.CheckValue( 0x28, "X", "Y", "C");
        slate.CheckValue(-0x29, "X", "Y", "D");
    }

    [TestMethod]
    public void TestBasicParses_SomeBooleanMath() {
        Slate slate = new();
        slate.Read(
            "in int A = 0x03;",
            "bool B := A & 0x01 != 0;",
            "bool C := A & 0x02 != 0;",
            "bool D := A & 0x04 != 0;",
            "bool E := A & 0x08 != 0;",
            "bool F := B & !C ^ (D | E);").
            Perform();

        slate.CheckValue(0x3, "A");
        slate.CheckValue(true, "B");
        slate.CheckValue(true, "C");
        slate.CheckValue(false, "D");
        slate.CheckValue(false, "E");
        slate.CheckValue(false, "F");

        slate.SetInt(0x5, "A");
        slate.CheckEvaluate(
            "Start Eval (pending: 4)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[0](A, <int>[8]), remaining: 3)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[4](A, <int>[4]), remaining: 3)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[0](A, <int>[2]), remaining: 3)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[1](A, <int>[1]), remaining: 2)",
            "  Evaluated (changed: True, depth: 2, node: C: NotEqual<bool>[false](BitwiseAnd<int>(A, <int>[2]), <int>[0]), remaining: 2)",
            "  Evaluated (changed: True, depth: 2, node: D: NotEqual<bool>[true](BitwiseAnd<int>(A, <int>[4]), <int>[0]), remaining: 2)",
            "  Evaluated (changed: True, depth: 3, node: Or<bool>[true](D, E), remaining: 2)",
            "  Evaluated (changed: True, depth: 3, node: Not<bool>[true](C), remaining: 2)",
            "  Evaluated (changed: True, depth: 4, node: And<bool>[true](B, Not<bool>(C)), remaining: 1)",
            "  Evaluated (changed: False, depth: 5, node: F: Xor<bool>[false](And<bool>(B, Not<bool>), Or<bool>(D, E)), remaining: 0)",
            "End Eval (provoked: 0)");
        slate.CheckValue(false, "F");

        slate.SetInt(0x4, "A");
        slate.CheckEvaluate(
            "Start Eval (pending: 4)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[0](A, <int>[8]), remaining: 3)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[4](A, <int>[4]), remaining: 2)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[0](A, <int>[2]), remaining: 1)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[0](A, <int>[1]), remaining: 1)",
            "  Evaluated (changed: True, depth: 2, node: B: NotEqual<bool>[false](BitwiseAnd<int>(A, <int>[1]), <int>[0]), remaining: 1)",
            "  Evaluated (changed: True, depth: 4, node: And<bool>[false](B, Not<bool>(C)), remaining: 1)",
            "  Evaluated (changed: True, depth: 5, node: F: Xor<bool>[true](And<bool>(B, Not<bool>), Or<bool>(D, E)), remaining: 0)",
            "End Eval (provoked: 0)");
        slate.CheckValue(true, "F");

        slate.SetInt(0x8, "A");
        slate.CheckEvaluate(
            "Start Eval (pending: 4)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[8](A, <int>[8]), remaining: 4)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[0](A, <int>[4]), remaining: 4)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[0](A, <int>[2]), remaining: 3)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[0](A, <int>[1]), remaining: 2)",
            "  Evaluated (changed: True, depth: 2, node: D: NotEqual<bool>[false](BitwiseAnd<int>(A, <int>[4]), <int>[0]), remaining: 2)",
            "  Evaluated (changed: True, depth: 2, node: E: NotEqual<bool>[true](BitwiseAnd<int>(A, <int>[8]), <int>[0]), remaining: 1)",
            "  Evaluated (changed: False, depth: 3, node: Or<bool>[true](D, E), remaining: 0)",
            "End Eval (provoked: 0)");
        slate.CheckValue(true, "F");

        slate.SetInt(0xF, "A");
        slate.CheckEvaluate(
            "Start Eval (pending: 4)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[8](A, <int>[8]), remaining: 3)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[4](A, <int>[4]), remaining: 3)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[2](A, <int>[2]), remaining: 3)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[1](A, <int>[1]), remaining: 3)",
            "  Evaluated (changed: True, depth: 2, node: B: NotEqual<bool>[true](BitwiseAnd<int>(A, <int>[1]), <int>[0]), remaining: 3)",
            "  Evaluated (changed: True, depth: 2, node: C: NotEqual<bool>[true](BitwiseAnd<int>(A, <int>[2]), <int>[0]), remaining: 3)",
            "  Evaluated (changed: True, depth: 2, node: D: NotEqual<bool>[true](BitwiseAnd<int>(A, <int>[4]), <int>[0]), remaining: 3)",
            "  Evaluated (changed: False, depth: 3, node: Or<bool>[true](D, E), remaining: 2)",
            "  Evaluated (changed: True, depth: 3, node: Not<bool>[false](C), remaining: 1)",
            "  Evaluated (changed: False, depth: 4, node: And<bool>[false](B, Not<bool>(C)), remaining: 0)",
            "End Eval (provoked: 0)");
        slate.CheckValue(true, "F");

        slate.SetInt(0x5, "A");
        slate.CheckEvaluate(
            "Start Eval (pending: 4)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[0](A, <int>[8]), remaining: 4)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[4](A, <int>[4]), remaining: 3)",
            "  Evaluated (changed: True, depth: 1, node: BitwiseAnd<int>[0](A, <int>[2]), remaining: 3)",
            "  Evaluated (changed: False, depth: 1, node: BitwiseAnd<int>[1](A, <int>[1]), remaining: 2)",
            "  Evaluated (changed: True, depth: 2, node: C: NotEqual<bool>[false](BitwiseAnd<int>(A, <int>[2]), <int>[0]), remaining: 2)",
            "  Evaluated (changed: True, depth: 2, node: E: NotEqual<bool>[false](BitwiseAnd<int>(A, <int>[8]), <int>[0]), remaining: 2)",
            "  Evaluated (changed: False, depth: 3, node: Or<bool>[true](D, E), remaining: 1)",
            "  Evaluated (changed: True, depth: 3, node: Not<bool>[true](C), remaining: 1)",
            "  Evaluated (changed: True, depth: 4, node: And<bool>[true](B, Not<bool>(C)), remaining: 1)",
            "  Evaluated (changed: True, depth: 5, node: F: Xor<bool>[false](And<bool>(B, Not<bool>), Or<bool>(D, E)), remaining: 0)",
            "End Eval (provoked: 0)");
        slate.CheckValue(false, "F");
    }

    [TestMethod]
    public void TestBasicParses_Trigger() {
        Slate slate = new();
        slate.Read(
            "in trigger A;",
            "in trigger B = true;",
            "C := A | B;",
            "D := A & B;",
            "E := C ^ D;").
            NoFinish().
            Perform();

        slate.Provoke("A");
        slate.CheckProvoked(true, "A");
        slate.CheckProvoked(true, "B"); // this was created provoked
        slate.CheckEvaluate(
            "Start Eval (pending: 5)",
            "  Evaluated (changed: True, depth: 0, node: A: Input<trigger>[provoked], remaining: 4)",
            "  Evaluated (changed: True, depth: 0, node: B: Input<trigger>[provoked], remaining: 3)",
            "  Evaluated (changed: True, depth: 1, node: D: All<trigger>[provoked](A, B), remaining: 2)",
            "  Evaluated (changed: True, depth: 1, node: C: Any<trigger>[provoked](A, B), remaining: 1)",
            "  Evaluated (changed: False, depth: 2, node: E: Xor<trigger>[](C, D), remaining: 0)",
            "End Eval (provoked: 4)");
        slate.ResetTriggers();

        slate.Provoke("A");
        slate.CheckProvoked(true, "A");
        slate.CheckProvoked(false, "B");
        slate.CheckEvaluate(
            "Start Eval (pending: 3)",
            "  Evaluated (changed: True, depth: 0, node: A: Input<trigger>[provoked], remaining: 2)",
            "  Evaluated (changed: False, depth: 1, node: D: All<trigger>[](A, B), remaining: 1)",
            "  Evaluated (changed: True, depth: 1, node: C: Any<trigger>[provoked](A, B), remaining: 1)",
            "  Evaluated (changed: True, depth: 2, node: E: Xor<trigger>[provoked](C, D), remaining: 0)",
            "End Eval (provoked: 3)");
        slate.ResetTriggers();

        slate.Provoke("B");
        slate.CheckProvoked(false, "A");
        slate.CheckProvoked(true, "B");
        slate.CheckEvaluate(
            "Start Eval (pending: 3)",
            "  Evaluated (changed: True, depth: 0, node: B: Input<trigger>[provoked], remaining: 2)",
            "  Evaluated (changed: False, depth: 1, node: D: All<trigger>[](A, B), remaining: 1)",
            "  Evaluated (changed: True, depth: 1, node: C: Any<trigger>[provoked](A, B), remaining: 1)",
            "  Evaluated (changed: True, depth: 2, node: E: Xor<trigger>[provoked](C, D), remaining: 0)",
            "End Eval (provoked: 3)");
        slate.ResetTriggers();

        slate.CheckProvoked(false, "A");
        slate.CheckProvoked(false, "B");
        slate.CheckEvaluate(
            "Start Eval (pending: 0)",
            "End Eval (provoked: 0)");
    }

    [TestMethod]
    public void TestBasicParses_ExplicitCasts() {
        Slate slate = new(addConsts: false);
        slate.Read(
            "in double A = 1.2;",
            "B := (int)A;",     // Explicit
            "C := (string)A;",  // Implicit
            "D := (double)A;"). // Inheritance
            Perform();
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Input<double>[1.2],",
            "  B: Explicit<int>[1](A[1.2]),",
            "  C: Implicit<string>[1.2](A[1.2]),",
            "  D: Shell<double>[1.2](A[1.2])",
            "}");

        slate.CheckValue( 1.2,  "A");
        slate.CheckValue( 1,    "B");
        slate.CheckValue("1.2", "C");
        slate.CheckValue( 1.2,  "D");

        slate.SetDouble(42.9, "A");
        slate.PerformEvaluation();
        slate.CheckValue( 42.9,  "A");
        slate.CheckValue( 42,    "B");
        slate.CheckValue("42.9", "C");
        slate.CheckValue( 42.9,  "D");
    }

    [TestMethod]
    public void TestBasicParses_ProvokingTriggers() {
        Slate slate = new();
        slate.Read(
            "in trigger A;",
            "in trigger B;",
            "C := A & B;",
            "in D = 3;").
            NoFinish().
            Perform();
        slate.PerformEvaluation();
        slate.CheckProvoked(false, "A");
        slate.CheckProvoked(false, "B");
        slate.CheckProvoked(false, "C");
        slate.CheckValue(3, "D");
        slate.ResetTriggers();

        slate.Read(
            "->A;",
            "D = 5;").
            NoFinish().
            Perform();
        slate.PerformEvaluation();
        slate.CheckProvoked(true, "A");
        slate.CheckProvoked(false, "B");
        slate.CheckProvoked(false, "C");
        slate.CheckValue(5, "D");
        slate.ResetTriggers();

        slate.Read(
            "D > 3 -> A;",
            "A -> B;").
            NoFinish().
            Perform();
        slate.PerformEvaluation();
        slate.CheckProvoked(true, "A");
        slate.CheckProvoked(true, "B");
        slate.CheckProvoked(true, "C");
        slate.CheckValue(5, "D");
        slate.ResetTriggers();

        slate.Read(
            "false -> A;",
            "D < -1 -> B;").
            NoFinish().
            Perform();
        slate.PerformEvaluation();
        slate.CheckProvoked(false, "A");
        slate.CheckProvoked(false, "B");
        slate.CheckProvoked(false, "C");
        slate.CheckValue(5, "D");
        slate.ResetTriggers();

        slate.Read(
            "-> A -> B;").
            NoFinish().
            Perform();
        slate.PerformEvaluation();
        slate.CheckProvoked(true, "A");
        slate.CheckProvoked(true, "B");
        slate.CheckProvoked(true, "C");
        slate.CheckValue(5, "D");
        slate.ResetTriggers();

        slate.Read(
            "false -> A -> B;").
            NoFinish().
            Perform();
        slate.PerformEvaluation();
        slate.CheckProvoked(false, "A");
        slate.CheckProvoked(false, "B");
        slate.CheckProvoked(false, "C");
        slate.CheckValue(5, "D");
        slate.ResetTriggers();

        slate.Read(
            "true -> A -> B;").
            NoFinish().
            Perform();
        slate.PerformEvaluation();
        slate.CheckProvoked(true, "A");
        slate.CheckProvoked(true, "B");
        slate.CheckProvoked(true, "C");
        slate.CheckValue(5, "D");
        slate.ResetTriggers();
    }
}
