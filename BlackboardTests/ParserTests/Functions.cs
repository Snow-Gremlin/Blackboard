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
            Tools.TestTools.CheckException(() => slate.Perform("G := all();"),
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
            Tools.TestTools.CheckException(() => slate.Perform("G := and();"),
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
            Tools.TestTools.CheckException(() => slate.Perform("G := any();"),
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
            Tools.TestTools.CheckException(() => slate.Perform("G := average();"),
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
            Slate slate = new Slate().Perform("in string A; B := format(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Format<string>");
            slate.Perform("A = \"Hello World\";").CheckValue("Hello World", "B");

            slate = new Slate().Perform("in string A; in bool B; C := format(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Format<string>");
            slate.Perform("A = \"Answer is {0}\"; B = true;").CheckValue("Answer is True", "C");
            slate.Perform("B = false;").CheckValue("Answer is False", "C");
            slate.Perform("A = \"{0} was the answer!\";").CheckValue("False was the answer!", "C");

            slate = new Slate().Perform("in string A; in int B; C := format(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Format<string>");
            slate.Perform("A = \"Answer is {0}\"; B = 42;").CheckValue("Answer is 42", "C");
            slate.Perform("B = -1234567;").CheckValue("Answer is -1234567", "C");
            slate.Perform("A = \"{0:#,##0} was the answer!\";").CheckValue("-1,234,567 was the answer!", "C");

            slate = new Slate().Perform("in string A; in double B; C := format(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Format<string>");
            slate.Perform("A = \"Answer is {0}\"; B = 42.0;").CheckValue("Answer is 42", "C");
            slate.Perform("B = 123.456789;").CheckValue("Answer is 123.456789", "C");
            slate.Perform("A = \"{0:#.000} was the answer!\";").CheckValue("123.457 was the answer!", "C");
            slate.Perform("B = 12;").CheckValue("12.000 was the answer!", "C");

            slate = new Slate().Perform("in string A; in string B; C := format(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Format<string>");
            slate.Perform("A = \"Answer is {0}\"; B = \"Sleep\";").CheckValue("Answer is Sleep", "C");
            slate.Perform("A = \"!{0,12}!\"; B = \"Caffeine\";").CheckValue("!    Caffeine!", "C");

            slate = new Slate().Perform("in string A; in double B; in int C; in bool D; in string E; F := format(A, B, C, D, E);");
            slate.CheckNodeString(Stringifier.Basic(), "F", "F: Format<string>");
            slate.Perform("A = \"[{0}, {1}, {2}, {3}]\"; B = 3.14; C = 320; D = false; E = \"Cat\";").CheckValue("[3.14, 320, False, Cat]", "F");
            slate.Perform("A = \"<<{0}>> <<{1}>> <<{2}>> <<{3}>> <<{2}>> <<{1}>> <<{0}>>\";").
                CheckValue("<<3.14>> <<320>> <<False>> <<Cat>> <<False>> <<320>> <<3.14>>", "F");
        }

        [TestMethod]
        [TestTag("implies:Implies")]
        public void TestFunctions_implies_Implies() {
            Slate slate = new Slate().Perform("in bool A, B; C := implies(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Implies<bool>");
            slate.Perform("A = false; B = false;").CheckValue(true,  "C");
            slate.Perform("A = false; B = true; ").CheckValue(true,  "C");
            slate.Perform("A = true;  B = false;").CheckValue(false, "C");
            slate.Perform("A = true;  B = true; ").CheckValue(true,  "C");
        }

        [TestMethod]
        [TestTag("inRange:InRange<Double>")]
        public void TestFunctions_inRange_InRange_Double() {
            Slate slate = new Slate().Perform("in double A, Min, Max; B := inRange(A, Min, Max);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: InRange<bool>");
            slate.Perform("A = 0.0; Min = 0.0; Max = 0.0;").CheckValue(true,  "B");
            slate.Perform("A = 0.5; Min = 1.0; Max = 2.0;").CheckValue(false, "B");
            slate.Perform("A = 1.0; Min = 1.0; Max = 2.0;").CheckValue(true,  "B");
            slate.Perform("A = 1.5; Min = 1.0; Max = 2.0;").CheckValue(true,  "B");
            slate.Perform("A = 2.0; Min = 1.0; Max = 2.0;").CheckValue(true,  "B");
            slate.Perform("A = 2.5; Min = 1.0; Max = 2.0;").CheckValue(false, "B");
            slate.Perform("A = 2.0; Min = 3.0; Max = 1.0;").CheckValue(false, "B"); // Min/max reversed
        }

        [TestMethod]
        [TestTag("inRange:InRange<Int>")]
        public void TestFunctions_inRange_InRange_Int() {
            Slate slate = new Slate().Perform("in int A, Min, Max; B := inRange(A, Min, Max);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: InRange<bool>");
            slate.Perform("A = 0; Min = 0; Max = 0;").CheckValue(true,  "B");
            slate.Perform("A = 0; Min = 1; Max = 3;").CheckValue(false, "B");
            slate.Perform("A = 1; Min = 1; Max = 3;").CheckValue(true,  "B");
            slate.Perform("A = 2; Min = 1; Max = 3;").CheckValue(true,  "B");
            slate.Perform("A = 3; Min = 1; Max = 3;").CheckValue(true,  "B");
            slate.Perform("A = 4; Min = 1; Max = 3;").CheckValue(false, "B");
            slate.Perform("A = 2; Min = 3; Max = 1;").CheckValue(false, "B"); // Min/max reversed
        }

        [TestMethod]
        [TestTag("isFinite:UnaryFuncs<Double, Bool>")]
        public void TestFunctions_isFinite_UnaryFuncs_Double_Bool() {
            Slate slate = new Slate().Perform("in double A; B := isFinite(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsFinite<bool>");
            slate.Perform("A =  0.0; ").CheckValue(true,  "B");
            slate.Perform("A =  9e13;").CheckValue(true,  "B");
            slate.Perform("A = -9e13;").CheckValue(true,  "B");
            slate.Perform("A =  inf; ").CheckValue(false, "B");
            slate.Perform("A = -inf; ").CheckValue(false, "B");
            slate.Perform("A =  nan; ").CheckValue(false, "B");
        }

        [TestMethod]
        [TestTag("isInf:UnaryFuncs<Double, Bool>")]
        public void TestFunctions_isInf_UnaryFuncs_Double_Bool() {
            Slate slate = new Slate().Perform("in double A; B := isInf(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsInfinity<bool>");
            slate.Perform("A =  0.0; ").CheckValue(false, "B");
            slate.Perform("A =  9e13;").CheckValue(false, "B");
            slate.Perform("A = -9e13;").CheckValue(false, "B");
            slate.Perform("A =  inf; ").CheckValue(true,  "B");
            slate.Perform("A = -inf; ").CheckValue(true,  "B");
            slate.Perform("A =  nan; ").CheckValue(false, "B");
        }

        [TestMethod]
        [TestTag("isNaN:UnaryFuncs<Double, Bool>")]
        public void TestFunctions_isNaN_UnaryFuncs_Double_Bool() {
            Slate slate = new Slate().Perform("in double A; B := isNaN(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNaN<bool>");
            slate.Perform("A =  0.0; ").CheckValue(false, "B");
            slate.Perform("A =  9e13;").CheckValue(false, "B");
            slate.Perform("A = -9e13;").CheckValue(false, "B");
            slate.Perform("A =  inf; ").CheckValue(false, "B");
            slate.Perform("A = -inf; ").CheckValue(false, "B");
            slate.Perform("A =  nan; ").CheckValue(true,  "B");
        }

        [TestMethod]
        [TestTag("isNeg:UnaryFuncs<Double, Bool>")]
        public void TestFunctions_isNeg_UnaryFuncs_Double_Bool() {
            Slate slate = new Slate().Perform("in double A; B := isNeg(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNegative<bool>");
            slate.Perform("A =  0.0; ").CheckValue(false, "B");
            slate.Perform("A =  1.0; ").CheckValue(false, "B");
            slate.Perform("A = -1.0; ").CheckValue(true,  "B");
            slate.Perform("A =  9e13;").CheckValue(false, "B");
            slate.Perform("A = -9e13;").CheckValue(true,  "B");
            slate.Perform("A =  inf; ").CheckValue(false, "B");
            slate.Perform("A = -inf; ").CheckValue(true,  "B");
            slate.Perform("A =  nan; ").CheckValue(true,  "B");
        }

        [TestMethod]
        [TestTag("isNeg:UnaryFuncs<Int, Bool>")]
        public void TestFunctions_isNeg_UnaryFuncs_Int_Bool() {
            Slate slate = new Slate().Perform("in int A; B := isNeg(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNegative<bool>");
            slate.Perform("A = -1234567;").CheckValue(true,  "B");
            slate.Perform("A =      -42;").CheckValue(true,  "B");
            slate.Perform("A =       -1;").CheckValue(true,  "B");
            slate.Perform("A =        0;").CheckValue(false, "B");
            slate.Perform("A =        1;").CheckValue(false, "B");
            slate.Perform("A =       42;").CheckValue(false, "B");
            slate.Perform("A =  1234567;").CheckValue(false, "B");
        }

        [TestMethod]
        [TestTag("isNegInf:UnaryFuncs<Double, Bool>")]
        public void TestFunctions_isNegInf_UnaryFuncs_Double_Bool() {
            Slate slate = new Slate().Perform("in double A; B := isNegInf(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNegativeInfinity<bool>");
            slate.Perform("A =  0.0; ").CheckValue(false, "B");
            slate.Perform("A =  9e13;").CheckValue(false, "B");
            slate.Perform("A = -9e13;").CheckValue(false, "B");
            slate.Perform("A =  inf; ").CheckValue(false, "B");
            slate.Perform("A = -inf; ").CheckValue(true,  "B");
            slate.Perform("A =  nan; ").CheckValue(false, "B");
        }

        [TestMethod]
        [TestTag("isNormal:UnaryFuncs<Double, Bool>")]
        public void TestFunctions_isNormal_UnaryFuncs_Double_Bool() {
            Slate slate = new Slate().Perform("in double A; B := isNormal(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNormal<bool>");
            slate.Perform("A =  0.0;   ").CheckValue(false, "B");
            slate.Perform("A =  1e96;  ").CheckValue(true,  "B");
            slate.Perform("A = -1e-96; ").CheckValue(true,  "B");
            slate.Perform("A =  1e-307;").CheckValue(true,  "B");
            slate.Perform("A = -1e-307;").CheckValue(true,  "B");
            slate.Perform("A =  1e-308;").CheckValue(false, "B");
            slate.Perform("A = -1e-308;").CheckValue(false, "B");
            slate.Perform("A =  1e308; ").CheckValue(true,  "B");
            slate.Perform("A = -1e308; ").CheckValue(true,  "B");
            slate.Perform("A =  1e309; ").CheckValue(false, "B");
            slate.Perform("A = -1e309; ").CheckValue(false, "B");
            slate.Perform("A =  inf;   ").CheckValue(false, "B");
            slate.Perform("A = -inf;   ").CheckValue(false, "B");
            slate.Perform("A =  nan;   ").CheckValue(false, "B");
        }

        [TestMethod]
        [TestTag("isNull:UnaryFuncs<Object, Bool>")]
        public void TestFunctions_isNull_UnaryFuncs_Object_Bool() {
            Slate slate = new Slate().Perform("in object A; B := isNull(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNull<bool>");
            slate.Perform("A = true;   ").CheckValue(false, "B");
            slate.Perform("A = 10;     ").CheckValue(false, "B");
            slate.Perform("A = 12.34;  ").CheckValue(false, "B");
            slate.Perform("A = 'Hello';").CheckValue(false, "B");
            slate.Perform("A = null;   ").CheckValue(true,  "B");
        }

        [TestMethod]
        [TestTag("isPosInf:UnaryFuncs<Double, Bool>")]
        public void TestFunctions_isPosInf_UnaryFuncs_Double_Bool() {
            Slate slate = new Slate().Perform("in double A; B := isPosInf(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsPositiveInfinity<bool>");
            slate.Perform("A =  0.0; ").CheckValue(false, "B");
            slate.Perform("A =  9e13;").CheckValue(false, "B");
            slate.Perform("A = -9e13;").CheckValue(false, "B");
            slate.Perform("A =  inf; ").CheckValue(true,  "B");
            slate.Perform("A = -inf; ").CheckValue(false, "B");
            slate.Perform("A =  nan; ").CheckValue(false, "B");
        }

        [TestMethod]
        [TestTag("isSubnormal:UnaryFuncs<Double, Bool>")]
        public void TestFunctions_isSubnormal_UnaryFuncs_Double_Bool() {
            Slate slate = new Slate().Perform("in double A; B := isSubnormal(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsSubnormal<bool>");
            slate.Perform("A =  0.0;   ").CheckValue(false, "B");
            slate.Perform("A =  1e96;  ").CheckValue(false, "B");
            slate.Perform("A = -1e-96; ").CheckValue(false, "B");
            slate.Perform("A =  1e-307;").CheckValue(false, "B");
            slate.Perform("A = -1e-307;").CheckValue(false, "B");
            slate.Perform("A =  1e-308;").CheckValue(true,  "B");
            slate.Perform("A = -1e-308;").CheckValue(true,  "B");
            slate.Perform("A =  1e308; ").CheckValue(false, "B");
            slate.Perform("A = -1e308; ").CheckValue(false, "B");
            slate.Perform("A =  1e309; ").CheckValue(false, "B");
            slate.Perform("A = -1e309; ").CheckValue(false, "B");
            slate.Perform("A =  inf;   ").CheckValue(false, "B");
            slate.Perform("A = -inf;   ").CheckValue(false, "B");
            slate.Perform("A =  nan;   ").CheckValue(false, "B");
        }

        [TestMethod]
        [TestTag("latch:Latch<Bool>")]
        public void TestFunctions_latch_Latch_Bool() {
            Slate slate = new Slate().Perform("in trigger A; in bool B; C := latch(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Latch<bool>");
            slate.Perform("true  -> A; B = true; ").CheckValue(true,  "C");
            slate.Perform("true  -> A; B = false;").CheckValue(false, "C");
            slate.Perform("false -> A; B = true; ").CheckValue(false, "C");
            slate.Perform("true  -> A;           ").CheckValue(true,  "C");
            slate.Perform("false -> A; B = false;").CheckValue(true,  "C");
            slate.Perform("            B = false;").CheckValue(true,  "C");
            slate.Perform("true  -> A; B = false;").CheckValue(false, "C");
        }

        [TestMethod]
        [TestTag("latch:Latch<Double>")]
        public void TestFunctions_latch_Latch_Double() {
            Slate slate = new Slate().Perform("in trigger A; in double B; C := latch(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Latch<double>");
            slate.Perform("true -> A; B = 0.0;").CheckValue(0.0, "C");
            slate.Perform("true -> A; B = 1.1;").CheckValue(1.1, "C");
            slate.Perform("true -> A; B = 2.2;").CheckValue(2.2, "C");
            slate.Perform("           B = 1.1;").CheckValue(2.2, "C");
            slate.Perform("           B = 0.0;").CheckValue(2.2, "C");
            slate.Perform("true -> A;         ").CheckValue(0.0, "C");
        }

        [TestMethod]
        [TestTag("latch:Latch<Int>")]
        public void TestFunctions_latch_Latch_Int() {
            Slate slate = new Slate().Perform("in trigger A; in int B; C := latch(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Latch<int>");
            slate.Perform("true -> A; B = 0;").CheckValue(0, "C");
            slate.Perform("true -> A; B = 1;").CheckValue(1, "C");
            slate.Perform("true -> A; B = 2;").CheckValue(2, "C");
            slate.Perform("           B = 1;").CheckValue(2, "C");
            slate.Perform("           B = 0;").CheckValue(2, "C");
            slate.Perform("true -> A;       ").CheckValue(0, "C");
        }

        [TestMethod]
        [TestTag("latch:Latch<Object>")]
        public void TestFunctions_latch_Latch_Object() {
            Slate slate = new Slate().Perform("in trigger A; in object B; C := latch(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Latch<object>");
            slate.Perform("true -> A; B = 'Hello';").CheckObject("Hello", "C");
            slate.Perform("true -> A; B = 2;      ").CheckObject(2,       "C");
            slate.Perform("true -> A; B = false;  ").CheckObject(false,   "C");
            slate.Perform("           B = 'World';").CheckObject(false,   "C");
            slate.Perform("           B = 3.14;   ").CheckObject(false,   "C");
            slate.Perform("true -> A;             ").CheckObject(3.14,    "C");
        }

        [TestMethod]
        [TestTag("latch:Latch<String>")]
        public void TestFunctions_latch_Latch_String() {
            Slate slate = new Slate().Perform("in trigger A; in string B; C := latch(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Latch<string>");
            slate.Perform("true -> A; B = 'A';").CheckValue("A", "C");
            slate.Perform("true -> A; B = 'B';").CheckValue("B", "C");
            slate.Perform("true -> A; B = 'C';").CheckValue("C", "C");
            slate.Perform("           B = 'D';").CheckValue("C", "C");
            slate.Perform("           B = 'E';").CheckValue("C", "C");
            slate.Perform("true -> A;         ").CheckValue("E", "C");
        }

        [TestMethod]
        [TestTag("lerp:Lerp<Double>")]
        public void TestFunctions_lerp_Lerp_Double() {
            Slate slate = new Slate().Perform("in double A, Min, Max; B := lerp(A, Min, Max);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Lerp<double>");
            slate.Perform("A = -1.0; Min = 0.0; Max = 1.0;").CheckValue(0.0, "B");
            slate.Perform("A =  0.0; Min = 0.0; Max = 1.0;").CheckValue(0.0, "B");
            slate.Perform("A =  0.5; Min = 0.0; Max = 1.0;").CheckValue(0.5, "B");
            slate.Perform("A =  1.0; Min = 0.0; Max = 1.0;").CheckValue(1.0, "B");
            slate.Perform("A =  2.0; Min = 0.0; Max = 1.0;").CheckValue(1.0, "B");

            slate.Perform("A = -1.0; Min = -10.0; Max = 10.0;").CheckValue(-10.0, "B");
            slate.Perform("A =  0.0; Min = -10.0; Max = 10.0;").CheckValue(-10.0, "B");
            slate.Perform("A =  0.5; Min = -10.0; Max = 10.0;").CheckValue(  0.0, "B");
            slate.Perform("A =  1.0; Min = -10.0; Max = 10.0;").CheckValue( 10.0, "B");
            slate.Perform("A =  2.0; Min = -10.0; Max = 10.0;").CheckValue( 10.0, "B");

            slate.Perform("A = 0.5; Min = 6.0; Max = 8.0;").CheckValue(7.0, "B");
        }

        [TestMethod]
        [TestTag("log:BinaryFunc<Double, Double, Double>")]
        public void TestFunctions_log_BinaryFunc_Double_Double_Double() {
            Slate slate = new Slate().Perform("in double A, B; C := log(A, B);");
            slate.CheckNodeString(Stringifier.Basic(), "C", "C: Log<double>");
            slate.Perform("A =  4.0; B = 2.0;").CheckValue(2.0, "C");
            slate.Perform("A =  8.0; B = 2.0;").CheckValue(3.0, "C");
            slate.Perform("A =  9.0; B = 3.0;").CheckValue(2.0, "C");
            slate.Perform("A =  3.0; B = 9.0;").CheckValue(0.5, "C");
            slate.Perform("A = 12.2; B = 4.3;").CheckValue(S.Math.Log(12.2, 4.3), "C");
        }

        [TestMethod]
        [TestTag("log:UnaryFuncs<Double, Double>")]
        public void TestFunctions_log_UnaryFunc_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := log(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Log<double>");
            slate.Perform("A =  4.0;").CheckValue(S.Math.Log( 4.0), "B");
            slate.Perform("A =  8.0;").CheckValue(S.Math.Log( 8.0), "B");
            slate.Perform("A =  9.0;").CheckValue(S.Math.Log( 9.0), "B");
            slate.Perform("A =  3.0;").CheckValue(S.Math.Log( 3.0), "B");
            slate.Perform("A = 12.3;").CheckValue(S.Math.Log(12.3), "B");
        }

        [TestMethod]
        [TestTag("log10:UnaryFuncs<Double, Double>")]
        public void TestFunctions_log10_UnaryFunc_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := log10(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Log10<double>");
            slate.Perform("A =  4.0;").CheckValue(S.Math.Log10( 4.0), "B");
            slate.Perform("A =  8.0;").CheckValue(S.Math.Log10( 8.0), "B");
            slate.Perform("A =  9.0;").CheckValue(S.Math.Log10( 9.0), "B");
            slate.Perform("A =  3.0;").CheckValue(S.Math.Log10( 3.0), "B");
            slate.Perform("A = 12.3;").CheckValue(S.Math.Log10(12.3), "B");
        }

        [TestMethod]
        [TestTag("log2:UnaryFuncs<Double, Double>")]
        public void TestFunctions_log2_UnaryFunc_Double_Double() {
            Slate slate = new Slate().Perform("in double A; B := log2(A);");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Log2<double>");
            slate.Perform("A =  4.0;").CheckValue(S.Math.Log2( 4.0), "B");
            slate.Perform("A =  8.0;").CheckValue(S.Math.Log2( 8.0), "B");
            slate.Perform("A =  9.0;").CheckValue(S.Math.Log2( 9.0), "B");
            slate.Perform("A =  3.0;").CheckValue(S.Math.Log2( 3.0), "B");
            slate.Perform("A = 12.3;").CheckValue(S.Math.Log2(12.3), "B");
        }





    }
}
