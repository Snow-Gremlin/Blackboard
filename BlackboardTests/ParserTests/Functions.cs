using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S = System;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Functions {

        [TestMethod]
        public void CheckAllFunctionsAreTested() =>
            TestTools.SetEntriesMatch(
                TestTools.FuncDefTags(new Slate().Global).WhereNot(tag => tag.StartsWith(Slate.OperatorNamespace)),
                TestTools.TestTags(this.GetType()),
                "Tests do not match the existing function");

        [TestMethod]
        [TestTag("abs:Abs<Double>")]
        public void TestFunctions_abs_Abs_Double() {
            Slate slate = new Slate().Perform("in double A; B := abs(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Abs<double>");
            slate.Perform("A =   0.0; ").CheckValue(0.0,   "B");
            slate.Perform("A =   1.0; ").CheckValue(1.0,   "B");
            slate.Perform("A =  -1.0; ").CheckValue(1.0,   "B");
            slate.Perform("A =  42.03;").CheckValue(42.03, "B");
            slate.Perform("A = -42.03;").CheckValue(42.03, "B");
        }

        [TestMethod]
        [TestTag("abs:Abs<Int>")]
        public void TestFunctions_abs_Abs_Int() {
            Slate slate = new Slate().Perform("in int A; B := abs(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Abs<int>");
            slate.Perform("A =   0;").CheckValue( 0, "B");
            slate.Perform("A =   1;").CheckValue( 1, "B");
            slate.Perform("A =  -1;").CheckValue( 1, "B");
            slate.Perform("A =  42;").CheckValue(42, "B");
            slate.Perform("A = -42;").CheckValue(42, "B");
        }

        [TestMethod]
        [TestTag("acos:UnaryFuncs<Double, Double>")]
        public void TestFunctions_acos_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := acos(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Acos<double>");
            slate.Perform("A =  0.0;").CheckValue(S.Math.PI*0.5, "B");
            slate.Perform("A =  1.0;").CheckValue(0.0,           "B");
            slate.Perform("A = -1.0;").CheckValue(S.Math.PI,     "B");
            slate.Perform("A =  1.1;").CheckValue(double.NaN,    "B");
        }

        [TestMethod]
        [TestTag("acosh:UnaryFuncs<Double, Double>")]
        public void TestFunctions_acosh_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := acosh(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Acosh<double>");
            slate.Perform("A = 0.0;   ").CheckValue(double.NaN,           "B");
            slate.Perform("A = 1.0;   ").CheckValue(S.Math.Acosh(1.0),    "B");
            slate.Perform("A = 1.5;   ").CheckValue(S.Math.Acosh(1.5),    "B");
            slate.Perform("A = 1000.0;").CheckValue(S.Math.Acosh(1000.0), "B");
        }

        [TestMethod]
        [TestTag("all:All")]
        public void TestFunctions_all_All() {
            Slate slate = new Slate().Perform("in trigger A, B, C, D; E := all(A, B, C, D);");
            slate.CheckNodeString(Stringifier.Basic(), "E", "E: All<trigger>");
            slate.PerformWithoutReset("A = false; B = false; C = false; D = false;").CheckProvoked(false, "E").ResetTriggers();
            slate.PerformWithoutReset("A = true;  B = false; C = false; D = false;").CheckProvoked(false, "E").ResetTriggers();
            slate.PerformWithoutReset("A = false; B = true;  C = false; D = false;").CheckProvoked(false, "E").ResetTriggers();
            slate.PerformWithoutReset("A = false; B = false; C = true;  D = false;").CheckProvoked(false, "E").ResetTriggers();
            slate.PerformWithoutReset("A = false; B = false; C = false; D = true; ").CheckProvoked(false, "E").ResetTriggers();
            slate.PerformWithoutReset("A = true;  B = true;  C = true;  D = true; ").CheckProvoked(true,  "E").ResetTriggers();
            slate.Perform("F := all(A);"); // Single pass through, F is set to A. 
            slate.CheckNodeString(Stringifier.Basic(), "F", "F: Input<trigger>");
            TestTools.CheckException(() => slate.Perform("G := all();"),
                "Error occurred while parsing input code.",
                "[Error: Could not perform the function without any inputs.",
                "   [Function: FuncGroup]",
                "   [Location: Unnamed:1, 9, 9]]");
        }

        [TestMethod]
        [TestTag("and:And")]
        public void TestFunctions_and_And() {
            Slate slate = new Slate().Perform("in bool A, B, C, D; E := and(A, B, C, D);");
            slate.CheckNodeString(Stringifier.Basic(), "E", "E: And<bool>");
            slate.Perform("A = false; B = false; C = false; D = false;").CheckValue(false, "E");
            slate.Perform("A = true;  B = false; C = false; D = false;").CheckValue(false, "E");
            slate.Perform("A = false; B = true;  C = false; D = false;").CheckValue(false, "E");
            slate.Perform("A = false; B = false; C = true;  D = false;").CheckValue(false, "E");
            slate.Perform("A = false; B = false; C = false; D = true; ").CheckValue(false, "E");
            slate.Perform("A = true;  B = true;  C = true;  D = true; ").CheckValue(true,  "E");
            slate.Perform("F := and(A);"); // Single pass through, F is set to A. 
            slate.CheckNodeString(Stringifier.Basic(), "F", "F: Input<bool>");
            TestTools.CheckException(() => slate.Perform("G := and();"),
                "Error occurred while parsing input code.",
                "[Error: Could not perform the function without any inputs.",
                "   [Function: FuncGroup]",
                "   [Location: Unnamed:1, 9, 9]]");
        }

        [TestMethod]
        [TestTag("and:BitwiseAnd<Int>")]
        public void TestFunctions_and_BitwiseAnd_Int() {
            Slate slate = new Slate().Perform("in int A, B, C, D; E := and(A, B, C, D);");
            slate.CheckNodeString(Stringifier.Basic(), "E", "E: BitwiseAnd<int>");
            slate.Perform("A = 0000b; B = 0000b; C = 0000b; D = 0000b;").CheckValue(0b0000, "E");
            slate.Perform("A = 1000b; B = 0100b; C = 0010b; D = 0001b;").CheckValue(0b0000, "E");
            slate.Perform("A = 1100b; B = 0110b; C = 0011b; D = 1111b;").CheckValue(0b0000, "E");
            slate.Perform("A = 1010b; B = 1001b; C = 1010b; D = 1011b;").CheckValue(0b1000, "E");
            slate.Perform("A = 1111b; B = 0000b; C = 0000b; D = 0000b;").CheckValue(0b0000, "E");
            slate.Perform("A = 0000b; B = 1111b; C = 0000b; D = 0000b;").CheckValue(0b0000, "E");
            slate.Perform("A = 0000b; B = 0000b; C = 1111b; D = 0000b;").CheckValue(0b0000, "E");
            slate.Perform("A = 0000b; B = 0000b; C = 0000b; D = 1111b;").CheckValue(0b0000, "E");
            slate.Perform("A = 1111b; B = 1111b; C = 1111b; D = 1111b;").CheckValue(0b1111, "E");
            slate.Perform("F := and(A);"); // Single pass through, F is set to A. 
            slate.CheckNodeString(Stringifier.Basic(), "F", "F: Input<int>");
            // The zero input scenario is checked in TestFunctions_and_And.
        }

        [TestMethod]
        [TestTag("any:Any")]
        public void TestFunctions_any_Any() {
            Slate slate = new Slate().Perform("in trigger A, B, C, D; E := any(A, B, C, D);");
            slate.CheckNodeString(Stringifier.Basic(), "E", "E: Any<trigger>");
            slate.PerformWithoutReset("A = false; B = false; C = false; D = false;").CheckProvoked(false, "E").ResetTriggers();
            slate.PerformWithoutReset("A = true;  B = false; C = false; D = false;").CheckProvoked(true,  "E").ResetTriggers();
            slate.PerformWithoutReset("A = false; B = true;  C = false; D = false;").CheckProvoked(true,  "E").ResetTriggers();
            slate.PerformWithoutReset("A = false; B = false; C = true;  D = false;").CheckProvoked(true,  "E").ResetTriggers();
            slate.PerformWithoutReset("A = false; B = false; C = false; D = true; ").CheckProvoked(true,  "E").ResetTriggers();
            slate.PerformWithoutReset("A = true;  B = true;  C = true;  D = true; ").CheckProvoked(true,  "E").ResetTriggers();
            slate.Perform("F := any(A);"); // Single pass through, F is set to A. 
            slate.CheckNodeString(Stringifier.Basic(), "F", "F: Input<trigger>");
            TestTools.CheckException(() => slate.Perform("G := any();"),
                "Error occurred while parsing input code.",
                "[Error: Could not perform the function without any inputs.",
                "   [Function: FuncGroup]",
                "   [Location: Unnamed:1, 9, 9]]");
        }

        [TestMethod]
        [TestTag("asin:UnaryFuncs<Double, Double>")]
        public void TestFunctions_asin_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := asin(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Asin<double>");
            slate.Perform("A =  0.0;").CheckValue( 0.0,           "B");
            slate.Perform("A =  1.0;").CheckValue( S.Math.PI*0.5, "B");
            slate.Perform("A = -1.0;").CheckValue(-S.Math.PI*0.5, "B");
            slate.Perform("A =  2.0;").CheckValue( double.NaN,    "B");
        }

        [TestMethod]
        [TestTag("asinh:UnaryFuncs<Double, Double>")]
        public void TestFunctions_asinh_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := asinh(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Asinh<double>");
            slate.Perform("A =  0.0;").CheckValue(S.Math.Asinh( 0.0), "B");
            slate.Perform("A =  1.0;").CheckValue(S.Math.Asinh( 1.0), "B");
            slate.Perform("A = -1.0;").CheckValue(S.Math.Asinh(-1.0), "B");
            slate.Perform("A =  2.0;").CheckValue(S.Math.Asinh( 2.0), "B");
            slate.Perform("A = 10.0;").CheckValue(S.Math.Asinh(10.0), "B");
        }

        [TestMethod]
        [TestTag("atan:BinaryFunc<Double, Double, Double>")]
        public void TestFunctions_atan_BinaryFunc_Double_Double_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := atan(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Atan2<double>");
            slate.Perform("A =  0.0; B =  0.0;").CheckValue( 0.0,               "C");
            slate.Perform("A =  1.0; B =  1.0;").CheckValue( S.Math.PI*0.25,    "C");
            slate.Perform("A = -1.0; B = -1.0;").CheckValue(-S.Math.PI*3.0/4.0, "C");
            slate.Perform("A =  1.0; B =  0.0;").CheckValue( S.Math.PI*0.5,     "C");
        }

        [TestMethod]
        [TestTag("atan:UnaryFuncs<Double, Double>")]
        public void TestFunctions_atan_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := atan(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Atan<double>");
            slate.Perform("A =  0.0;").CheckValue( 0.0,               "B");
            slate.Perform("A =  1.0;").CheckValue( S.Math.PI*0.25,    "B");
            slate.Perform("A = -1.0;").CheckValue(-S.Math.PI*0.25,    "B");
            slate.Perform("A =  2.0;").CheckValue( S.Math.Atan( 2.0), "B");
            slate.Perform("A = 10.0;").CheckValue( S.Math.Atan(10.0), "B");
        }

        [TestMethod]
        [TestTag("atanh:UnaryFuncs<Double, Double>")]
        public void TestFunctions_atanh_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := atanh(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Atanh<double>");
            slate.Perform("A =  0.0;").CheckValue( 0.0,                    "B");
            slate.Perform("A =  0.5;").CheckValue(S.Math.Atanh( 0.5),      "B");
            slate.Perform("A = -0.5;").CheckValue(S.Math.Atanh(-0.5),      "B");
            slate.Perform("A =  1.0;").CheckValue(double.PositiveInfinity, "B");
            slate.Perform("A = -1.0;").CheckValue(double.NegativeInfinity, "B");
            slate.Perform("A =  2.0;").CheckValue(double.NaN,              "B");
        }

        [TestMethod]
        [TestTag("average:Average")]
        public void TestFunctions_average_Average() {
            Slate slate = new Slate().Perform("in double A, B, C, D; E := average(A, B, C, D);");
            slate.CheckNodeString(Stringifier.Basic(), "E", "E: Average<double>");
            slate.Perform("A =  1.0; B =  1.0; C = 1.0; D = 1.0;").CheckValue(1.0, "E");
            slate.Perform("A = -1.0; B = -1.0; C = 1.0; D = 1.0;").CheckValue(0.0, "E");
            slate.Perform("A =  1.0; B =  1.0; C = 2.0; D = 2.0;").CheckValue(1.5, "E");
            slate.Perform("A =  1.0; B =  2.0; C = 3.0; D = 4.0;").CheckValue(2.5, "E");
            slate.Perform("A =  5.0; B =  1.0; C = 5.0; D = 1.0;").CheckValue(3.0, "E");
            slate.Perform("F := average(A);"); // Single pass through, F is set to A. 
            slate.CheckNodeString(Stringifier.Basic(), "F", "F: Input<double>");
            TestTools.CheckException(() => slate.Perform("G := average();"),
                "Error occurred while parsing input code.",
                "[Error: Could not perform the function without any inputs.",
                "   [Function: FuncGroup]",
                "   [Location: Unnamed:1, 13, 13]]");
        }

        [TestMethod]
        [TestTag("cbrt:UnaryFuncs<Double, Double>")]
        public void TestFunctions_cbrt_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := cbrt(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Cbrt<double>");
            slate.Perform("A =  0.0;").CheckValue( 0.0, "B");
            slate.Perform("A =  8.0;").CheckValue( 2.0, "B");
            slate.Perform("A = -8.0;").CheckValue(-2.0, "B");
            slate.Perform("A = 27.0;").CheckValue( 3.0, "B");
        }

        [TestMethod]
        [TestTag("ceiling:UnaryFuncs<Double, Double>")]
        public void TestFunctions_ceiling_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := ceiling(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Ceiling<double>");
            slate.Perform("A =  0.0;").CheckValue( 0.0, "B");
            slate.Perform("A =  1.0;").CheckValue( 1.0, "B");
            slate.Perform("A = -1.0;").CheckValue(-1.0, "B");
            slate.Perform("A =  2.4;").CheckValue( 3.0, "B");
            slate.Perform("A = -2.4;").CheckValue(-2.0, "B");
            slate.Perform("A =  2.5;").CheckValue( 3.0, "B");
            slate.Perform("A = -2.5;").CheckValue(-2.0, "B");
        }

        [TestMethod]
        [TestTag("clamp:Clamp<Double>")]
        public void TestFunctions_clamp_Clamp_Double() {
            Slate slate = new Slate().Perform("in double A, B, C; D := clamp(A, B, C);");
            slate.CheckNodeString(Stringifier.Basic(), "D", "D: Clamp<double>");
            slate.Perform("A =  0.0; B =  0.0; C =  0.0;").CheckValue( 0.0, "D");
            slate.Perform("A = -0.1; B =  0.0; C =  1.0;").CheckValue( 0.0, "D");
            slate.Perform("A =  0.0; B =  0.0; C =  1.0;").CheckValue( 0.0, "D");
            slate.Perform("A =  0.1; B =  0.0; C =  1.0;").CheckValue( 0.1, "D");
            slate.Perform("A =  0.9; B =  0.0; C =  1.0;").CheckValue( 0.9, "D");
            slate.Perform("A =  1.0; B =  0.0; C =  1.0;").CheckValue( 1.0, "D");
            slate.Perform("A =  1.1; B =  0.0; C =  1.0;").CheckValue( 1.0, "D");
            slate.Perform("A = -4.0; B = -1.0; C = 10.0;").CheckValue(-1.0, "D");
            slate.Perform("A =  6.0; B = -1.0; C = 10.0;").CheckValue( 6.0, "D");
            slate.Perform("A = 12.1; B = -1.0; C = 10.0;").CheckValue(10.0, "D");
        }

        [TestMethod]
        [TestTag("clamp:Clamp<Int>")]
        public void TestFunctions_clamp_Clamp_Int() {
            Slate slate = new Slate().Perform("in int A, B, C; D := clamp(A, B, C);");
            slate.CheckNodeString(Stringifier.Basic(), "D", "D: Clamp<int>");
            slate.Perform("A =  0; B =  0; C = 0;").CheckValue( 0, "D");
            slate.Perform("A =  0; B =  1; C = 3;").CheckValue( 1, "D");
            slate.Perform("A =  1; B =  1; C = 3;").CheckValue( 1, "D");
            slate.Perform("A =  2; B =  1; C = 3;").CheckValue( 2, "D");
            slate.Perform("A =  3; B =  1; C = 3;").CheckValue( 3, "D");
            slate.Perform("A =  4; B =  1; C = 3;").CheckValue( 3, "D");
            slate.Perform("A =  2; B = -1; C = 0;").CheckValue( 0, "D");
            slate.Perform("A = -2; B = -1; C = 0;").CheckValue(-1, "D");
        }

        [TestMethod]
        [TestTag("cos:UnaryFuncs<Double, Double>")]
        public void TestFunctions_cos_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := cos(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Cos<double>");
            slate.Perform("A =  0.0;   ").CheckValue( 1.0, "B");
            slate.Perform("A =  pi*0.5;").CheckValue(S.Math.Cos( S.Math.PI*0.5), "B");
            slate.Perform("A = -pi*0.5;").CheckValue(S.Math.Cos(-S.Math.PI*0.5), "B");
            slate.Perform("A =  pi;    ").CheckValue(-1.0, "B");
            slate.Perform("A = -pi;    ").CheckValue(-1.0, "B");
            slate.Perform("A =  pi*1.5;").CheckValue(S.Math.Cos( S.Math.PI*1.5), "B");
        }

        [TestMethod]
        [TestTag("cosh:UnaryFuncs<Double, Double>")]
        public void TestFunctions_cosh_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := cosh(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Cosh<double>");
            slate.Perform("A =  0.0;").CheckValue(1.0, "B");
            slate.Perform("A =  1.0;").CheckValue(S.Math.Cosh( 1.0), "B");
            slate.Perform("A = -1.0;").CheckValue(S.Math.Cosh(-1.0), "B");
            slate.Perform("A =  3.0;").CheckValue(S.Math.Cosh( 3.0), "B");
            slate.Perform("A = 12.3;").CheckValue(S.Math.Cosh(12.3), "B");
        }

        [TestMethod]
        [TestTag("epsilon:EpsilonEqual<Double>")]
        public void TestFunctions_epsilon_EpsilonEqual_Double() {
            Slate slate = new Slate().Perform("in double A, B, C; D := epsilon(A, B, C);");
            slate.CheckNodeString(Stringifier.Basic(), "D", "D: EpsilonEqual<bool>");
            slate.Perform("A =  0.0; B = -0.3; C = 0.2;").CheckValue(false, "D");
            slate.Perform("A =  0.0; B = -0.2; C = 0.2;").CheckValue(false, "D");
            slate.Perform("A =  0.0; B = -0.1; C = 0.2;").CheckValue(true,  "D");
            slate.Perform("A =  0.0; B =  0.0; C = 0.2;").CheckValue(true,  "D");
            slate.Perform("A =  0.0; B =  0.1; C = 0.2;").CheckValue(true,  "D");
            slate.Perform("A =  0.0; B =  0.2; C = 0.2;").CheckValue(false, "D");
            slate.Perform("A =  0.0; B =  0.3; C = 0.2;").CheckValue(false, "D");
            slate.Perform("A = -0.3; B =  0.0; C = 0.2;").CheckValue(false, "D");
            slate.Perform("A = -0.2; B =  0.0; C = 0.2;").CheckValue(false, "D");
            slate.Perform("A = -0.1; B =  0.0; C = 0.2;").CheckValue(true,  "D");
            slate.Perform("A =  0.0; B =  0.0; C = 0.2;").CheckValue(true,  "D");
            slate.Perform("A =  0.1; B =  0.0; C = 0.2;").CheckValue(true,  "D");
            slate.Perform("A =  0.2; B =  0.0; C = 0.2;").CheckValue(false, "D");
            slate.Perform("A =  0.3; B =  0.0; C = 0.2;").CheckValue(false, "D");
        }

        [TestMethod]
        [TestTag("exp:UnaryFuncs<Double, Double>")]
        public void TestFunctions_exp_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := exp(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Exp<double>");
            slate.Perform("A = 0.0;").CheckValue(1.0, "B");
            slate.Perform("A = 1.0;").CheckValue(S.Math.E, "B");
            slate.Perform("A = 2.0;").CheckValue(S.Math.Exp(2.0), "B");
            slate.Perform("A = 3.1;").CheckValue(S.Math.Exp(3.1), "B");
            slate.Perform("A = 4.2;").CheckValue(S.Math.Exp(4.2), "B");
        }

        [TestMethod]
        [TestTag("floor:UnaryFuncs<Double, Double>")]
        public void TestFunctions_floor_UnaryFuncs_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := floor(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Floor<double>");
            slate.Perform("A =  0.0;").CheckValue( 0.0, "B");
            slate.Perform("A =  1.0;").CheckValue( 1.0, "B");
            slate.Perform("A = -1.0;").CheckValue(-1.0, "B");
            slate.Perform("A =  2.4;").CheckValue( 2.0, "B");
            slate.Perform("A = -2.4;").CheckValue(-3.0, "B");
            slate.Perform("A =  2.5;").CheckValue( 2.0, "B");
            slate.Perform("A = -2.5;").CheckValue(-3.0, "B");
        }

        [TestMethod]
        [TestTag("format:Format")]
        public void TestFunctions_format_Format() {
            Slate slate = new Slate().Perform("in string A; in double B, C, D; D := format(A, B, C, D);");
            slate.CheckNodeString(Stringifier.Basic(), "D", "D: Floor<double>");

            // TODO: Fix case error when casting InputValue<Double> to IValueParent<IData>.
        }



    }
}
