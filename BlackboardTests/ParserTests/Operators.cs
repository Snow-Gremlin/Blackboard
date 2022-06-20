using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Outer;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Operators {

        [TestMethod]
        public void CheckAllOperatorsAreTested() {
            HashSet<string> testedTags = TestTools.TestTags(typeof(Operators)).ToHashSet();
            HashSet<string> opTags = TestTools.FuncDefTags(new Slate().Global[Slate.OperatorNamespace] as Namespace).ToHashSet();

            List<string> notTested = opTags.WhereNot(testedTags.Contains).ToList();
            List<string> notAnOp = testedTags.WhereNot(opTags.Contains).ToList();

            if (notAnOp.Count > 0 || notTested.Count > 0) {
                Assert.Fail("Tests do not match the existing operations:\n" +
                    "Not Tested (" + notTested.Count + "):\n  " + notTested.Join("\n  ") + "\n" +
                    "Not an Operation (" + notAnOp.Count + "):\n  " + notAnOp.Join("\n  "));
            }
        }

        [TestMethod]
        [TestTag("and:And")]
        public void TestOperators_and_And() {
            Slate slate = new Slate().Perform("in bool A, B; C := A & B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: And<bool>");
            slate.Perform("A = false; B = false;").CheckValue(false, "C");
            slate.Perform("A = false; B = true; ").CheckValue(false, "C");
            slate.Perform("A = true;  B = false;").CheckValue(false, "C");
            slate.Perform("A = true;  B = true; ").CheckValue(true,  "C");
        }

        [TestMethod]
        [TestTag("and:BitwiseAnd<Int>")]
        public void TestOperators_and_BitwiseAnd_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A & B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: BitwiseAnd<int>");
            slate.Perform("A = 0000b; B = 0000b;").CheckValue(0b0000, "C");
            slate.Perform("A = 1100b; B = 0011b;").CheckValue(0b0000, "C");
            slate.Perform("A = 1110b; B = 0111b;").CheckValue(0b0110, "C");
            slate.Perform("A = 1111b; B = 0101b;").CheckValue(0b0101, "C");
            slate.Perform("A = 1010b; B = 1111b;").CheckValue(0b1010, "C");
            slate.Perform("A = 1111b; B = 1111b;").CheckValue(0b1111, "C");
        }

        [TestMethod]
        [TestTag("and:All")]
        public void TestOperators_and_All() {
            Slate slate = new Slate().Perform("in trigger A, B; C := A & B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: All<trigger>");
            slate.PerformWithoutReset("false -> A; false -> B;").CheckProvoked(false, "C").ResetTriggers();
            slate.PerformWithoutReset("false -> A; true  -> B;").CheckProvoked(false, "C").ResetTriggers();
            slate.PerformWithoutReset("true  -> A; false -> B;").CheckProvoked(false, "C").ResetTriggers();
            slate.PerformWithoutReset("true  -> A; true  -> B;").CheckProvoked(true,  "C").ResetTriggers();
        }

        [TestMethod]
        [TestTag("castTrigger:BoolAsTrigger")]
        public void TestOperators_castTrigger_BoolAsTrigger() {
            Slate slate = new Slate().Perform("in bool A; trigger B := A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: BoolAsTrigger<trigger>");
            slate.PerformWithoutReset("A = false;").CheckProvoked(false, "B").ResetTriggers();
            slate.PerformWithoutReset("A = true; ").CheckProvoked(true,  "B").ResetTriggers();
        }

        [TestMethod]
        [TestTag("castInt:Explicit<Double, Int>")]
        public void TestOperators_castInt_Explicit_Double_Int() {
            Slate slate = new Slate().Perform("in double A; B := (int)A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Explicit<int>");
            slate.Perform("A =  0.1;").CheckValue( 0, "B");
            slate.Perform("A =  0.9;").CheckValue( 0, "B");
            slate.Perform("A =  2.1;").CheckValue( 2, "B");
            slate.Perform("A =  2.9;").CheckValue( 2, "B");
            slate.Perform("A = -4.2;").CheckValue(-4, "B");
        }

        [TestMethod]
        [TestTag("castDouble:Implicit<Int, Double>")]
        public void TestOperators_castDouble_Implicit_Int_Double() {
            Slate slate = new Slate().Perform("in int A; double B := A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<double>");
            slate.Perform("A =  0;").CheckValue( 0.0, "B");
            slate.Perform("A =  1;").CheckValue( 1.0, "B");
            slate.Perform("A =  2;").CheckValue( 2.0, "B");
            slate.Perform("A = -1;").CheckValue(-1.0, "B");
        }

        [TestMethod]
        [TestTag("castString:Implicit<Bool, String>")]
        public void TestOperators_castString_Implicit_Bool_String() {
            Slate slate = new Slate().Perform("in bool A; string B := A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<string>");
            slate.Perform("A = true; ").CheckValue("true",  "B");
            slate.Perform("A = false;").CheckValue("false", "B");
        }

        [TestMethod]
        [TestTag("castString:Implicit<Int, String>")]
        public void TestOperators_castString_Implicit_Int_String() {
            Slate slate = new Slate().Perform("in int A; string B := A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<string>");
            slate.Perform("A =  0;").CheckValue("0",  "B");
            slate.Perform("A =  1;").CheckValue("1",  "B");
            slate.Perform("A =  2;").CheckValue("2" , "B");
            slate.Perform("A = -1;").CheckValue("-1", "B");
        }

        [TestMethod]
        [TestTag("castString:Implicit<Double, String>")]
        public void TestOperators_castString_Implicit_Double_String() {
            Slate slate = new Slate().Perform("in double A; string B := A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<string>");
            slate.Perform("A =  0.0;     ").CheckValue("0",        "B");
            slate.Perform("A =  1.0;     ").CheckValue("1",        "B");
            slate.Perform("A =  2.1;     ").CheckValue("2.1",      "B");
            slate.Perform("A = -1.24;    ").CheckValue("-1.24",    "B");
            slate.Perform("A =  1e3;     ").CheckValue("1000",     "B");
            slate.Perform("A =  0.123e-9;").CheckValue("1.23E-10", "B");
        }

        [TestMethod]
        [TestTag("divide:Div<Int>")]
        public void TestOperators_divide_Div_Int() {
            Slate slate = new Slate().Perform("in int A, B = 1; C := A/B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Div<int>");
            slate.Perform("A =  4; B =  2;").CheckValue( 2, "C");
            slate.Perform("A =  3; B =  2;").CheckValue( 1, "C");
            slate.Perform("A =  9; B =  3;").CheckValue( 3, "C");
            slate.Perform("A =  1; B =  3;").CheckValue( 0, "C");
            slate.Perform("A =  8; B =  3;").CheckValue( 2, "C");
            slate.Perform("A =  8; B = -3;").CheckValue(-2, "C");
            slate.Perform("A = -8; B =  3;").CheckValue(-2, "C");
            slate.Perform("A = -8; B = -3;").CheckValue( 2, "C");
            Assert.ThrowsException<System.DivideByZeroException>(() => slate.Perform("B = 0;"));
        }

        [TestMethod]
        [TestTag("divide:Div<Double>")]
        public void TestOperators_divide_Div_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A/B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Div<double>");
            slate.Perform("A =  4.0; B =  2.0;").CheckValue( 2.0,     "C");
            slate.Perform("A =  1.0; B =  4.0;").CheckValue( 0.25,    "C");
            slate.Perform("A =  8.0; B =  3.0;").CheckValue( 8.0/3.0, "C");
            slate.Perform("A =  8.0; B = -3.0;").CheckValue(-8.0/3.0, "C");
            slate.Perform("A = -8.0; B =  3.0;").CheckValue(-8.0/3.0, "C");
            slate.Perform("A = -8.0; B = -3.0;").CheckValue( 8.0/3.0, "C");
            slate.Perform("A =  1.0; B =  0.0;").CheckValue(double.PositiveInfinity, "C");
            slate.Perform("A =  0.0; B =  0.0;").CheckValue(double.NaN, "C");
            slate.Perform("A = -1.0; B =  0.0;").CheckValue(double.NegativeInfinity, "C");
        }

        [TestMethod]
        [TestTag("equal:Equal<Bool>")]
        public void TestOperators_equal_Equal_Bool() {
            Slate slate = new Slate().Perform("in bool A, B; C := A == B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Equal<bool>");
            slate.Perform("A = false; B = false;").CheckValue(true,  "C");
            slate.Perform("A = false; B = true; ").CheckValue(false, "C");
            slate.Perform("A = true;  B = false;").CheckValue(false, "C");
            slate.Perform("A = true;  B = true; ").CheckValue(true,  "C");
        }

        [TestMethod]
        [TestTag("equal:Equal<Int>")]
        public void TestOperators_equal_Equal_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A == B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Equal<bool>");
            slate.Perform("A =  0; B =  0;").CheckValue(true,  "C");
            slate.Perform("A = -1; B =  1;").CheckValue(false, "C");
            slate.Perform("A =  1; B = -1;").CheckValue(false, "C");
            slate.Perform("A = 42; B = 42;").CheckValue(true,  "C");
        }

        [TestMethod]
        [TestTag("equal:Equal<Double>")]
        public void TestOperators_equal_Equal_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A == B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Equal<bool>");
            slate.Perform("A =  0.0;     B = 0.0;    ").CheckValue(true,  "C");
            slate.Perform("A = -1.0;     B = 1.0;    ").CheckValue(false, "C");
            slate.Perform("A =  1.00004; B = 1.00005;").CheckValue(false, "C");
            slate.Perform("A =  0.001;   B = 1.0e-3; ").CheckValue(true,  "C");
        }

        [TestMethod]
        [TestTag("equal:Equal<String>")]
        public void TestOperators_equal_Equal_String() {
            Slate slate = new Slate().Perform("in string A, B; C := A == B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Equal<bool>");
            slate.Perform("A = '';            B = '';           ").CheckValue(true,  "C");
            slate.Perform("A = 'Hello World'; B = 'Hello World';").CheckValue(true,  "C");
            slate.Perform("A = 'mop';         B = 'pop';        ").CheckValue(false, "C");
            slate.Perform("A = 'mop';         B = 'map';        ").CheckValue(false, "C");
            slate.Perform("A = 'mop';         B = 'Mop';        ").CheckValue(false, "C");
        }

        [TestMethod]
        [TestTag("greater:GreaterThan<Int>")]
        public void TestOperators_greater_GreaterThan_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A > B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThan<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("greater:GreaterThan<Double>")]
        public void TestOperators_greater_GreaterThan_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A > B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThan<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("greater:GreaterThan<String>")]
        public void TestOperators_greater_GreaterThan_String() {
            Slate slate = new Slate().Perform("in string A, B; C := A > B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThan<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("greaterEqual:GreaterThanOrEqual<Int>")]
        public void TestOperators_greaterEqual_GreaterThanOrEqual_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A >= B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThanOrEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("greaterEqual:GreaterThanOrEqual<Double>")]
        public void TestOperators_greaterEqual_GreaterThanOrEqual_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A >= B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThanOrEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("greaterEqual:GreaterThanOrEqual<String>")]
        public void TestOperators_greaterEqual_GreaterThanOrEqual_String() {
            Slate slate = new Slate().Perform("in string A, B; C := A >= B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThanOrEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("invert:BitwiseNot<Int>")]
        public void TestOperators_invert_BitwiseNot_Int() {
            Slate slate = new Slate().Perform("in int A; B := ~A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: BitwiseNot<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("less:LessThan<Int>")]
        public void TestOperators_less_LessThan_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A < B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThan<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("less:LessThan<Double>")]
        public void TestOperators_less_LessThan_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A < B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThan<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("less:LessThan<String>")]
        public void TestOperators_less_LessThan_String() {
            Slate slate = new Slate().Perform("in string A, B; C := A < B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThan<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("lessEqual:LessThanOrEqual<Int>")]
        public void TestOperators_lessEqual_LessThanOrEqual_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A <= B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThanOrEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("lessEqual:LessThanOrEqual<Double>")]
        public void TestOperators_lessEqual_LessThanOrEqual_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A <= B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThanOrEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("lessEqual:LessThanOrEqual<String>")]
        public void TestOperators_lessEqual_LessThanOrEqual_String() {
            Slate slate = new Slate().Perform("in string A, B; C := A <= B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThanOrEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("logicalAnd:And")]
        public void TestOperators_logicalAnd_And() {
            Slate slate = new Slate().Perform("in bool A, B; C := A && B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: And<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("logicalAnd:All")]
        public void TestOperators_logicalAnd_All() {
            Slate slate = new Slate().Perform("in trigger A, B; C := A && B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: All<trigger>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("logicalOr:Or")]
        public void TestOperators_logicalOr_Or() {
            Slate slate = new Slate().Perform("in bool A, B; C := A || B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Or<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("logicalOr:Any")]
        public void TestOperators_logicalOr_Any() {
            Slate slate = new Slate().Perform("in trigger A, B; C := A || B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Any<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("logicalXor:Xor")]
        public void TestOperators_logicalXor_Xor() {
            Slate slate = new Slate().Perform("in bool A, B; C := A ^^ B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Xor<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("logicalXor:XorTrigger")]
        public void TestOperators_logicalXor_XorTrigger() {
            Slate slate = new Slate().Perform("in trigger A, B; C := A ^^ B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: XorTrigger<trigger>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("modulo:Mod<Int>")]
        public void TestOperators_modulo_Mod_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A % B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mod<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("modulo:Mod<Double>")]
        public void TestOperators_modulo_Mod_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A % B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mod<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("multiply:Mul<Int>")]
        public void TestOperators_multiply_Mul_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A * B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mul<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("multiply:Mul<Double>")]
        public void TestOperators_multiply_Mul_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A * B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mul<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("negate:Neg<Int>")]
        public void TestOperators_negate_Neg_Int() {
            Slate slate = new Slate().Perform("in int A; B := -A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Neg<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("negate:Neg<Double>")]
        public void TestOperators_negate_Neg_Double() {
            Slate slate = new Slate().Perform("in double A; B := -A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Neg<double>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("not:Not")]
        public void TestOperators_not_Not() {
            Slate slate = new Slate().Perform("in bool A; B := !A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Not");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("notEqual:NotEqual<Bool>")]
        public void TestOperators_notEqual_NotEqual_Bool() {
            Slate slate = new Slate().Perform("in bool A, B; C := A != B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("notEqual:NotEqual<Int>")]
        public void TestOperators_notEqual_NotEqual_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A != B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("notEqual:NotEqual<Double>")]
        public void TestOperators_notEqual_NotEqual_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A != B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("notEqual:NotEqual<String>")]
        public void TestOperators_notEqual_NotEqual_String() {
            Slate slate = new Slate().Perform("in string A, B; C := A != B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("or:Or")]
        public void TestOperators_or_Or() {
            Slate slate = new Slate().Perform("in bool A, B; C := A | B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Or<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("or:BitwiseOr<Int>")]
        public void TestOperators_or_BitwiseOr_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A | B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: BitwiseOr<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("or:Any")]
        public void TestOperators_or_Any() {
            Slate slate = new Slate().Perform("in trigger A, B; C := A | B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Any<trigger>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("power:BinaryFunc<Double, Double, Double>")]
        public void TestOperators_power_BinaryFunc_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A ** B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: BinaryFunc<double>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("shiftLeft:LeftShift<Int>")]
        public void TestOperators_shiftLeft_LeftShift_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A << B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: LeftShift<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("shiftRight:RightShift<Int>")]
        public void TestOperators_shiftRight_RightShift_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A >> B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: RightShift<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("subtract:Sub<Int>")]
        public void TestOperators_subtract_Sub_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A - B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sub<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("subtract:Sub<Double>")]
        public void TestOperators_subtract_Sub_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A - B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sub<double>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("sum:Sum<Int>")]
        public void TestOperators_sum_Sum_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A + B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sum<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("sum:Sum<Double>")]
        public void TestOperators_sum_Sum_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := A + B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sum<double>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("sum:Sum<String>")]
        public void TestOperators_sum_Sum_String() {
            Slate slate = new Slate().Perform("in string A, B; C := A + B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sum<string>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("ternary:SelectValue<Bool>")]
        public void TestOperators_ternary_SelectValue_Bool() {
            Slate slate = new Slate().Perform("in bool A, B, C; D := A ? B : C;");
            slate.CheckNodeString(Stringifier.Basic(), "D", "D: SelectValue<bool>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("ternary:SelectValue<Int>")]
        public void TestOperators_ternary_SelectValue_Int() {
            Slate slate = new Slate().Perform("in bool A; in int B, C; D := A ? B : C;");
            slate.CheckNodeString(Stringifier.Basic(), "D", "D: SelectValue<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("ternary:SelectValue<Double>")]
        public void TestOperators_ternary_SelectValue_Doule() {
            Slate slate = new Slate().Perform("in bool A; in double B, C; D := A ? B : C;");
            slate.CheckNodeString(Stringifier.Basic(), "D", "D: SelectValue<double>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("ternary:SelectValue<String>")]
        public void TestOperators_ternary_SelectValue_String() {
            Slate slate = new Slate().Perform("in bool A; in string B, C; D := A ? B : C;");
            slate.CheckNodeString(Stringifier.Basic(), "D", "D: SelectValue<string>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("ternary:SelectTrigger")]
        public void TestOperators_ternary_SelectTrigger() {
            Slate slate = new Slate().Perform("in bool A; in trigger B, C; D := A ? B : C;");
            slate.CheckNodeString(Stringifier.Basic(), "D", "D: SelectTrigger");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("xor:Xor")]
        public void TestOperators_xor_Xor() {
            Slate slate = new Slate().Perform("in bool A, B; C := A ^ B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Xor");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("xor:BitwiseXor<Int>")]
        public void TestOperators_xor_BitwiseXor_Int() {
            Slate slate = new Slate().Perform("in int A, B; C := A ^ B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: BitwiseXor<int>");

            // TODO: Implement
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [TestTag("xor:XorTrigger")]
        public void TestOperators_xor_XorTrigger() {
            Slate slate = new Slate().Perform("in trigger A, B; C := A ^ B;");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: XorTrigger");

            // TODO: Implement
            throw new System.NotImplementedException();
        }
    }
}
