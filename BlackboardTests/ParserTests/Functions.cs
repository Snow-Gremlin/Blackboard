﻿using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S = System;

namespace BlackboardTests.ParserTests;

[TestClass]
public class Functions {

    [TestMethod]
    public void CheckAllFunctionsAreTested() =>
        TestTools.SetEntriesMatch(
            TestTools.FuncDefTags(new Slate().Global).WhereNot(tag =>
                tag.StartsWith(Blackboard.Core.Innate.Operators.Namespace)),
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
    [TestTag("abs:Abs<Float>")]
    public void TestFunctions_abs_Abs_Float() {
        Slate slate = new Slate().Perform("in float A; B := abs(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Abs<float>");
        slate.Perform("A = (float)  0.0; ").CheckValue(0.0f,   "B");
        slate.Perform("A = (float)  1.0; ").CheckValue(1.0f,   "B");
        slate.Perform("A = (float) -1.0; ").CheckValue(1.0f,   "B");
        slate.Perform("A = (float) 42.03;").CheckValue(42.03f, "B");
        slate.Perform("A = (float)-42.03;").CheckValue(42.03f, "B");
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
    [TestTag("acos:UnaryFuncs<Float, Float>")]
    public void TestFunctions_acos_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := acos(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Acos<float>");
        slate.Perform("A =(float) 0.0;").CheckValue(float.Pi*0.5f, "B");
        slate.Perform("A =(float) 1.0;").CheckValue(0.0f,          "B");
        slate.Perform("A =(float)-1.0;").CheckValue(float.Pi,      "B");
        slate.Perform("A =(float) 1.1;").CheckValue(float.NaN,     "B");
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
    [TestTag("acosh:UnaryFuncs<Float, Float>")]
    public void TestFunctions_acosh_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := acosh(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Acosh<float>");
        slate.Perform("A = (float)0.0;   ").CheckValue(float.NaN,                   "B");
        slate.Perform("A = (float)1.0;   ").CheckValue((float)S.Math.Acosh(1.0),    "B");
        slate.Perform("A = (float)1.5;   ").CheckValue((float)S.Math.Acosh(1.5),    "B");
        slate.Perform("A = (float)1000.0;").CheckValue((float)S.Math.Acosh(1000.0), "B");
    }

    [TestMethod]
    [TestTag("all:All")]
    public void TestFunctions_all_All() {
        Slate slate = new Slate().Perform("in trigger A, B, C, D; E := all(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: All<trigger>");
        slate.PerformWithoutReset("A = false; B = false; C = false; D = false;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("A = true;  B = false; C = false; D = false;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("A = false; B = true;  C = false; D = false;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("A = false; B = false; C = true;  D = false;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("A = false; B = false; C = false; D = true; ").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("A = true;  B = true;  C = true;  D = true; ").CheckProvoked(true,  "E").FinishEvaluation();
        slate.Perform("F := all(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<trigger>[](A)");
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
        slate.Perform("F := and(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<bool>[true](A)");
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
        slate.Perform("F := and(A);"); // Single pass through, F is set to A. with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<int>[15](A)");
        // The zero input scenario is checked in TestFunctions_and_And.
    }

    [TestMethod]
    [TestTag("any:Any")]
    public void TestFunctions_any_Any() {
        Slate slate = new Slate().Perform("in trigger A, B, C, D; E := any(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Any<trigger>");
        slate.PerformWithoutReset("A = false; B = false; C = false; D = false;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("A = true;  B = false; C = false; D = false;").CheckProvoked(true,  "E").FinishEvaluation();
        slate.PerformWithoutReset("A = false; B = true;  C = false; D = false;").CheckProvoked(true,  "E").FinishEvaluation();
        slate.PerformWithoutReset("A = false; B = false; C = true;  D = false;").CheckProvoked(true,  "E").FinishEvaluation();
        slate.PerformWithoutReset("A = false; B = false; C = false; D = true; ").CheckProvoked(true,  "E").FinishEvaluation();
        slate.PerformWithoutReset("A = true;  B = true;  C = true;  D = true; ").CheckProvoked(true,  "E").FinishEvaluation();
        slate.Perform("F := any(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<trigger>[](A)");
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
    [TestTag("asin:UnaryFuncs<Float, Float>")]
    public void TestFunctions_asin_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := asin(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Asin<float>");
        slate.Perform("A = (float) 0.0;").CheckValue(0.0f,                    "B");
        slate.Perform("A = (float) 1.0;").CheckValue((float)( S.Math.PI*0.5), "B");
        slate.Perform("A = (float)-1.0;").CheckValue((float)(-S.Math.PI*0.5), "B");
        slate.Perform("A = (float) 2.0;").CheckValue(float.NaN,               "B");
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
    [TestTag("asinh:UnaryFuncs<Float, Float>")]
    public void TestFunctions_asinh_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := asinh(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Asinh<float>");
        slate.Perform("A = (float) 0.0;").CheckValue(float.Asinh( 0.0f), "B");
        slate.Perform("A = (float) 1.0;").CheckValue(float.Asinh( 1.0f), "B");
        slate.Perform("A = (float)-1.0;").CheckValue(float.Asinh(-1.0f), "B");
        slate.Perform("A = (float) 2.0;").CheckValue(float.Asinh( 2.0f), "B");
        slate.Perform("A = (float)10.0;").CheckValue(float.Asinh(10.0f), "B");
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
    [TestTag("atan:BinaryFunc<Float, Float, Float>")]
    public void TestFunctions_atan_BinaryFunc_Float_Float_Float() {
        Slate slate = new Slate().Perform("in float A, B; C := atan(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Atan2<float>");
        slate.Perform("A = (float) 0.0; B = (float) 0.0;").CheckValue(0.0f,                        "C");
        slate.Perform("A = (float) 1.0; B = (float) 1.0;").CheckValue((float)( S.Math.PI*0.25),    "C");
        slate.Perform("A = (float)-1.0; B = (float)-1.0;").CheckValue((float)(-S.Math.PI*3.0/4.0), "C");
        slate.Perform("A = (float) 1.0; B = (float) 0.0;").CheckValue((float)( S.Math.PI*0.5),     "C");
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
    [TestTag("atan:UnaryFuncs<Float, Float>")]
    public void TestFunctions_atan_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := atan(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Atan<float>");
        slate.Perform("A = (float) 0.0;").CheckValue(0.0f,                     "B");
        slate.Perform("A = (float) 1.0;").CheckValue((float)( S.Math.PI*0.25), "B");
        slate.Perform("A = (float)-1.0;").CheckValue((float)(-S.Math.PI*0.25), "B");
        slate.Perform("A = (float) 2.0;").CheckValue((float)S.Math.Atan( 2.0), "B");
        slate.Perform("A = (float)10.0;").CheckValue((float)S.Math.Atan(10.0), "B");
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
    [TestTag("atanh:UnaryFuncs<Float, Float>")]
    public void TestFunctions_atanh_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := atanh(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Atanh<float>");
        slate.Perform("A = (float) 0.0;").CheckValue(0.0f,                      "B");
        slate.Perform("A = (float) 0.5;").CheckValue((float)S.Math.Atanh( 0.5), "B");
        slate.Perform("A = (float)-0.5;").CheckValue((float)S.Math.Atanh(-0.5), "B");
        slate.Perform("A = (float) 1.0;").CheckValue(float.PositiveInfinity,    "B");
        slate.Perform("A = (float)-1.0;").CheckValue(float.NegativeInfinity,    "B");
        slate.Perform("A = (float) 2.0;").CheckValue(float.NaN,                 "B");
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
        slate.Perform("F := average(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<double>[5](A)");
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
    [TestTag("cbrt:UnaryFuncs<Float, Float>")]
    public void TestFunctions_cbrt_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := cbrt(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Cbrt<float>");
        slate.Perform("A = (float) 0.0;").CheckValue( 0.0f, "B");
        slate.Perform("A = (float) 8.0;").CheckValue( 2.0f, "B");
        slate.Perform("A = (float)-8.0;").CheckValue(-2.0f, "B");
        slate.Perform("A = (float)27.0;").CheckValue( 3.0f, "B");
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
    [TestTag("ceiling:UnaryFuncs<Float, Float>")]
    public void TestFunctions_ceiling_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := ceiling(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Ceiling<float>");
        slate.Perform("A = (float) 0.0;").CheckValue( 0.0f, "B");
        slate.Perform("A = (float) 1.0;").CheckValue( 1.0f, "B");
        slate.Perform("A = (float)-1.0;").CheckValue(-1.0f, "B");
        slate.Perform("A = (float) 2.4;").CheckValue( 3.0f, "B");
        slate.Perform("A = (float)-2.4;").CheckValue(-2.0f, "B");
        slate.Perform("A = (float) 2.5;").CheckValue( 3.0f, "B");
        slate.Perform("A = (float)-2.5;").CheckValue(-2.0f, "B");
    }

    [TestMethod]
    [TestTag("clamp:TernaryFunc<Double, Double, Double, Double>")]
    public void TestFunctions_clamp_TernaryFunc_Double_Double_Double_Double() {
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
    [TestTag("clamp:TernaryFunc<Float, Float, Float, Float>")]
    public void TestFunctions_clamp_TernaryFunc_Float_Float_Float_Float() {
        Slate slate = new Slate().Perform("in float A, B, C; D := clamp(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Clamp<float>");
        slate.Perform("A = (float) 0.0; B = (float) 0.0; C = (float) 0.0;").CheckValue( 0.0f, "D");
        slate.Perform("A = (float)-0.1; B = (float) 0.0; C = (float) 1.0;").CheckValue( 0.0f, "D");
        slate.Perform("A = (float) 0.0; B = (float) 0.0; C = (float) 1.0;").CheckValue( 0.0f, "D");
        slate.Perform("A = (float) 0.1; B = (float) 0.0; C = (float) 1.0;").CheckValue( 0.1f, "D");
        slate.Perform("A = (float) 0.9; B = (float) 0.0; C = (float) 1.0;").CheckValue( 0.9f, "D");
        slate.Perform("A = (float) 1.0; B = (float) 0.0; C = (float) 1.0;").CheckValue( 1.0f, "D");
        slate.Perform("A = (float) 1.1; B = (float) 0.0; C = (float) 1.0;").CheckValue( 1.0f, "D");
        slate.Perform("A = (float)-4.0; B = (float)-1.0; C = (float)10.0;").CheckValue(-1.0f, "D");
        slate.Perform("A = (float) 6.0; B = (float)-1.0; C = (float)10.0;").CheckValue( 6.0f, "D");
        slate.Perform("A = (float)12.1; B = (float)-1.0; C = (float)10.0;").CheckValue(10.0f, "D");
    }

    [TestMethod]
    [TestTag("clamp:TernaryFunc<Int, Int, Int, Int>")]
    public void TestFunctions_clamp_TernaryFunc_Int_Int_Int_Int() {
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
    [TestTag("clamp:TernaryFunc<Uint, Uint, Uint, Uint>")]
    public void TestFunctions_clamp_TernaryFunc_Uint_Uint_Uint_Uint() {
        Slate slate = new Slate().Perform("in uint A, B, C; D := clamp(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Clamp<uint>");
        slate.Perform("A = (uint)0; B = (uint)0; C = (uint)0;").CheckValue((uint)0, "D");
        slate.Perform("A = (uint)0; B = (uint)1; C = (uint)3;").CheckValue((uint)1, "D");
        slate.Perform("A = (uint)1; B = (uint)1; C = (uint)3;").CheckValue((uint)1, "D");
        slate.Perform("A = (uint)2; B = (uint)1; C = (uint)3;").CheckValue((uint)2, "D");
        slate.Perform("A = (uint)3; B = (uint)1; C = (uint)3;").CheckValue((uint)3, "D");
        slate.Perform("A = (uint)4; B = (uint)1; C = (uint)3;").CheckValue((uint)3, "D");
    }

    [TestMethod]
    [TestTag("contains:BinaryFunc<String, String, Bool>")]
    public void TestFunctions_contains_BinaryFunc_String_String_Bool() {
        Slate slate = new Slate().Perform("in string A, B; C := contains(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Contains<bool>");
        slate.Perform("A = 'Hello'; B = 'H';   ").CheckValue(true,  "C");
        slate.Perform("A = 'Hello'; B = 'l';   ").CheckValue(true,  "C");
        slate.Perform("A = 'Hello'; B = 'Hell';").CheckValue(true,  "C");
        slate.Perform("A = 'Hello'; B = 'Help';").CheckValue(false, "C");
        slate.Perform("A = 'Hello'; B = '';    ").CheckValue(true,  "C");
        slate.Perform("A = '';      B = '';    ").CheckValue(true,  "C");
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
    [TestTag("cos:UnaryFuncs<Float, Float>")]
    public void TestFunctions_cos_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := cos(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Cos<float>");
        slate.Perform("A = (float)( 0.0);   ").CheckValue( 1.0f, "B");
        slate.Perform("A = (float)( pi*0.5);").CheckValue(float.Cos( float.Pi*0.5f), "B");
        slate.Perform("A = (float)(-pi*0.5);").CheckValue(float.Cos(-float.Pi*0.5f), "B");
        slate.Perform("A = (float)( pi);    ").CheckValue(-1.0f, "B");
        slate.Perform("A = (float)(-pi);    ").CheckValue(-1.0f, "B");
        slate.Perform("A = (float)( pi*1.5);").CheckValue(float.Cos( float.Pi*1.5f), "B");
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
    [TestTag("cosh:UnaryFuncs<Float, Float>")]
    public void TestFunctions_cosh_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := cosh(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Cosh<float>");
        slate.Perform("A = (float) 0.0;").CheckValue(1.0f, "B");
        slate.Perform("A = (float) 1.0;").CheckValue(float.Cosh( 1.0f), "B");
        slate.Perform("A = (float)-1.0;").CheckValue(float.Cosh(-1.0f), "B");
        slate.Perform("A = (float) 3.0;").CheckValue(float.Cosh( 3.0f), "B");
        slate.Perform("A = (float)12.3;").CheckValue(float.Cosh(12.3f), "B");
    }

    [TestMethod]
    [TestTag("endsWith:BinaryFunc<String, String, Bool>")]
    public void TestFunctions_endsWith_BinaryFunc_String_String_Bool() {
        Slate slate = new Slate().Perform("in string A, B; C := endsWith(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: EndsWith<bool>");
        slate.Perform("A = '';      B = '';  ").CheckValue(true,  "C");
        slate.Perform("A = 'Hello'; B = '';  ").CheckValue(true,  "C");
        slate.Perform("A = 'Hello'; B = 'He';").CheckValue(false, "C");
        slate.Perform("A = 'Hello'; B = 'lo';").CheckValue(true,  "C");
        slate.Perform("A = 'Hello'; B = 'do';").CheckValue(false, "C");
        slate.Perform("A = 'Hello'; B = 'o'; ").CheckValue(true,  "C");
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
    [TestTag("epsilon:EpsilonEqual<Float>")]
    public void TestFunctions_epsilon_EpsilonEqual_Float() {
        Slate slate = new Slate().Perform("in float A, B, C; D := epsilon(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: EpsilonEqual<bool>");
        slate.Perform("A = (float) 0.0; B = (float)-0.3; C = (float)0.2;").CheckValue(false, "D");
        slate.Perform("A = (float) 0.0; B = (float)-0.2; C = (float)0.2;").CheckValue(false, "D");
        slate.Perform("A = (float) 0.0; B = (float)-0.1; C = (float)0.2;").CheckValue(true,  "D");
        slate.Perform("A = (float) 0.0; B = (float) 0.0; C = (float)0.2;").CheckValue(true,  "D");
        slate.Perform("A = (float) 0.0; B = (float) 0.1; C = (float)0.2;").CheckValue(true,  "D");
        slate.Perform("A = (float) 0.0; B = (float) 0.2; C = (float)0.2;").CheckValue(false, "D");
        slate.Perform("A = (float) 0.0; B = (float) 0.3; C = (float)0.2;").CheckValue(false, "D");
        slate.Perform("A = (float)-0.3; B = (float) 0.0; C = (float)0.2;").CheckValue(false, "D");
        slate.Perform("A = (float)-0.2; B = (float) 0.0; C = (float)0.2;").CheckValue(false, "D");
        slate.Perform("A = (float)-0.1; B = (float) 0.0; C = (float)0.2;").CheckValue(true,  "D");
        slate.Perform("A = (float) 0.0; B = (float) 0.0; C = (float)0.2;").CheckValue(true,  "D");
        slate.Perform("A = (float) 0.1; B = (float) 0.0; C = (float)0.2;").CheckValue(true,  "D");
        slate.Perform("A = (float) 0.2; B = (float) 0.0; C = (float)0.2;").CheckValue(false, "D");
        slate.Perform("A = (float) 0.3; B = (float) 0.0; C = (float)0.2;").CheckValue(false, "D");
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
    [TestTag("exp:UnaryFuncs<Float, Float>")]
    public void TestFunctions_exp_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := exp(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Exp<float>");
        slate.Perform("A = (float)0.0;").CheckValue(1.0f, "B");
        slate.Perform("A = (float)1.0;").CheckValue(float.E, "B");
        slate.Perform("A = (float)2.0;").CheckValue(float.Exp(2.0f), "B");
        slate.Perform("A = (float)3.1;").CheckValue(float.Exp(3.1f), "B");
        slate.Perform("A = (float)4.2;").CheckValue(float.Exp(4.2f), "B");
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
    [TestTag("floor:UnaryFuncs<Float, Float>")]
    public void TestFunctions_floor_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := floor(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Floor<float>");
        slate.Perform("A = (float) 0.0;").CheckValue( 0.0f, "B");
        slate.Perform("A = (float) 1.0;").CheckValue( 1.0f, "B");
        slate.Perform("A = (float)-1.0;").CheckValue(-1.0f, "B");
        slate.Perform("A = (float) 2.4;").CheckValue( 2.0f, "B");
        slate.Perform("A = (float)-2.4;").CheckValue(-3.0f, "B");
        slate.Perform("A = (float) 2.5;").CheckValue( 2.0f, "B");
        slate.Perform("A = (float)-2.5;").CheckValue(-3.0f, "B");
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
    [TestTag("indexOf:BinaryFunc<String, String, Int>")]
    public void TestFunctions_indexOf_BinaryFunc_String_String_Int() {
        Slate slate = new Slate().Perform("in string A, B; C := indexOf(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: IndexOf<int>");
        slate.Perform("A = '';      B = '';  ").CheckValue( 0, "C");
        slate.Perform("A = 'Hello'; B = 'l'; ").CheckValue( 2, "C");
        slate.Perform("A = 'Hello'; B = 'lo';").CheckValue( 3, "C");
        slate.Perform("A = 'Hello'; B = 'x'; ").CheckValue(-1, "C");
    }

    [TestMethod]
    [TestTag("indexOf:TernaryFunc<String, String, Int, Int>")]
    public void TestFunctions_indexOf_TernaryFunc_String_String_Int_Int() {
        Slate slate = new Slate().Perform("in string A, B; in int C; D := indexOf(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: IndexOf<int>");
        slate.Perform("A = '';      B = '';   C = 0;").CheckValue( 0, "D");
        slate.Perform("A = 'Hello'; B = 'l';  C = 0;").CheckValue( 2, "D");
        slate.Perform("A = 'Hello'; B = 'l';  C = 1;").CheckValue( 2, "D");
        slate.Perform("A = 'Hello'; B = 'l';  C = 2;").CheckValue( 2, "D");
        slate.Perform("A = 'Hello'; B = 'l';  C = 3;").CheckValue( 3, "D");
        slate.Perform("A = 'Hello'; B = 'l';  C = 4;").CheckValue(-1, "D");
        slate.Perform("A = 'Hello'; B = 'lo'; C = 0;").CheckValue( 3, "D");
        slate.Perform("A = 'Hello'; B = 'x';  C = 0;").CheckValue(-1, "D");
    }

    [TestMethod]
    [TestTag("inRange:TernaryFunc<Double, Double, Double, Bool>")]
    public void TestFunctions_inRange_TernaryFunc_Double_Double_Double_Bool() {
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
    [TestTag("inRange:TernaryFunc<Float, Float, Float, Bool>")]
    public void TestFunctions_inRange_TernaryFunc_Float_Float_Float_Bool() {
        Slate slate = new Slate().Perform("in float A, Min, Max; B := inRange(A, Min, Max);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: InRange<bool>");
        slate.Perform("A = (float)0.0; Min = (float)0.0; Max = (float)0.0;").CheckValue(true,  "B");
        slate.Perform("A = (float)0.5; Min = (float)1.0; Max = (float)2.0;").CheckValue(false, "B");
        slate.Perform("A = (float)1.0; Min = (float)1.0; Max = (float)2.0;").CheckValue(true,  "B");
        slate.Perform("A = (float)1.5; Min = (float)1.0; Max = (float)2.0;").CheckValue(true,  "B");
        slate.Perform("A = (float)2.0; Min = (float)1.0; Max = (float)2.0;").CheckValue(true,  "B");
        slate.Perform("A = (float)2.5; Min = (float)1.0; Max = (float)2.0;").CheckValue(false, "B");
        slate.Perform("A = (float)2.0; Min = (float)3.0; Max = (float)1.0;").CheckValue(false, "B"); // Min/max reversed
    }

    [TestMethod]
    [TestTag("inRange:TernaryFunc<Int, Int, Int, Bool>")]
    public void TestFunctions_inRange_TernaryFunc_Int_Int_Int_Bool() {
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
    [TestTag("inRange:TernaryFunc<Uint, Uint, Uint, Bool>")]
    public void TestFunctions_inRange_TernaryFunc_Uint_Uint_Uint_Bool() {
        Slate slate = new Slate().Perform("in uint A, Min, Max; B := inRange(A, Min, Max);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: InRange<bool>");
        slate.Perform("A = (uint)0; Min = (uint)0; Max = (uint)0;").CheckValue(true,  "B");
        slate.Perform("A = (uint)0; Min = (uint)1; Max = (uint)3;").CheckValue(false, "B");
        slate.Perform("A = (uint)1; Min = (uint)1; Max = (uint)3;").CheckValue(true,  "B");
        slate.Perform("A = (uint)2; Min = (uint)1; Max = (uint)3;").CheckValue(true,  "B");
        slate.Perform("A = (uint)3; Min = (uint)1; Max = (uint)3;").CheckValue(true,  "B");
        slate.Perform("A = (uint)4; Min = (uint)1; Max = (uint)3;").CheckValue(false, "B");
        slate.Perform("A = (uint)2; Min = (uint)3; Max = (uint)1;").CheckValue(false, "B"); // Min/max reversed
    }

    [TestMethod]
    [TestTag("insert:TernaryFunc<String, Int, String, String>")]
    public void TestFunctions_insert_TernaryFunc_String_Int_String_String() {
        Slate slate = new Slate().Perform("in string A, C; in int B; D := insert(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Insert<string>");
        slate.Perform("A = '';      B = 0; C = '';   ").CheckValue("",         "D");
        slate.Perform("A = '';      B = 0; C = 'a';  ").CheckValue("a",        "D");
        slate.Perform("A = 'Cat';   B = 0; C = '';   ").CheckValue("Cat",      "D");
        slate.Perform("A = 'ab';    B = 0; C = 'c';  ").CheckValue("cab",      "D");
        slate.Perform("A = 'ab';    B = 1; C = 'c';  ").CheckValue("acb",      "D");
        slate.Perform("A = 'ab';    B = 2; C = 'c';  ").CheckValue("abc",      "D");
        slate.Perform("A = 'hello'; B = 3; C = 'p, ';").CheckValue("help, lo", "D");
    }

    [TestMethod]
    [TestTag("isEmpty:UnaryFuncs<String, Bool>")]
    public void TestFunctions_isEmpty_UnaryFuncs_String_Bool() {
        Slate slate = new Slate().Perform("in string A; B := isEmpty(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsEmpty<bool>");
        slate.Perform("A = 'Cat';").CheckValue(false, "B");
        slate.Perform("A = '';   ").CheckValue(true,  "B");
        slate.Perform("A = 'A';  ").CheckValue(false, "B");
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
    [TestTag("isFinite:UnaryFuncs<Float, Bool>")]
    public void TestFunctions_isFinite_UnaryFuncs_Float_Bool() {
        Slate slate = new Slate().Perform("in float A; B := isFinite(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsFinite<bool>");
        slate.Perform("A = (float) 0.0; ").CheckValue(true,  "B");
        slate.Perform("A = (float) 9e13;").CheckValue(true,  "B");
        slate.Perform("A = (float)-9e13;").CheckValue(true,  "B");
        slate.Perform("A = (float) inf; ").CheckValue(false, "B");
        slate.Perform("A = (float)-inf; ").CheckValue(false, "B");
        slate.Perform("A = (float) nan; ").CheckValue(false, "B");
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
    [TestTag("isInf:UnaryFuncs<Float, Bool>")]
    public void TestFunctions_isInf_UnaryFuncs_Float_Bool() {
        Slate slate = new Slate().Perform("in float A; B := isInf(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsInfinity<bool>");
        slate.Perform("A = (float) 0.0; ").CheckValue(false, "B");
        slate.Perform("A = (float) 9e13;").CheckValue(false, "B");
        slate.Perform("A = (float)-9e13;").CheckValue(false, "B");
        slate.Perform("A = (float) inf; ").CheckValue(true,  "B");
        slate.Perform("A = (float)-inf; ").CheckValue(true,  "B");
        slate.Perform("A = (float) nan; ").CheckValue(false, "B");
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
    [TestTag("isNaN:UnaryFuncs<Float, Bool>")]
    public void TestFunctions_isNaN_UnaryFuncs_Float_Bool() {
        Slate slate = new Slate().Perform("in float A; B := isNaN(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNaN<bool>");
        slate.Perform("A = (float) 0.0; ").CheckValue(false, "B");
        slate.Perform("A = (float) 9e13;").CheckValue(false, "B");
        slate.Perform("A = (float)-9e13;").CheckValue(false, "B");
        slate.Perform("A = (float) inf; ").CheckValue(false, "B");
        slate.Perform("A = (float)-inf; ").CheckValue(false, "B");
        slate.Perform("A = (float) nan; ").CheckValue(true,  "B");
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
    [TestTag("isNeg:UnaryFuncs<Float, Bool>")]
    public void TestFunctions_isNeg_UnaryFuncs_Float_Bool() {
        Slate slate = new Slate().Perform("in float A; B := isNeg(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNegative<bool>");
        slate.Perform("A = (float) 0.0; ").CheckValue(false, "B");
        slate.Perform("A = (float) 1.0; ").CheckValue(false, "B");
        slate.Perform("A = (float)-1.0; ").CheckValue(true,  "B");
        slate.Perform("A = (float) 9e13;").CheckValue(false, "B");
        slate.Perform("A = (float)-9e13;").CheckValue(true,  "B");
        slate.Perform("A = (float) inf; ").CheckValue(false, "B");
        slate.Perform("A = (float)-inf; ").CheckValue(true,  "B");
        slate.Perform("A = (float) nan; ").CheckValue(true,  "B");
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
    [TestTag("isNegInf:UnaryFuncs<Float, Bool>")]
    public void TestFunctions_isNegInf_UnaryFuncs_Float_Bool() {
        Slate slate = new Slate().Perform("in float A; B := isNegInf(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNegativeInfinity<bool>");
        slate.Perform("A = (float) 0.0; ").CheckValue(false, "B");
        slate.Perform("A = (float) 9e13;").CheckValue(false, "B");
        slate.Perform("A = (float)-9e13;").CheckValue(false, "B");
        slate.Perform("A = (float) inf; ").CheckValue(false, "B");
        slate.Perform("A = (float)-inf; ").CheckValue(true,  "B");
        slate.Perform("A = (float) nan; ").CheckValue(false, "B");
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
    [TestTag("isNormal:UnaryFuncs<Float, Bool>")]
    public void TestFunctions_isNormal_UnaryFuncs_Float_Bool() {
        Slate slate = new Slate().Perform("in double A; B := isNormal(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsNormal<bool>");
        slate.Perform("A = (float) 0.0;  ").CheckValue(false, "B");
        slate.Perform("A = (float) 1e26; ").CheckValue(true,  "B");
        slate.Perform("A = (float)-1e-26;").CheckValue(true,  "B");
        slate.Perform("A = (float) 1e-45;").CheckValue(true,  "B");
        slate.Perform("A = (float)-1e-45;").CheckValue(true,  "B");
        slate.Perform("A = (float) 1e-46;").CheckValue(false, "B");
        slate.Perform("A = (float)-1e-46;").CheckValue(false, "B");
        slate.Perform("A = (float) 1e38; ").CheckValue(true,  "B");
        slate.Perform("A = (float)-1e38; ").CheckValue(true,  "B");
        slate.Perform("A = (float) 1e39; ").CheckValue(false, "B");
        slate.Perform("A = (float)-1e39; ").CheckValue(false, "B");
        slate.Perform("A = (float) inf;  ").CheckValue(false, "B");
        slate.Perform("A = (float)-inf;  ").CheckValue(false, "B");
        slate.Perform("A = (float) nan;  ").CheckValue(false, "B");
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
    [TestTag("isPosInf:UnaryFuncs<Float, Bool>")]
    public void TestFunctions_isPosInf_UnaryFuncs_Float_Bool() {
        Slate slate = new Slate().Perform("in float A; B := isPosInf(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsPositiveInfinity<bool>");
        slate.Perform("A = (float) 0.0; ").CheckValue(false, "B");
        slate.Perform("A = (float) 9e13;").CheckValue(false, "B");
        slate.Perform("A = (float)-9e13;").CheckValue(false, "B");
        slate.Perform("A = (float) inf; ").CheckValue(true,  "B");
        slate.Perform("A = (float)-inf; ").CheckValue(false, "B");
        slate.Perform("A = (float) nan; ").CheckValue(false, "B");
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
    [TestTag("isSubnormal:UnaryFuncs<Float, Bool>")]
    public void TestFunctions_isSubnormal_UnaryFuncs_Float_Bool() {
        Slate slate = new Slate().Perform("in float A; B := isSubnormal(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: IsSubnormal<bool>");
        slate.Perform("A = (float) 0.0;  ").CheckValue(false, "B");
        slate.Perform("A = (float) 1e26; ").CheckValue(false, "B");
        slate.Perform("A = (float)-1e-26;").CheckValue(false, "B");
        slate.Perform("A = (float) 1e-37;").CheckValue(false, "B");
        slate.Perform("A = (float)-1e-37;").CheckValue(false, "B");
        slate.Perform("A = (float) 1e-38;").CheckValue(true,  "B");
        slate.Perform("A = (float)-1e-38;").CheckValue(true,  "B");
        slate.Perform("A = (float) 1e38; ").CheckValue(false, "B");
        slate.Perform("A = (float)-1e38; ").CheckValue(false, "B");
        slate.Perform("A = (float) 1e39; ").CheckValue(false, "B");
        slate.Perform("A = (float)-1e39; ").CheckValue(false, "B");
        slate.Perform("A = (float) inf;  ").CheckValue(false, "B");
        slate.Perform("A = (float)-inf;  ").CheckValue(false, "B");
        slate.Perform("A = (float) nan;  ").CheckValue(false, "B");
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
    [TestTag("latch:Latch<Float>")]
    public void TestFunctions_latch_Latch_Float() {
        Slate slate = new Slate().Perform("in trigger A; in float B; C := latch(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Latch<float>");
        slate.Perform("true -> A; B = (float)0.0;").CheckValue(0.0f, "C");
        slate.Perform("true -> A; B = (float)1.1;").CheckValue(1.1f, "C");
        slate.Perform("true -> A; B = (float)2.2;").CheckValue(2.2f, "C");
        slate.Perform("           B = (float)1.1;").CheckValue(2.2f, "C");
        slate.Perform("           B = (float)0.0;").CheckValue(2.2f, "C");
        slate.Perform("true -> A;                ").CheckValue(0.0f, "C");
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
    [TestTag("latch:Latch<Uint>")]
    public void TestFunctions_latch_Latch_Uint() {
        Slate slate = new Slate().Perform("in trigger A; in uint B; C := latch(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Latch<uint>");
        slate.Perform("true -> A; B = (uint)0;").CheckValue((uint)0, "C");
        slate.Perform("true -> A; B = (uint)1;").CheckValue((uint)1, "C");
        slate.Perform("true -> A; B = (uint)2;").CheckValue((uint)2, "C");
        slate.Perform("           B = (uint)1;").CheckValue((uint)2, "C");
        slate.Perform("           B = (uint)0;").CheckValue((uint)2, "C");
        slate.Perform("true -> A;             ").CheckValue((uint)0, "C");
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
    [TestTag("length:UnaryFuncs<String, Int>")]
    public void TestFunctions_length_UnaryFuncs_String_Int() {
        Slate slate = new Slate().Perform("in string A; B := length(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Length<int>");
        slate.Perform("A = '';     ").CheckValue(0, "B");
        slate.Perform("A = 'a';    ").CheckValue(1, "B");
        slate.Perform("A = 'cat';  ").CheckValue(3, "B");
        slate.Perform("A = 'hello';").CheckValue(5, "B");
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
    [TestTag("lerp:Lerp<Float>")]
    public void TestFunctions_lerp_Lerp_Float() {
        Slate slate = new Slate().Perform("in float A, Min, Max; B := lerp(A, Min, Max);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Lerp<float>");
        slate.Perform("A = (float)-1.0; Min = (float)0.0; Max = (float)1.0;").CheckValue(0.0f, "B");
        slate.Perform("A = (float) 0.0; Min = (float)0.0; Max = (float)1.0;").CheckValue(0.0f, "B");
        slate.Perform("A = (float) 0.5; Min = (float)0.0; Max = (float)1.0;").CheckValue(0.5f, "B");
        slate.Perform("A = (float) 1.0; Min = (float)0.0; Max = (float)1.0;").CheckValue(1.0f, "B");
        slate.Perform("A = (float) 2.0; Min = (float)0.0; Max = (float)1.0;").CheckValue(1.0f, "B");

        slate.Perform("A = (float)-1.0; Min = (float)-10.0; Max = (float)10.0;").CheckValue(-10.0f, "B");
        slate.Perform("A = (float) 0.0; Min = (float)-10.0; Max = (float)10.0;").CheckValue(-10.0f, "B");
        slate.Perform("A = (float) 0.5; Min = (float)-10.0; Max = (float)10.0;").CheckValue(  0.0f, "B");
        slate.Perform("A = (float) 1.0; Min = (float)-10.0; Max = (float)10.0;").CheckValue( 10.0f, "B");
        slate.Perform("A = (float) 2.0; Min = (float)-10.0; Max = (float)10.0;").CheckValue( 10.0f, "B");

        slate.Perform("A = (float)0.5; Min = (float)6.0; Max = (float)8.0;").CheckValue(7.0f, "B");
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
    [TestTag("log:BinaryFunc<Float, Float, Float>")]
    public void TestFunctions_log_BinaryFunc_Float_Float_Float() {
        Slate slate = new Slate().Perform("in float A, B; C := log(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Log<float>");
        slate.Perform("A = (float) 4.0; B = (float)2.0;").CheckValue(2.0f, "C");
        slate.Perform("A = (float) 8.0; B = (float)2.0;").CheckValue(3.0f, "C");
        slate.Perform("A = (float) 9.0; B = (float)3.0;").CheckValue(2.0f, "C");
        slate.Perform("A = (float) 3.0; B = (float)9.0;").CheckValue(0.5f, "C");
        slate.Perform("A = (float)12.2; B = (float)4.3;").CheckValue((float)S.Math.Log(12.2f, 4.3f), "C");
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
    [TestTag("log:UnaryFuncs<Float, Float>")]
    public void TestFunctions_log_UnaryFunc_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := log(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Log<float>");
        slate.Perform("A = (float) 4.0;").CheckValue(float.Log( 4.0f), "B");
        slate.Perform("A = (float) 8.0;").CheckValue(float.Log( 8.0f), "B");
        slate.Perform("A = (float) 9.0;").CheckValue(float.Log( 9.0f), "B");
        slate.Perform("A = (float) 3.0;").CheckValue(float.Log( 3.0f), "B");
        slate.Perform("A = (float)12.3;").CheckValue(float.Log(12.3f), "B");
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
    [TestTag("log10:UnaryFuncs<Float, Float>")]
    public void TestFunctions_log10_UnaryFunc_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := log10(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Log10<float>");
        slate.Perform("A = (float) 4.0;").CheckValue(float.Log10( 4.0f), "B");
        slate.Perform("A = (float) 8.0;").CheckValue(float.Log10( 8.0f), "B");
        slate.Perform("A = (float) 9.0;").CheckValue(float.Log10( 9.0f), "B");
        slate.Perform("A = (float) 3.0;").CheckValue(float.Log10( 3.0f), "B");
        slate.Perform("A = (float)12.3;").CheckValue(float.Log10(12.3f), "B");
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

    [TestMethod]
    [TestTag("log2:UnaryFuncs<Float, Float>")]
    public void TestFunctions_log2_UnaryFunc_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := log2(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Log2<float>");
        slate.Perform("A = (float) 4.0;").CheckValue(float.Log2( 4.0f), "B");
        slate.Perform("A = (float) 8.0;").CheckValue(float.Log2( 8.0f), "B");
        slate.Perform("A = (float) 9.0;").CheckValue(float.Log2( 9.0f), "B");
        slate.Perform("A = (float) 3.0;").CheckValue(float.Log2( 3.0f), "B");
        slate.Perform("A = (float)12.3;").CheckValue(float.Log2(12.3f), "B");
    }

    [TestMethod]
    [TestTag("max:Max<Double>")]
    public void TestFunctions_max_Max_Double() {
        Slate slate = new Slate().Perform("in double A, B, C, D; E := max(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Max<double>");
        slate.Perform("A = 1.0; B = 2.0; C = 3.0; D = 4.0;").CheckValue(4.0, "E");
        slate.Perform("C = 4.2;").CheckValue(4.2, "E");
        slate.Perform("B = 4.5;").CheckValue(4.5, "E");
        slate.Perform("B = 2.0;").CheckValue(4.2, "E");
        slate.Perform("A = 5.0;").CheckValue(5.0, "E");
        slate.Perform("A = 2.0; C = 2.0;").CheckValue(4.0, "E");
        slate.Perform("F := max(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<double>[2](A)");
        TestTools.CheckException(() => slate.Perform("G := max();"),
            "Error occurred while parsing input code.",
            "[Error: Could not perform the function without any inputs.",
            "   [Function: FuncGroup]",
            "   [Location: Unnamed:1, 9, 9]]");
    }

    [TestMethod]
    [TestTag("max:Max<Float>")]
    public void TestFunctions_max_Max_Float() {
        Slate slate = new Slate().Perform("in float A, B, C, D; E := max(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Max<float>");
        slate.Perform("A = (float)1.0; B = (float)2.0; C = (float)3.0; D = (float)4.0;").CheckValue(4.0f, "E");
        slate.Perform("C = (float)4.2;").CheckValue(4.2f, "E");
        slate.Perform("B = (float)4.5;").CheckValue(4.5f, "E");
        slate.Perform("B = (float)2.0;").CheckValue(4.2f, "E");
        slate.Perform("A = (float)5.0;").CheckValue(5.0f, "E");
        slate.Perform("A = (float)2.0; C = (float)2.0;").CheckValue(4.0f, "E");
        slate.Perform("F := max(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<float>[2](A)");
        // The zero input scenario is checked in TestFunctions_max_Max_Double.
    }

    [TestMethod]
    [TestTag("max:Max<Int>")]
    public void TestFunctions_max_Max_Int() {
        Slate slate = new Slate().Perform("in int A, B, C, D; E := max(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Max<int>");
        slate.Perform("A = 1; B = 2; C = 3; D = 4;").CheckValue(4, "E");
        slate.Perform("C = 5;").CheckValue(5, "E");
        slate.Perform("B = 6;").CheckValue(6, "E");
        slate.Perform("B = 2;").CheckValue(5, "E");
        slate.Perform("A = 7;").CheckValue(7, "E");
        slate.Perform("A = 2; C = 2;").CheckValue(4, "E");
        slate.Perform("F := max(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<int>[2](A)");
        // The zero input scenario is checked in TestFunctions_max_Max_Double.
    }

    [TestMethod]
    [TestTag("max:Max<Uint>")]
    public void TestFunctions_max_Max_Uint() {
        Slate slate = new Slate().Perform("in uint A, B, C, D; E := max(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Max<uint>");
        slate.Perform("A = (uint)1; B = (uint)2; C = (uint)3; D = (uint)4;").CheckValue((uint)4, "E");
        slate.Perform("C = (uint)5;").CheckValue((uint)5, "E");
        slate.Perform("B = (uint)6;").CheckValue((uint)6, "E");
        slate.Perform("B = (uint)2;").CheckValue((uint)5, "E");
        slate.Perform("A = (uint)7;").CheckValue((uint)7, "E");
        slate.Perform("A = (uint)2; C = (uint)2;").CheckValue((uint)4, "E");
        slate.Perform("F := max(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<uint>[2](A)");
        // The zero input scenario is checked in TestFunctions_max_Max_Double.
    }

    [TestMethod]
    [TestTag("min:Min<Double>")]
    public void TestFunctions_min_Min_Double() {
        Slate slate = new Slate().Perform("in double A, B, C, D; E := min(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Min<double>");
        slate.Perform("A = 1.0; B = 2.0; C = 3.0; D = 4.0;").CheckValue(1.0, "E");
        slate.Perform("C = 0.5;").CheckValue(0.5, "E");
        slate.Perform("B = 0.2;").CheckValue(0.2, "E");
        slate.Perform("B = 2.0;").CheckValue(0.5, "E");
        slate.Perform("A = 0.0;").CheckValue(0.0, "E");
        slate.Perform("A = 5.0; C = 4.0;").CheckValue(2.0, "E");
        slate.Perform("F := min(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<double>[5](A)");
        TestTools.CheckException(() => slate.Perform("G := min();"),
            "Error occurred while parsing input code.",
            "[Error: Could not perform the function without any inputs.",
            "   [Function: FuncGroup]",
            "   [Location: Unnamed:1, 9, 9]]");
    }

    [TestMethod]
    [TestTag("min:Min<Float>")]
    public void TestFunctions_min_Min_Float() {
        Slate slate = new Slate().Perform("in float A, B, C, D; E := min(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Min<float>");
        slate.Perform("A = (float)1.0; B = (float)2.0; C = (float)3.0; D = (float)4.0;").CheckValue(1.0f, "E");
        slate.Perform("C = (float)0.5;").CheckValue(0.5f, "E");
        slate.Perform("B = (float)0.2;").CheckValue(0.2f, "E");
        slate.Perform("B = (float)2.0;").CheckValue(0.5f, "E");
        slate.Perform("A = (float)0.0;").CheckValue(0.0f, "E");
        slate.Perform("A = (float)5.0; C = (float)4.0;").CheckValue(2.0f, "E");
        slate.Perform("F := min(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<float>[5](A)");
        // The zero input scenario is checked in TestFunctions_max_Max_Double.
    }

    [TestMethod]
    [TestTag("min:Min<Int>")]
    public void TestFunctions_min_Min_Int() {
        Slate slate = new Slate().Perform("in int A, B, C, D; E := min(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Min<int>");
        slate.Perform("A = 3; B = 6; C = 7; D = 8;").CheckValue(3, "E");
        slate.Perform("C = 2;").CheckValue(2, "E");
        slate.Perform("B = 1;").CheckValue(1, "E");
        slate.Perform("B = 4;").CheckValue(2, "E");
        slate.Perform("A = 0;").CheckValue(0, "E");
        slate.Perform("A = 7; C = 8;").CheckValue(4, "E");
        slate.Perform("F := min(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<int>[7](A)");
        // The zero input scenario is checked in TestFunctions_max_Max_Double.
    }

    [TestMethod]
    [TestTag("min:Min<Uint>")]
    public void TestFunctions_min_Min_Uint() {
        Slate slate = new Slate().Perform("in uint A, B, C, D; E := min(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Min<uint>");
        slate.Perform("A = (uint)3; B = (uint)6; C = (uint)7; D = (uint)8;").CheckValue((uint)3, "E");
        slate.Perform("C = (uint)2;").CheckValue((uint)2, "E");
        slate.Perform("B = (uint)1;").CheckValue((uint)1, "E");
        slate.Perform("B = (uint)4;").CheckValue((uint)2, "E");
        slate.Perform("A = (uint)0;").CheckValue((uint)0, "E");
        slate.Perform("A = (uint)7; C = (uint)8;").CheckValue((uint)4, "E");
        slate.Perform("F := min(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<uint>[7](A)");
        // The zero input scenario is checked in TestFunctions_max_Max_Double.
    }

    [TestMethod]
    [TestTag("mul:Mul<Double>")]
    public void TestFunctions_mul_Mul_Double() {
        Slate slate = new Slate().Perform("in double A, B, C, D; E := mul(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Mul<double>");
        slate.Perform("A =  1.0; B =  1.0; C =  1.0; D =  1.0;").CheckValue(  1.0, "E");
        slate.Perform("A =  2.0; B =  1.0; C =  1.0; D =  1.0;").CheckValue(  2.0, "E");
        slate.Perform("A =  1.0; B =  2.0; C =  1.0; D =  1.0;").CheckValue(  2.0, "E");
        slate.Perform("A =  1.0; B =  1.0; C =  2.0; D =  1.0;").CheckValue(  2.0, "E");
        slate.Perform("A =  1.0; B =  1.0; C =  1.0; D =  2.0;").CheckValue(  2.0, "E");
        slate.Perform("A =  2.0; B =  3.0; C =  4.0; D =  5.0;").CheckValue(120.0, "E");
        slate.Perform("A = -2.0; B =  1.0; C =  1.0; D =  1.0;").CheckValue( -2.0, "E");
        slate.Perform("A =  1.0; B = -2.0; C =  1.0; D =  1.0;").CheckValue( -2.0, "E");
        slate.Perform("A =  1.0; B =  1.0; C = -2.0; D =  1.0;").CheckValue( -2.0, "E");
        slate.Perform("A =  1.0; B =  1.0; C =  1.0; D = -2.0;").CheckValue( -2.0, "E");
        slate.Perform("F := mul(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<double>[1](A)");
        TestTools.CheckException(() => slate.Perform("G := mul();"),
            "Error occurred while parsing input code.",
            "[Error: Could not perform the function without any inputs.",
            "   [Function: FuncGroup]",
            "   [Location: Unnamed:1, 9, 9]]");
    }

    [TestMethod]
    [TestTag("mul:Mul<Float>")]
    public void TestFunctions_mul_Mul_Float() {
        Slate slate = new Slate().Perform("in float A, B, C, D; E := mul(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Mul<float>");
        slate.Perform("A = (float) 1.0; B = (float) 1.0; C = (float) 1.0; D = (float) 1.0;").CheckValue(  1.0f, "E");
        slate.Perform("A = (float) 2.0; B = (float) 1.0; C = (float) 1.0; D = (float) 1.0;").CheckValue(  2.0f, "E");
        slate.Perform("A = (float) 1.0; B = (float) 2.0; C = (float) 1.0; D = (float) 1.0;").CheckValue(  2.0f, "E");
        slate.Perform("A = (float) 1.0; B = (float) 1.0; C = (float) 2.0; D = (float) 1.0;").CheckValue(  2.0f, "E");
        slate.Perform("A = (float) 1.0; B = (float) 1.0; C = (float) 1.0; D = (float) 2.0;").CheckValue(  2.0f, "E");
        slate.Perform("A = (float) 2.0; B = (float) 3.0; C = (float) 4.0; D = (float) 5.0;").CheckValue(120.0f, "E");
        slate.Perform("A = (float)-2.0; B = (float) 1.0; C = (float) 1.0; D = (float) 1.0;").CheckValue( -2.0f, "E");
        slate.Perform("A = (float) 1.0; B = (float)-2.0; C = (float) 1.0; D = (float) 1.0;").CheckValue( -2.0f, "E");
        slate.Perform("A = (float) 1.0; B = (float) 1.0; C = (float)-2.0; D = (float) 1.0;").CheckValue( -2.0f, "E");
        slate.Perform("A = (float) 1.0; B = (float) 1.0; C = (float) 1.0; D = (float)-2.0;").CheckValue( -2.0f, "E");
        slate.Perform("F := mul(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<float>[1](A)");
        // The zero input scenario is checked in TestFunctions_max_Max_Double.
    }

    [TestMethod]
    [TestTag("mul:Mul<Int>")]
    public void TestFunctions_mul_Mul_Int() {
        Slate slate = new Slate().Perform("in int A, B, C, D; E := mul(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Mul<int>");
        slate.Perform("A =  1; B =  1; C =  1; D =  1;").CheckValue(  1, "E");
        slate.Perform("A =  2; B =  1; C =  1; D =  1;").CheckValue(  2, "E");
        slate.Perform("A =  1; B =  2; C =  1; D =  1;").CheckValue(  2, "E");
        slate.Perform("A =  1; B =  1; C =  2; D =  1;").CheckValue(  2, "E");
        slate.Perform("A =  1; B =  1; C =  1; D =  2;").CheckValue(  2, "E");
        slate.Perform("A =  2; B =  3; C =  4; D =  5;").CheckValue(120, "E");
        slate.Perform("A = -2; B =  1; C =  1; D =  1;").CheckValue( -2, "E");
        slate.Perform("A =  1; B = -2; C =  1; D =  1;").CheckValue( -2, "E");
        slate.Perform("A =  1; B =  1; C = -2; D =  1;").CheckValue( -2, "E");
        slate.Perform("A =  1; B =  1; C =  1; D = -2;").CheckValue( -2, "E");
        slate.Perform("F := mul(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<int>[1](A)");
        // The zero input scenario is checked in TestFunctions_max_Max_Double.
    }

    [TestMethod]
    [TestTag("mul:Mul<Uint>")]
    public void TestFunctions_mul_Mul_Uint() {
        Slate slate = new Slate().Perform("in uint A, B, C, D; E := mul(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Mul<uint>");
        slate.Perform("A = (uint) 1; B = (uint) 1; C = (uint) 1; D = (uint) 1;").CheckValue((uint)  1, "E");
        slate.Perform("A = (uint) 2; B = (uint) 1; C = (uint) 1; D = (uint) 1;").CheckValue((uint)  2, "E");
        slate.Perform("A = (uint) 1; B = (uint) 2; C = (uint) 1; D = (uint) 1;").CheckValue((uint)  2, "E");
        slate.Perform("A = (uint) 1; B = (uint) 1; C = (uint) 2; D = (uint) 1;").CheckValue((uint)  2, "E");
        slate.Perform("A = (uint) 1; B = (uint) 1; C = (uint) 1; D = (uint) 2;").CheckValue((uint)  2, "E");
        slate.Perform("A = (uint) 2; B = (uint) 3; C = (uint) 4; D = (uint) 5;").CheckValue((uint)120, "E");
        slate.Perform("F := mul(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<uint>[2](A)");
        // The zero input scenario is checked in TestFunctions_max_Max_Double.
    }

    [TestMethod]
    [TestTag("on:OnTrue")]
    public void TestFunctions_on_OnTrue() {
        Slate slate = new Slate().Perform("in bool A; B := on(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: OnTrue<trigger>");
        slate.PerformWithoutReset("A = true; ").CheckProvoked(true,  "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = true; ").CheckProvoked(true,  "B").FinishEvaluation();
        slate.PerformWithoutReset("A = true; ").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(false, "B").FinishEvaluation();
    }

    [TestMethod]
    [TestTag("onChange:OnChange")]
    public void TestFunctions_onChange_OnChange() {
        Slate slate = new Slate().Perform("in bool A; B := onChange(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: OnChange<trigger>");
        slate.PerformWithoutReset("A = true; ").CheckProvoked(true,  "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(true,  "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = true; ").CheckProvoked(true,  "B").FinishEvaluation();
        slate.PerformWithoutReset("A = true; ").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(true,  "B").FinishEvaluation();
    }

    [TestMethod]
    [TestTag("onFalse:OnFalse")]
    public void TestFunctions_onFalse_OnFalse() {
        Slate slate = new Slate().Perform("in bool A; B := onFalse(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: OnFalse<trigger>");
        slate.PerformWithoutReset("A = true; ").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(true,  "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = true; ").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = true; ").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(true,  "B").FinishEvaluation();
    }
        
    [TestMethod]
    [TestTag("onlyOne:OnlyOne")]
    public void TestFunctions_onlyOne_OnlyOne() {
        Slate slate = new Slate().Perform("in trigger A, B, C, D; E := onlyOne(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: OnlyOne<trigger>");
        slate.PerformWithoutReset("-> A;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> B;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> C;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> D;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> B;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> C;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> B -> C;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> B -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> C -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> B -> C;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> B -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> C -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> B -> C -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> B -> C -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.Perform("F := onlyOne(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<trigger>[](A)");
        TestTools.CheckException(() => slate.Perform("G := onlyOne();"),
            "Error occurred while parsing input code.",
            "[Error: Could not perform the function without any inputs.",
            "   [Function: FuncGroup]",
            "   [Location: Unnamed:1, 13, 13]]");
    }

    [TestMethod]
    [TestTag("onTrue:OnTrue")]
    public void TestFunctions_onTrue_OnTrue() {
        Slate slate = new Slate().Perform("in bool A; B := onTrue(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: OnTrue<trigger>");
        slate.PerformWithoutReset("A = true; ").CheckProvoked(true,  "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = true; ").CheckProvoked(true,  "B").FinishEvaluation();
        slate.PerformWithoutReset("A = true; ").CheckProvoked(false, "B").FinishEvaluation();
        slate.PerformWithoutReset("A = false;").CheckProvoked(false, "B").FinishEvaluation();
    }

    [TestMethod]
    [TestTag("or:BitwiseOr<Int>")]
    public void TestFunctions_or_BitwiseOr_Int() {
        Slate slate = new Slate().Perform("in int A, B, C, D; E := or(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: BitwiseOr<int>");
        slate.Perform("A = 0000b; B = 0000b; C = 0000b; D = 0000b;").CheckValue(0b0000, "E");
        slate.Perform("A = 1000b; B = 0100b; C = 0010b; D = 0001b;").CheckValue(0b1111, "E");
        slate.Perform("A = 1100b; B = 0110b; C = 0011b; D = 1111b;").CheckValue(0b1111, "E");
        slate.Perform("A = 1010b; B = 1001b; C = 1010b; D = 1011b;").CheckValue(0b1011, "E");
        slate.Perform("A = 1111b; B = 0000b; C = 0000b; D = 0000b;").CheckValue(0b1111, "E");
        slate.Perform("A = 0000b; B = 1111b; C = 0000b; D = 0000b;").CheckValue(0b1111, "E");
        slate.Perform("A = 0000b; B = 0000b; C = 1111b; D = 0000b;").CheckValue(0b1111, "E");
        slate.Perform("A = 0000b; B = 0000b; C = 0000b; D = 1111b;").CheckValue(0b1111, "E");
        slate.Perform("A = 1111b; B = 1111b; C = 1111b; D = 1111b;").CheckValue(0b1111, "E");
        slate.Perform("F := or(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<int>[15](A)");
        TestTools.CheckException(() => slate.Perform("G := or();"),
            "Error occurred while parsing input code.",
            "[Error: Could not perform the function without any inputs.",
            "   [Function: FuncGroup]",
            "   [Location: Unnamed:1, 8, 8]]");
    }
    
    [TestMethod]
    [TestTag("or:BitwiseOr<Uint>")]
    public void TestFunctions_or_BitwiseOr_Uint() {
        Slate slate = new Slate().Perform("in uint A, B, C, D; E := or(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: BitwiseOr<uint>");
        slate.Perform("A = (uint)0000b; B = (uint)0000b; C = (uint)0000b; D = (uint)0000b;").CheckValue((uint)0b0000, "E");
        slate.Perform("A = (uint)1000b; B = (uint)0100b; C = (uint)0010b; D = (uint)0001b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)1100b; B = (uint)0110b; C = (uint)0011b; D = (uint)1111b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)1010b; B = (uint)1001b; C = (uint)1010b; D = (uint)1011b;").CheckValue((uint)0b1011, "E");
        slate.Perform("A = (uint)1111b; B = (uint)0000b; C = (uint)0000b; D = (uint)0000b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)0000b; B = (uint)1111b; C = (uint)0000b; D = (uint)0000b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)0000b; B = (uint)0000b; C = (uint)1111b; D = (uint)0000b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)0000b; B = (uint)0000b; C = (uint)0000b; D = (uint)1111b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)1111b; B = (uint)1111b; C = (uint)1111b; D = (uint)1111b;").CheckValue((uint)0b1111, "E");
        slate.Perform("F := or(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<uint>[15](A)");
        // The zero input scenario is checked in TestFunctions_or_BitwiseOr_Int.
    }

    [TestMethod]
    [TestTag("or:Or")]
    public void TestFunctions_or_Or() {
        Slate slate = new Slate().Perform("in bool A, B, C, D; E := or(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Or<bool>");
        slate.Perform("A = false; B = false; C = false; D = false;").CheckValue(false, "E");
        slate.Perform("A = true;  B = false; C = false; D = false;").CheckValue(true,  "E");
        slate.Perform("A = false; B = true;  C = false; D = false;").CheckValue(true,  "E");
        slate.Perform("A = false; B = false; C = true;  D = false;").CheckValue(true,  "E");
        slate.Perform("A = false; B = false; C = false; D = true; ").CheckValue(true,  "E");
        slate.Perform("A = true;  B = true;  C = true;  D = true; ").CheckValue(true,  "E");
        slate.Perform("F := or(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<bool>[true](A)");
        // The zero input scenario is checked in TestFunctions_or_BitwiseOr_Int.
    }

    [TestMethod]
    [TestTag("padLeft:BinaryFunc<String, Int, String>")]
    public void TestFunctions_padLeft_BinaryFunc_String_Int_String() {
        Slate slate = new Slate().Perform("in string A; in int B; C := padLeft(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: PadLeft<string>");
        slate.Perform("A = '';      B =  0;").CheckValue("",            "C");
        slate.Perform("A = 'hello'; B =  3;").CheckValue("hello",       "C");
        slate.Perform("A = 'hello'; B =  5;").CheckValue("hello",       "C");
        slate.Perform("A = 'hello'; B =  7;").CheckValue("  hello",     "C");
        slate.Perform("A = 'hello'; B = 10;").CheckValue("     hello",  "C");
    }

    [TestMethod]
    [TestTag("padLeft:TernaryFunc<String, Int, String, String>")]
    public void TestFunctions_padLeft_TernaryFunc_String_Int_String_String() {
        Slate slate = new Slate().Perform("in string A, C; in int B; D := padLeft(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: PadLeft<string>");
        slate.Perform("A = '';      B =  0; C = '';     ").CheckValue("",            "D");
        slate.Perform("A = 'yo';    B = 10; C = '';     ").CheckValue("yo",          "D");
        slate.Perform("A = '';      B =  4; C = 'a';    ").CheckValue("aaaa",        "D");
        slate.Perform("A = '';      B =  4; C = 'ab';   ").CheckValue("abab",        "D");
        slate.Perform("A = '';      B =  3; C = 'ab';   ").CheckValue("aba",         "D");
        slate.Perform("A = '';      B = 11; C = 'abc';  ").CheckValue("abcabcabcab", "D");
        slate.Perform("A = 'hello'; B =  3; C = '-';    ").CheckValue("hello",       "D");
        slate.Perform("A = 'hello'; B =  5; C = '-';    ").CheckValue("hello",       "D");
        slate.Perform("A = 'hello'; B =  7; C = '-';    ").CheckValue("--hello",     "D");
        slate.Perform("A = 'hello'; B =  7; C = 'world';").CheckValue("wohello",     "D");
        slate.Perform("A = 'hello'; B = 10; C = 'world';").CheckValue("worldhello",  "D");
    }

    [TestMethod]
    [TestTag("padRight:BinaryFunc<String, Int, String>")]
    public void TestFunctions_padRight_BinaryFunc_String_Int_String() {
        Slate slate = new Slate().Perform("in string A; in int B; C := padRight(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: PadRight<string>");
        slate.Perform("A = '';      B =  0;").CheckValue("",           "C");
        slate.Perform("A = 'hello'; B =  3;").CheckValue("hello",      "C");
        slate.Perform("A = 'hello'; B =  5;").CheckValue("hello",      "C");
        slate.Perform("A = 'hello'; B =  7;").CheckValue("hello  ",    "C");
        slate.Perform("A = 'hello'; B = 10;").CheckValue("hello     ", "C");
    }

    [TestMethod]
    [TestTag("padRight:TernaryFunc<String, Int, String, String>")]
    public void TestFunctions_padRight_TernaryFunc_String_Int_String_String() {
        Slate slate = new Slate().Perform("in string A, C; in int B; D := padRight(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: PadRight<string>");
        // Several other scenarios are checked in TestFunctions_padLeft_TernaryFunc_String_Int_String_String.
        slate.Perform("A = '';      B =  0; C = '';     ").CheckValue("",           "D");
        slate.Perform("A = 'hello'; B =  3; C = '-';    ").CheckValue("hello",      "D");
        slate.Perform("A = 'hello'; B =  5; C = '-';    ").CheckValue("hello",      "D");
        slate.Perform("A = 'hello'; B =  7; C = '-';    ").CheckValue("hello--",    "D");
        slate.Perform("A = 'hello'; B =  7; C = 'world';").CheckValue("hellowo",    "D");
        slate.Perform("A = 'hello'; B = 10; C = 'world';").CheckValue("helloworld", "D");
    }

    [TestMethod]
    [TestTag("pow:BinaryFunc<Double, Double, Double>")]
    public void TestFunctions_pow_BinaryFunc_Double_Double_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := pow(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Pow<double>");
        slate.Perform("A =  4.0; B = 2.0;").CheckValue(16.0, "C");
        slate.Perform("A =  8.0; B = 2.0;").CheckValue(64.0, "C");
        slate.Perform("A =  3.0; B = 3.0;").CheckValue(27.0, "C");
        slate.Perform("A =  3.0; B = 9.0;").CheckValue(19683.0, "C");
        slate.Perform("A = 12.2; B = 4.3;").CheckValue(S.Math.Pow(12.2, 4.3), "C");
    }

    [TestMethod]
    [TestTag("pow:BinaryFunc<Float, Float, Float>")]
    public void TestFunctions_pow_BinaryFunc_Float_Float_Float() {
        Slate slate = new Slate().Perform("in float A, B; C := pow(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Pow<float>");
        slate.Perform("A = (float) 4.0; B = (float)2.0;").CheckValue(16.0f, "C");
        slate.Perform("A = (float) 8.0; B = (float)2.0;").CheckValue(64.0f, "C");
        slate.Perform("A = (float) 3.0; B = (float)3.0;").CheckValue(27.0f, "C");
        slate.Perform("A = (float) 3.0; B = (float)9.0;").CheckValue(19683.0f, "C");
        slate.Perform("A = (float)12.2; B = (float)4.3;").CheckValue(float.Pow(12.2f, 4.3f), "C");
    }

    [TestMethod]
    [TestTag("remainder:BinaryFunc<Double, Double, Double>")]
    public void TestFunctions_remainder_BinaryFunc_Double_Double_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := remainder(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: IEEERemainder<double>");
        slate.Perform("A =  4.0;  B =  2.0; ").CheckValue( 0.0, "C");
        slate.Perform("A =  1.0;  B =  4.0; ").CheckValue( 1.0, "C");
        slate.Perform("A =  8.0;  B =  3.0; ").CheckValue(-1.0, "C");
        slate.Perform("A =  8.0;  B = -3.0; ").CheckValue(-1.0, "C");
        slate.Perform("A = -8.0;  B =  3.0; ").CheckValue( 1.0, "C");
        slate.Perform("A = -8.0;  B = -3.0; ").CheckValue( 1.0, "C");
        slate.Perform("A =  1.0;  B =  0.0; ").CheckValue(double.NaN, "C");
        slate.Perform("A =  0.0;  B =  0.0; ").CheckValue(double.NaN, "C");
        slate.Perform("A = -1.0;  B =  0.0; ").CheckValue(double.NaN, "C");
        slate.Perform("A =  3.13; B =  0.25;").CheckValue(S.Math.IEEERemainder(3.13, 0.25), "C");
    }

    [TestMethod]
    [TestTag("remainder:BinaryFunc<Float, Float, Float>")]
    public void TestFunctions_remainder_BinaryFunc_Float_Float_Float() {
        Slate slate = new Slate().Perform("in float A, B; C := remainder(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: IEEERemainder<float>");
        slate.Perform("A = (float) 4.0;  B = (float) 2.0; ").CheckValue( 0.0f, "C");
        slate.Perform("A = (float) 1.0;  B = (float) 4.0; ").CheckValue( 1.0f, "C");
        slate.Perform("A = (float) 8.0;  B = (float) 3.0; ").CheckValue(-1.0f, "C");
        slate.Perform("A = (float) 8.0;  B = (float)-3.0; ").CheckValue(-1.0f, "C");
        slate.Perform("A = (float)-8.0;  B = (float) 3.0; ").CheckValue( 1.0f, "C");
        slate.Perform("A = (float)-8.0;  B = (float)-3.0; ").CheckValue( 1.0f, "C");
        slate.Perform("A = (float) 1.0;  B = (float) 0.0; ").CheckValue(float.NaN, "C");
        slate.Perform("A = (float) 0.0;  B = (float) 0.0; ").CheckValue(float.NaN, "C");
        slate.Perform("A = (float)-1.0;  B = (float) 0.0; ").CheckValue(float.NaN, "C");
        slate.Perform("A = (float) 3.13; B = (float) 0.25;").CheckValue((float)S.Math.IEEERemainder(3.13f, 0.25f), "C");
    }

    [TestMethod]
    [TestTag("remove:TernaryFunc<String, Int, Int, String>")]
    public void TestFunctions_remove_TernaryFunc_String_Int_Int_String() {
        Slate slate = new Slate().Perform("in string A; in int B, C; D := remove(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Remove<string>");
        slate.Perform("A = '';      B = 0; C = 0;").CheckValue("",     "D");
        slate.Perform("A = 'hello'; B = 0; C = 1;").CheckValue("ello", "D");
        slate.Perform("A = 'hello'; B = 4; C = 1;").CheckValue("hell", "D");
        slate.Perform("A = 'hello'; B = 0; C = 4;").CheckValue("o",    "D");
        slate.Perform("A = 'hello'; B = 0; C = 5;").CheckValue("",     "D");
    }

    [TestMethod]
    [TestTag("round:BinaryFunc<Double, Int, Double>")]
    public void TestFunctions_round_BinaryFunc_Double_Int_Double() {
        Slate slate = new Slate().Perform("in double A; in int B; C := round(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Round<double>");
        slate.Perform("A = 3.141592653589; B = 0; ").CheckValue(3.0, "C");
        slate.Perform("A = 3.141592653589; B = 1; ").CheckValue(3.1, "C");
        slate.Perform("A = 3.141592653589; B = 2; ").CheckValue(3.14, "C");
        slate.Perform("A = 3.141592653589; B = 4; ").CheckValue(3.1416, "C");
        slate.Perform("A = 3.141592653589; B = 8; ").CheckValue(3.14159265, "C");
        TestTools.CheckException(() => slate.Perform("B = -1;"),
            "Rounding digits must be between 0 and 15, inclusive. (Parameter 'digits')");
    }

    [TestMethod]
    [TestTag("round:BinaryFunc<Float, Int, Float>")]
    public void TestFunctions_round_BinaryFunc_Float_Int_Float() {
        Slate slate = new Slate().Perform("in float A; in int B; C := round(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Round<float>");
        slate.Perform("A = (float)3.141592653589; B = 0; ").CheckValue(3.0f, "C");
        slate.Perform("A = (float)3.141592653589; B = 1; ").CheckValue(3.1f, "C");
        slate.Perform("A = (float)3.141592653589; B = 2; ").CheckValue(3.14f, "C");
        slate.Perform("A = (float)3.141592653589; B = 4; ").CheckValue(3.1416f, "C");
        slate.Perform("A = (float)3.141592653589; B = 8; ").CheckValue(3.14159265f, "C");
        TestTools.CheckException(() => slate.Perform("B = -1;"),
            "Rounding digits must be between 0 and 15, inclusive. (Parameter 'digits')");
    }

    [TestMethod]
    [TestTag("round:UnaryFuncs<Double, Double>")]
    public void TestFunctions_round_UnaryFuncs_Double_Double() {
        Slate slate = new Slate().Perform("in double A; B := round(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Round<double>");
        slate.Perform("A =  3.14;  ").CheckValue( 3.0, "B");
        slate.Perform("A =  3.4999;").CheckValue( 3.0, "B");
        slate.Perform("A =  3.5;   ").CheckValue( 4.0, "B");
        slate.Perform("A =  3.6;   ").CheckValue( 4.0, "B");
        slate.Perform("A = 42.0;   ").CheckValue(42.0, "B");
    }

    [TestMethod]
    [TestTag("round:UnaryFuncs<Float, Float>")]
    public void TestFunctions_round_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := round(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Round<float>");
        slate.Perform("A = (float) 3.14;  ").CheckValue( 3.0f, "B");
        slate.Perform("A = (float) 3.4999;").CheckValue( 3.0f, "B");
        slate.Perform("A = (float) 3.5;   ").CheckValue( 4.0f, "B");
        slate.Perform("A = (float) 3.6;   ").CheckValue( 4.0f, "B");
        slate.Perform("A = (float)42.0;   ").CheckValue(42.0f, "B");
    }

    [TestMethod]
    [TestTag("select:SelectTrigger")]
    public void TestFunctions_select_SelectTrigger() {
        Slate slate = new Slate().Perform("in bool A; in trigger B, C; D := select(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<trigger>");
        slate.PerformWithoutReset("A = false; B = false; C = false;").CheckProvoked(false, "D").FinishEvaluation();
        slate.PerformWithoutReset("A = false; B = false; C = true; ").CheckProvoked(true,  "D").FinishEvaluation();
        slate.PerformWithoutReset("A = false; B = true;  C = false;").CheckProvoked(false, "D").FinishEvaluation();
        slate.PerformWithoutReset("A = false; B = true;  C = true; ").CheckProvoked(true,  "D").FinishEvaluation();
        slate.PerformWithoutReset("A = true;  B = false; C = false;").CheckProvoked(false, "D").FinishEvaluation();
        slate.PerformWithoutReset("A = true;  B = false; C = true; ").CheckProvoked(false, "D").FinishEvaluation();
        slate.PerformWithoutReset("A = true;  B = true;  C = false;").CheckProvoked(true,  "D").FinishEvaluation();
        slate.PerformWithoutReset("A = true;  B = true;  C = true; ").CheckProvoked(true,  "D").FinishEvaluation();
    }

    [TestMethod]
    [TestTag("select:SelectValue<Bool>")]
    public void TestFunctions_select_SelectValue_Bool() {
        Slate slate = new Slate().Perform("in bool A, B, C; D := select(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<bool>");
        slate.Perform("A = false; B = false; C = false;").CheckValue(false, "D");
        slate.Perform("A = false; B = false; C = true; ").CheckValue(true,  "D");
        slate.Perform("A = false; B = true;  C = false;").CheckValue(false, "D");
        slate.Perform("A = false; B = true;  C = true; ").CheckValue(true,  "D");
        slate.Perform("A = true;  B = false; C = false;").CheckValue(false, "D");
        slate.Perform("A = true;  B = false; C = true; ").CheckValue(false, "D");
        slate.Perform("A = true;  B = true;  C = false;").CheckValue(true,  "D");
        slate.Perform("A = true;  B = true;  C = true; ").CheckValue(true,  "D");
    }

    [TestMethod]
    [TestTag("select:SelectValue<Double>")]
    public void TestFunctions_select_SelectValue_Double() {
        Slate slate = new Slate().Perform("in bool A; in double B, C; D := select(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<double>");
        slate.Perform("A = false; B =  1.23; C = 32.1;").CheckValue(32.1, "D");
        slate.Perform("A = true;  B = 42.5;  C = 55.3;").CheckValue(42.5, "D");
    }

    [TestMethod]
    [TestTag("select:SelectValue<Float>")]
    public void TestFunctions_select_SelectValue_Float() {
        Slate slate = new Slate().Perform("in bool A; in float B, C; D := select(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<float>");
        slate.Perform("A = false; B = (float) 1.23; C = (float)32.1;").CheckValue(32.1f, "D");
        slate.Perform("A = true;  B = (float)42.5;  C = (float)55.3;").CheckValue(42.5f, "D");
    }

    [TestMethod]
    [TestTag("select:SelectValue<Int>")]
    public void TestFunctions_select_SelectValue_Int() {
        Slate slate = new Slate().Perform("in bool A; in int B, C; D := select(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<int>");
        slate.Perform("A = false; B = 10; C = 32;").CheckValue(32, "D");
        slate.Perform("A = true;  B = 42; C = 87;").CheckValue(42, "D");
    }

    [TestMethod]
    [TestTag("select:SelectValue<Uint>")]
    public void TestFunctions_select_SelectValue_Uint() {
        Slate slate = new Slate().Perform("in bool A; in uint B, C; D := select(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<uint>");
        slate.Perform("A = false; B = (uint)10; C = (uint)32;").CheckValue((uint)32, "D");
        slate.Perform("A = true;  B = (uint)42; C = (uint)87;").CheckValue((uint)42, "D");
    }

    [TestMethod]
    [TestTag("select:SelectValue<Object>")]
    public void TestFunctions_select_SelectValue_Object() {
        Slate slate = new Slate().Perform("in bool A; in object B, C; D := select(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<object>");
        slate.Perform("A = false; B = 'Goodbye'; C = 'Moon'; ").CheckObject("Moon",  "D");
        slate.Perform("A = true;  B = 'Hello';   C = 'World';").CheckObject("Hello", "D");
        slate.Perform("A = false; B = 'Goodbye'; C = 2;      ").CheckObject(2,       "D");
        slate.Perform("A = true;  B = false;     C = 0.2;    ").CheckObject(false,   "D");
        slate.Perform("A = false; B = true;      C = 0.4;    ").CheckObject(0.4,     "D");
        slate.Perform("A = true;  B = null;      C = 42;     ").CheckObject(null,    "D");
    }

    [TestMethod]
    [TestTag("select:SelectValue<String>")]
    public void TestFunctions_select_SelectValue_String() {
        Slate slate = new Slate().Perform("in bool A; in string B, C; D := select(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<string>");
        slate.Perform("A = false; B = 'Goodbye'; C = 'Moon'; ").CheckValue("Moon",  "D");
        slate.Perform("A = true;  B = 'Hello';   C = 'World';").CheckValue("Hello", "D");
    }

    [TestMethod]
    [TestTag("sin:UnaryFuncs<Double, Double>")]
    public void TestFunctions_sin_UnaryFuncs_Double_Double() {
        Slate slate = new Slate().Perform("in double A; B := sin(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Sin<double>");
        slate.Perform("A =  0.0;   ").CheckValue( 0.0, "B");
        slate.Perform("A =  pi*0.5;").CheckValue( 1.0, "B");
        slate.Perform("A = -pi*0.5;").CheckValue(-1.0, "B");
        slate.Perform("A =  pi;    ").CheckValue(S.Math.Sin( S.Math.PI), "B");
        slate.Perform("A = -pi;    ").CheckValue(S.Math.Sin(-S.Math.PI), "B");
        slate.Perform("A =  pi*1.5;").CheckValue(S.Math.Sin( S.Math.PI*1.5), "B");
    }

    [TestMethod]
    [TestTag("sin:UnaryFuncs<Float, Float>")]
    public void TestFunctions_sin_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := sin(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Sin<float>");
        slate.Perform("A = (float) 0.0;     ").CheckValue( 0.0f, "B");
        slate.Perform("A = (float)( pi*0.5);").CheckValue( 1.0f, "B");
        slate.Perform("A = (float)(-pi*0.5);").CheckValue(-1.0f, "B");
        slate.Perform("A = (float) pi;      ").CheckValue(float.Sin( float.Pi), "B");
        slate.Perform("A = (float)-pi;      ").CheckValue(float.Sin(-float.Pi), "B");
        slate.Perform("A = (float)( pi*1.5);").CheckValue(float.Sin( float.Pi*1.5f), "B");
    }

    [TestMethod]
    [TestTag("sinh:UnaryFuncs<Double, Double>")]
    public void TestFunctions_sinh_UnaryFuncs_Double_Double() {
        Slate slate = new Slate().Perform("in double A; B := sinh(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Sinh<double>");
        slate.Perform("A =  0.0;").CheckValue(0.0, "B");
        slate.Perform("A =  1.0;").CheckValue(S.Math.Sinh( 1.0), "B");
        slate.Perform("A = -1.0;").CheckValue(S.Math.Sinh(-1.0), "B");
        slate.Perform("A =  3.0;").CheckValue(S.Math.Sinh( 3.0), "B");
        slate.Perform("A = 12.3;").CheckValue(S.Math.Sinh(12.3), "B");
    }

    [TestMethod]
    [TestTag("sinh:UnaryFuncs<Float, Float>")]
    public void TestFunctions_sinh_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := sinh(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Sinh<float>");
        slate.Perform("A = (float) 0.0;").CheckValue(0.0f, "B");
        slate.Perform("A = (float) 1.0;").CheckValue(float.Sinh( 1.0f), "B");
        slate.Perform("A = (float)-1.0;").CheckValue(float.Sinh(-1.0f), "B");
        slate.Perform("A = (float) 3.0;").CheckValue(float.Sinh( 3.0f), "B");
        slate.Perform("A = (float)12.3;").CheckValue(float.Sinh(12.3f), "B");
    }

    [TestMethod]
    [TestTag("sqrt:UnaryFuncs<Double, Double>")]
    public void TestFunctions_sqrt_UnaryFuncs_Double_Double() {
        Slate slate = new Slate().Perform("in double A; B := sqrt(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Sqrt<double>");
        slate.Perform("A =  4.0; ").CheckValue(2.0, "B");
        slate.Perform("A =  9.0; ").CheckValue(3.0, "B");
        slate.Perform("A = 81.0; ").CheckValue(9.0, "B");
        slate.Perform("A =  1.21;").CheckValue(1.1, "B");
        slate.Perform("A = 12.1; ").CheckValue(S.Math.Sqrt(12.1), "B");
    }

    [TestMethod]
    [TestTag("sqrt:UnaryFuncs<Float, Float>")]
    public void TestFunctions_sqrt_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := sqrt(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Sqrt<float>");
        slate.Perform("A = (float) 4.0; ").CheckValue(2.0f, "B");
        slate.Perform("A = (float) 9.0; ").CheckValue(3.0f, "B");
        slate.Perform("A = (float)81.0; ").CheckValue(9.0f, "B");
        slate.Perform("A = (float) 1.21;").CheckValue(1.1f, "B");
        slate.Perform("A = (float)12.1; ").CheckValue(float.Sqrt(12.1f), "B");
    }

    [TestMethod]
    [TestTag("startsWith:BinaryFunc<String, String, Bool>")]
    public void TestFunctions_startsWith_BinaryFunc_String_String_Bool_() {
        Slate slate = new Slate().Perform("in string A, B; C := startsWith(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: StartsWith<bool>");
        slate.Perform("A = ''; B = '';").CheckValue(true, "C");
        slate.Perform("A = 'Hello'; B = '';   ").CheckValue(true,  "C");
        slate.Perform("A = 'Hello'; B = 'a';  ").CheckValue(false, "C");
        slate.Perform("A = 'Hello'; B = 'H';  ").CheckValue(true,  "C");
        slate.Perform("A = 'Hello'; B = 'He'; ").CheckValue(true,  "C");
        slate.Perform("A = 'Hello'; B = 'Hee';").CheckValue(false, "C");
        slate.Perform("A = 'Hello'; B = 'o';  ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("substring:TernaryFunc<String, Int, Int, String>")]
    public void TestFunctions_substring_TernaryFunc_String_Int_Int_String() {
        Slate slate = new Slate().Perform("in string A; in int B, C; D := substring(A, B, C);");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Substring<string>");
        slate.Perform("A = 'Hello'; B = 0; C = 0;").CheckValue("",      "D");
        slate.Perform("A = 'Hello'; B = 0; C = 1;").CheckValue("H",     "D");
        slate.Perform("A = 'Hello'; B = 1; C = 1;").CheckValue("e",     "D");
        slate.Perform("A = 'Hello'; B = 3; C = 2;").CheckValue("lo",    "D");
        slate.Perform("A = 'Hello'; B = 0; C = 5;").CheckValue("Hello", "D");
    }

    [TestMethod]
    [TestTag("sum:Sum<Double>")]
    public void TestFunctions_sum_Sum_Double() {
        Slate slate = new Slate().Perform("in double A, B, C, D; E := sum(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Sum<double>");
        slate.Perform("A = 0.0; B = 0.0; C = 0.0; D = 0.0;").CheckValue( 0.0, "E");
        slate.Perform("A = 1.0; B = 0.0; C = 0.0; D = 0.0;").CheckValue( 1.0, "E");
        slate.Perform("A = 0.0; B = 1.0; C = 0.0; D = 0.0;").CheckValue( 1.0, "E");
        slate.Perform("A = 0.0; B = 0.0; C = 1.0; D = 0.0;").CheckValue( 1.0, "E");
        slate.Perform("A = 0.0; B = 0.0; C = 0.0; D = 1.0;").CheckValue( 1.0, "E");
        slate.Perform("A = 1.1; B = 2.3; C = 4.3; D = 6.0;").CheckValue(13.7, "E");
        slate.Perform("F := sum(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<double>[1.1](A)");
        TestTools.CheckException(() => slate.Perform("G := sum();"),
            "Error occurred while parsing input code.",
            "[Error: Could not perform the function without any inputs.",
            "   [Function: FuncGroup]",
            "   [Location: Unnamed:1, 9, 9]]");
    }

    [TestMethod]
    [TestTag("sum:Sum<Float>")]
    public void TestFunctions_sum_Sum_Float() {
        Slate slate = new Slate().Perform("in float A, B, C, D; E := sum(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Sum<float>");
        slate.Perform("A = (float)0.0; B = (float)0.0; C = (float)0.0; D = (float)0.0;").CheckValue( 0.0f, "E");
        slate.Perform("A = (float)1.0; B = (float)0.0; C = (float)0.0; D = (float)0.0;").CheckValue( 1.0f, "E");
        slate.Perform("A = (float)0.0; B = (float)1.0; C = (float)0.0; D = (float)0.0;").CheckValue( 1.0f, "E");
        slate.Perform("A = (float)0.0; B = (float)0.0; C = (float)1.0; D = (float)0.0;").CheckValue( 1.0f, "E");
        slate.Perform("A = (float)0.0; B = (float)0.0; C = (float)0.0; D = (float)1.0;").CheckValue( 1.0f, "E");
        slate.Perform("A = (float)1.1; B = (float)2.3; C = (float)4.3; D = (float)6.0;").CheckValue(13.7f, "E");
        slate.Perform("F := sum(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<float>[1.1](A)");
        // The zero input scenario is checked in TestFunctions_sum_Sum_Double.
    }

    [TestMethod]
    [TestTag("sum:Sum<Int>")]
    public void TestFunctions_sum_Sum_Int() {
        Slate slate = new Slate().Perform("in int A, B, C, D; E := sum(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Sum<int>");
        slate.Perform("A = 0; B = 0; C = 0; D = 0;").CheckValue( 0, "E");
        slate.Perform("A = 1; B = 0; C = 0; D = 0;").CheckValue( 1, "E");
        slate.Perform("A = 0; B = 1; C = 0; D = 0;").CheckValue( 1, "E");
        slate.Perform("A = 0; B = 0; C = 1; D = 0;").CheckValue( 1, "E");
        slate.Perform("A = 0; B = 0; C = 0; D = 1;").CheckValue( 1, "E");
        slate.Perform("A = 1; B = 2; C = 4; D = 6;").CheckValue(13, "E");
        slate.Perform("F := sum(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<int>[1](A)");
        // The zero input scenario is checked in TestFunctions_sum_Sum_Double.
    }

    [TestMethod]
    [TestTag("sum:Sum<Uint>")]
    public void TestFunctions_sum_Sum_Uint() {
        Slate slate = new Slate().Perform("in uint A, B, C, D; E := sum(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Sum<uint>");
        slate.Perform("A = (uint)0; B = (uint)0; C = (uint)0; D = (uint)0;").CheckValue((uint) 0, "E");
        slate.Perform("A = (uint)1; B = (uint)0; C = (uint)0; D = (uint)0;").CheckValue((uint) 1, "E");
        slate.Perform("A = (uint)0; B = (uint)1; C = (uint)0; D = (uint)0;").CheckValue((uint) 1, "E");
        slate.Perform("A = (uint)0; B = (uint)0; C = (uint)1; D = (uint)0;").CheckValue((uint) 1, "E");
        slate.Perform("A = (uint)0; B = (uint)0; C = (uint)0; D = (uint)1;").CheckValue((uint) 1, "E");
        slate.Perform("A = (uint)1; B = (uint)2; C = (uint)4; D = (uint)6;").CheckValue((uint)13, "E");
        slate.Perform("F := sum(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<uint>[1](A)");
        // The zero input scenario is checked in TestFunctions_sum_Sum_Double.
    }

    [TestMethod]
    [TestTag("sum:Sum<String>")]
    public void TestFunctions_sum_Sum_String() {
        Slate slate = new Slate().Perform("in string A, B, C, D; E := sum(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Sum<string>");
        slate.Perform("A = 'd'; B = 'o'; C = 'g'; D = 's';").CheckValue("dogs", "E");
        slate.Perform("C = 't';").CheckValue("dots", "E");
        slate.Perform("A = 'c';").CheckValue("cots", "E");
        slate.Perform("B = 'a';").CheckValue("cats", "E");
        slate.Perform("F := sum(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<string>[c](A)");
        // The zero input scenario is checked in TestFunctions_sum_Sum_Double.
    }

    [TestMethod]
    [TestTag("tan:UnaryFuncs<Double, Double>")]
    public void TestFunctions_tan_UnaryFuncs_Double_Double() {
        Slate slate = new Slate().Perform("in double A; B := tan(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Tan<double>");
        slate.Perform("A =  0.0;   ").CheckValue( 0.0, "B");
        slate.Perform("A =  pi*0.5;").CheckValue(S.Math.Tan( S.Math.PI*0.5), "B");
        slate.Perform("A = -pi*0.5;").CheckValue(S.Math.Tan(-S.Math.PI*0.5), "B");
        slate.Perform("A =  pi;    ").CheckValue(S.Math.Tan( S.Math.PI), "B");
        slate.Perform("A = -pi;    ").CheckValue(S.Math.Tan(-S.Math.PI), "B");
        slate.Perform("A =  pi*1.5;").CheckValue(S.Math.Tan( S.Math.PI*1.5), "B");
    }

    [TestMethod]
    [TestTag("tan:UnaryFuncs<Float, Float>")]
    public void TestFunctions_tan_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := tan(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Tan<float>");
        slate.Perform("A = (float) 0.0;     ").CheckValue(0.0f, "B");
        slate.Perform("A = (float)( pi*0.5);").CheckValue(float.Tan( float.Pi*0.5f), "B");
        slate.Perform("A = (float)(-pi*0.5);").CheckValue(float.Tan(-float.Pi*0.5f), "B");
        slate.Perform("A = (float) pi;      ").CheckValue(float.Tan( float.Pi),      "B");
        slate.Perform("A = (float)-pi;      ").CheckValue(float.Tan(-float.Pi),      "B");
        slate.Perform("A = (float)( pi*1.5);").CheckValue(float.Tan( float.Pi*1.5f), "B");
    }

    [TestMethod]
    [TestTag("tanh:UnaryFuncs<Double, Double>")]
    public void TestFunctions_tanh_UnaryFuncs_Double_Double() {
        Slate slate = new Slate().Perform("in double A; B := tanh(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Tanh<double>");
        slate.Perform("A =  0.0;").CheckValue(0.0, "B");
        slate.Perform("A =  1.0;").CheckValue(S.Math.Tanh( 1.0), "B");
        slate.Perform("A = -1.0;").CheckValue(S.Math.Tanh(-1.0), "B");
        slate.Perform("A =  3.0;").CheckValue(S.Math.Tanh( 3.0), "B");
        slate.Perform("A = 12.3;").CheckValue(S.Math.Tanh(12.3), "B");
    }

    [TestMethod]
    [TestTag("tanh:UnaryFuncs<Float, Float>")]
    public void TestFunctions_tanh_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := tanh(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Tanh<float>");
        slate.Perform("A = (float) 0.0;").CheckValue(0.0f, "B");
        slate.Perform("A = (float) 1.0;").CheckValue(float.Tanh( 1.0f), "B");
        slate.Perform("A = (float)-1.0;").CheckValue(float.Tanh(-1.0f), "B");
        slate.Perform("A = (float) 3.0;").CheckValue(float.Tanh( 3.0f), "B");
        slate.Perform("A = (float)12.3;").CheckValue(float.Tanh(12.3f), "B");
    }

    [TestMethod]
    [TestTag("trim:BinaryFunc<String, String, String>")]
    public void TestFunctions_trim_BinaryFunc_String_String_String() {
        Slate slate = new Slate().Perform("in string A, B; C := trim(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Trim<string>");
        slate.Perform("A = '';          B = '';  ").CheckValue("",      "C");
        slate.Perform("A = '';          B = ' '; ").CheckValue("",      "C");
        slate.Perform("A = 'Hello';     B = '';  ").CheckValue("Hello", "C");
        slate.Perform("A = 'Hello';     B = ' '; ").CheckValue("Hello", "C");
        slate.Perform("A = '  Hello';   B = ' '; ").CheckValue("Hello", "C");
        slate.Perform("A = 'Hello  ';   B = ' '; ").CheckValue("Hello", "C");
        slate.Perform("A = '  Hello  '; B = ' '; ").CheckValue("Hello", "C");
        slate.Perform("A = 'Hello';     B = 'lo';").CheckValue("He",    "C");
        slate.Perform("A = 'Hello';     B = 'oH';").CheckValue("ell",   "C");
    }

    [TestMethod]
    [TestTag("trim:UnaryFuncs<String, String>")]
    public void TestFunctions_trim_UnaryFuncs_String_String() {
        Slate slate = new Slate().Perform("in string A; B := trim(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Trim<string>");
        slate.Perform("A = '';         ").CheckValue("",      "B");
        slate.Perform("A = 'Hello';    ").CheckValue("Hello", "B");
        slate.Perform("A = '  Hello';  ").CheckValue("Hello", "B");
        slate.Perform("A = 'Hello  ';  ").CheckValue("Hello", "B");
        slate.Perform("A = '  Hello  ';").CheckValue("Hello", "B");
    }

    [TestMethod]
    [TestTag("trimEnd:BinaryFunc<String, String, String>")]
    public void TestFunctions_trimEnd_BinaryFunc_String_String_String() {
        Slate slate = new Slate().Perform("in string A, B; C := trimEnd(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: TrimEnd<string>");
        slate.Perform("A = '';          B = '';  ").CheckValue("",        "C");
        slate.Perform("A = '';          B = ' '; ").CheckValue("",        "C");
        slate.Perform("A = 'Hello';     B = '';  ").CheckValue("Hello",   "C");
        slate.Perform("A = 'Hello';     B = ' '; ").CheckValue("Hello",   "C");
        slate.Perform("A = '  Hello';   B = ' '; ").CheckValue("  Hello", "C");
        slate.Perform("A = 'Hello  ';   B = ' '; ").CheckValue("Hello",   "C");
        slate.Perform("A = '  Hello  '; B = ' '; ").CheckValue("  Hello", "C");
        slate.Perform("A = 'Hello';     B = 'lo';").CheckValue("He",      "C");
        slate.Perform("A = 'Hello';     B = 'oH';").CheckValue("Hell",    "C");
    }

    [TestMethod]
    [TestTag("trimEnd:UnaryFuncs<String, String>")]
    public void TestFunctions_trimEnd_UnaryFuncs_String_String() {
        Slate slate = new Slate().Perform("in string A; B := trimEnd(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: TrimEnd<string>");
        slate.Perform("A = '';         ").CheckValue("",        "B");
        slate.Perform("A = 'Hello';    ").CheckValue("Hello",   "B");
        slate.Perform("A = '  Hello';  ").CheckValue("  Hello", "B");
        slate.Perform("A = 'Hello  ';  ").CheckValue("Hello",   "B");
        slate.Perform("A = '  Hello  ';").CheckValue("  Hello", "B");
    }

    [TestMethod]
    [TestTag("trimStart:BinaryFunc<String, String, String>")]
    public void TestFunctions_trimStart_BinaryFunc_String_String_String() {
        Slate slate = new Slate().Perform("in string A, B; C := trimStart(A, B);");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: TrimStart<string>");
        slate.Perform("A = '';          B = '';  ").CheckValue("",        "C");
        slate.Perform("A = '';          B = ' '; ").CheckValue("",        "C");
        slate.Perform("A = 'Hello';     B = '';  ").CheckValue("Hello",   "C");
        slate.Perform("A = 'Hello';     B = ' '; ").CheckValue("Hello",   "C");
        slate.Perform("A = '  Hello';   B = ' '; ").CheckValue("Hello",   "C");
        slate.Perform("A = 'Hello  ';   B = ' '; ").CheckValue("Hello  ", "C");
        slate.Perform("A = '  Hello  '; B = ' '; ").CheckValue("Hello  ", "C");
        slate.Perform("A = 'Hello';     B = 'lo';").CheckValue("Hello",   "C");
        slate.Perform("A = 'Hello';     B = 'oH';").CheckValue("ello",    "C");
    }

    [TestMethod]
    [TestTag("trimStart:UnaryFuncs<String, String>")]
    public void TestFunctions_trimStart_UnaryFuncs_String_String() {
        Slate slate = new Slate().Perform("in string A; B := trimStart(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: TrimStart<string>");
        slate.Perform("A = '';         ").CheckValue("",        "B");
        slate.Perform("A = 'Hello';    ").CheckValue("Hello",   "B");
        slate.Perform("A = '  Hello';  ").CheckValue("Hello",   "B");
        slate.Perform("A = 'Hello  ';  ").CheckValue("Hello  ", "B");
        slate.Perform("A = '  Hello  ';").CheckValue("Hello  ", "B");
    }

    [TestMethod]
    [TestTag("trunc:UnaryFuncs<Double, Double>")]
    public void TestFunctions_trunc_UnaryFuncs_Double_Double() {
        Slate slate = new Slate().Perform("in double A; B := trunc(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Truncate<double>");
        slate.Perform("A =  3.14;  ").CheckValue( 3.0, "B");
        slate.Perform("A =  3.4999;").CheckValue( 3.0, "B");
        slate.Perform("A =  3.5;   ").CheckValue( 3.0, "B");
        slate.Perform("A =  3.6;   ").CheckValue( 3.0, "B");
        slate.Perform("A = 42.0;   ").CheckValue(42.0, "B");
    }

    [TestMethod]
    [TestTag("trunc:UnaryFuncs<Float, Float>")]
    public void TestFunctions_trunc_UnaryFuncs_Float_Float() {
        Slate slate = new Slate().Perform("in float A; B := trunc(A);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Truncate<float>");
        slate.Perform("A = (float) 3.14;  ").CheckValue( 3.0f, "B");
        slate.Perform("A = (float) 3.4999;").CheckValue( 3.0f, "B");
        slate.Perform("A = (float) 3.5;   ").CheckValue( 3.0f, "B");
        slate.Perform("A = (float) 3.6;   ").CheckValue( 3.0f, "B");
        slate.Perform("A = (float)42.0;   ").CheckValue(42.0f, "B");
    }

    [TestMethod]
    [TestTag("xor:BitwiseXor<Int>")]
    public void TestFunctions_xor_BitwiseXor_Int() {
        Slate slate = new Slate().Perform("in int A, B, C, D; E := xor(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: BitwiseXor<int>");
        slate.Perform("A = 0000b; B = 0000b; C = 0000b; D = 0000b;").CheckValue(0b0000, "E");
        slate.Perform("A = 1000b; B = 0100b; C = 0010b; D = 0001b;").CheckValue(0b1111, "E");
        slate.Perform("A = 1100b; B = 0110b; C = 0011b; D = 1111b;").CheckValue(0b0110, "E");
        slate.Perform("A = 1010b; B = 1001b; C = 1010b; D = 1011b;").CheckValue(0b0010, "E");
        slate.Perform("A = 1111b; B = 0000b; C = 0000b; D = 0000b;").CheckValue(0b1111, "E");
        slate.Perform("A = 0000b; B = 1111b; C = 0000b; D = 0000b;").CheckValue(0b1111, "E");
        slate.Perform("A = 0000b; B = 0000b; C = 1111b; D = 0000b;").CheckValue(0b1111, "E");
        slate.Perform("A = 1111b; B = 1111b; C = 0000b; D = 0000b;").CheckValue(0b0000, "E");
        slate.Perform("A = 1111b; B = 0000b; C = 1111b; D = 0000b;").CheckValue(0b0000, "E");
        slate.Perform("A = 1111b; B = 0000b; C = 0000b; D = 1111b;").CheckValue(0b0000, "E");
        slate.Perform("A = 0000b; B = 1111b; C = 1111b; D = 0000b;").CheckValue(0b0000, "E");
        slate.Perform("A = 0000b; B = 1111b; C = 0000b; D = 1111b;").CheckValue(0b0000, "E");
        slate.Perform("A = 0000b; B = 0000b; C = 1111b; D = 1111b;").CheckValue(0b0000, "E");
        slate.Perform("A = 0000b; B = 0000b; C = 0000b; D = 1111b;").CheckValue(0b1111, "E");
        slate.Perform("A = 0000b; B = 1111b; C = 1111b; D = 1111b;").CheckValue(0b1111, "E");
        slate.Perform("A = 1111b; B = 0000b; C = 1111b; D = 1111b;").CheckValue(0b1111, "E");
        slate.Perform("A = 1111b; B = 1111b; C = 0000b; D = 1111b;").CheckValue(0b1111, "E");
        slate.Perform("A = 1111b; B = 1111b; C = 1111b; D = 0000b;").CheckValue(0b1111, "E");
        slate.Perform("A = 1111b; B = 1111b; C = 1111b; D = 1111b;").CheckValue(0b0000, "E");
        slate.Perform("F := xor(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<int>[15](A)");
        TestTools.CheckException(() => slate.Perform("G := xor();"),
            "Error occurred while parsing input code.",
            "[Error: Could not perform the function without any inputs.",
            "   [Function: FuncGroup]",
            "   [Location: Unnamed:1, 9, 9]]");
    }

    [TestMethod]
    [TestTag("xor:BitwiseXor<Uint>")]
    public void TestFunctions_xor_BitwiseXor_Uint() {
        Slate slate = new Slate().Perform("in uint A, B, C, D; E := xor(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: BitwiseXor<uint>");
        slate.Perform("A = (uint)0000b; B = (uint)0000b; C = (uint)0000b; D = (uint)0000b;").CheckValue((uint)0b0000, "E");
        slate.Perform("A = (uint)1000b; B = (uint)0100b; C = (uint)0010b; D = (uint)0001b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)1100b; B = (uint)0110b; C = (uint)0011b; D = (uint)1111b;").CheckValue((uint)0b0110, "E");
        slate.Perform("A = (uint)1010b; B = (uint)1001b; C = (uint)1010b; D = (uint)1011b;").CheckValue((uint)0b0010, "E");
        slate.Perform("A = (uint)1111b; B = (uint)0000b; C = (uint)0000b; D = (uint)0000b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)0000b; B = (uint)1111b; C = (uint)0000b; D = (uint)0000b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)0000b; B = (uint)0000b; C = (uint)1111b; D = (uint)0000b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)1111b; B = (uint)1111b; C = (uint)0000b; D = (uint)0000b;").CheckValue((uint)0b0000, "E");
        slate.Perform("A = (uint)1111b; B = (uint)0000b; C = (uint)1111b; D = (uint)0000b;").CheckValue((uint)0b0000, "E");
        slate.Perform("A = (uint)1111b; B = (uint)0000b; C = (uint)0000b; D = (uint)1111b;").CheckValue((uint)0b0000, "E");
        slate.Perform("A = (uint)0000b; B = (uint)1111b; C = (uint)1111b; D = (uint)0000b;").CheckValue((uint)0b0000, "E");
        slate.Perform("A = (uint)0000b; B = (uint)1111b; C = (uint)0000b; D = (uint)1111b;").CheckValue((uint)0b0000, "E");
        slate.Perform("A = (uint)0000b; B = (uint)0000b; C = (uint)1111b; D = (uint)1111b;").CheckValue((uint)0b0000, "E");
        slate.Perform("A = (uint)0000b; B = (uint)0000b; C = (uint)0000b; D = (uint)1111b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)0000b; B = (uint)1111b; C = (uint)1111b; D = (uint)1111b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)1111b; B = (uint)0000b; C = (uint)1111b; D = (uint)1111b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)1111b; B = (uint)1111b; C = (uint)0000b; D = (uint)1111b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)1111b; B = (uint)1111b; C = (uint)1111b; D = (uint)0000b;").CheckValue((uint)0b1111, "E");
        slate.Perform("A = (uint)1111b; B = (uint)1111b; C = (uint)1111b; D = (uint)1111b;").CheckValue((uint)0b0000, "E");
        slate.Perform("F := xor(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<uint>[15](A)");
        // The zero input scenario is checked in TestFunctions_xor_BitwiseXor_Int.
    }

    [TestMethod]
    [TestTag("xor:Xor")]
    public void TestFunctions_xor_Xor() {
        Slate slate = new Slate().Perform("in bool A, B, C, D; E := xor(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Xor<bool>");
        slate.Perform("A = false; B = false; C = false; D = false;").CheckValue(false, "E");
        slate.Perform("A = true;  B = false; C = false; D = false;").CheckValue(true,  "E");
        slate.Perform("A = false; B = true;  C = false; D = false;").CheckValue(true,  "E");
        slate.Perform("A = false; B = false; C = true;  D = false;").CheckValue(true,  "E");
        slate.Perform("A = false; B = false; C = false; D = true; ").CheckValue(true,  "E");
        slate.Perform("A = true;  B = true;  C = false; D = false;").CheckValue(false, "E");
        slate.Perform("A = true;  B = false; C = true;  D = false;").CheckValue(false, "E");
        slate.Perform("A = true;  B = false; C = false; D = true; ").CheckValue(false, "E");
        slate.Perform("A = false; B = true;  C = true;  D = false;").CheckValue(false, "E");
        slate.Perform("A = false; B = true;  C = false; D = true; ").CheckValue(false, "E");
        slate.Perform("A = false; B = false; C = true;  D = true; ").CheckValue(false, "E");
        slate.Perform("A = true;  B = true;  C = true;  D = false;").CheckValue(true,  "E");
        slate.Perform("A = true;  B = true;  C = false; D = true; ").CheckValue(true,  "E");
        slate.Perform("A = false; B = true;  C = true;  D = true; ").CheckValue(true,  "E");
        slate.Perform("A = true;  B = true;  C = true;  D = true; ").CheckValue(false, "E");
        slate.Perform("F := xor(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<bool>[true](A)");
        // The zero input scenario is checked in TestFunctions_xor_BitwiseXor_Int.
    }

    [TestMethod]
    [TestTag("xor:XorTrigger")]
    public void TestFunctions_xor_XorTrigger() {
        Slate slate = new Slate().Perform("in trigger A, B, C, D; E := xor(A, B, C, D);");
        slate.CheckNodeString(Stringifier.Basic(), "E", "E: Xor<trigger>");
        slate.PerformWithoutReset("-> A;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> B;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> C;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> D;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> B;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> C;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> B -> C;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> B -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> C -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> B -> C;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> B -> D;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> C -> D;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> B -> C -> D;").CheckProvoked(true, "E").FinishEvaluation();
        slate.PerformWithoutReset("-> A -> B -> C -> D;").CheckProvoked(false, "E").FinishEvaluation();
        slate.Perform("F := xor(A);"); // Single pass through, F is set to A with a shell to prevent F from being set like A.
        slate.CheckNodeString(Stringifier.Shallow(), "F", "F: Shell<trigger>[](A)");
        // The zero input scenario is checked in TestFunctions_xor_BitwiseXor_Int.
    }

    [TestMethod]
    [TestTag("zener:Zener<Double>")]
    public void TestFunctions_zener_Zener_Double() {
        Slate slate = new Slate().Perform("in double A, Min, Max; B := zener(A, Min, Max);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Zener<bool>");
        slate.Perform("A = 0.0; Min = 0.0; Max = 0.0;").CheckValue(false, "B");
        slate.Perform("A = 0.5; Min = 1.0; Max = 2.0;").CheckValue(false, "B");
        slate.Perform("A = 1.0; Min = 1.0; Max = 2.0;").CheckValue(false, "B");
        slate.Perform("A = 1.5; Min = 1.0; Max = 2.0;").CheckValue(false, "B");
        slate.Perform("A = 2.0; Min = 1.0; Max = 2.0;").CheckValue(true,  "B");
        slate.Perform("A = 2.5; Min = 1.0; Max = 2.0;").CheckValue(true,  "B");
        slate.Perform("A = 2.0; Min = 1.0; Max = 2.0;").CheckValue(true,  "B");
        slate.Perform("A = 1.5; Min = 1.0; Max = 2.0;").CheckValue(true,  "B");
        slate.Perform("A = 1.0; Min = 1.0; Max = 2.0;").CheckValue(false, "B");
        slate.Perform("A = 0.5; Min = 1.0; Max = 2.0;").CheckValue(false, "B");
        slate.Perform("A = 2.0; Min = 3.0; Max = 1.0;").CheckValue(false, "B"); // Min/max reversed
    }

    [TestMethod]
    [TestTag("zener:Zener<Float>")]
    public void TestFunctions_zener_Zener_Float() {
        Slate slate = new Slate().Perform("in float A, Min, Max; B := zener(A, Min, Max);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Zener<bool>");
        slate.Perform("A = (float)0.0; Min = (float)0.0; Max = (float)0.0;").CheckValue(false, "B");
        slate.Perform("A = (float)0.5; Min = (float)1.0; Max = (float)2.0;").CheckValue(false, "B");
        slate.Perform("A = (float)1.0; Min = (float)1.0; Max = (float)2.0;").CheckValue(false, "B");
        slate.Perform("A = (float)1.5; Min = (float)1.0; Max = (float)2.0;").CheckValue(false, "B");
        slate.Perform("A = (float)2.0; Min = (float)1.0; Max = (float)2.0;").CheckValue(true,  "B");
        slate.Perform("A = (float)2.5; Min = (float)1.0; Max = (float)2.0;").CheckValue(true,  "B");
        slate.Perform("A = (float)2.0; Min = (float)1.0; Max = (float)2.0;").CheckValue(true,  "B");
        slate.Perform("A = (float)1.5; Min = (float)1.0; Max = (float)2.0;").CheckValue(true,  "B");
        slate.Perform("A = (float)1.0; Min = (float)1.0; Max = (float)2.0;").CheckValue(false, "B");
        slate.Perform("A = (float)0.5; Min = (float)1.0; Max = (float)2.0;").CheckValue(false, "B");
        slate.Perform("A = (float)2.0; Min = (float)3.0; Max = (float)1.0;").CheckValue(false, "B"); // Min/max reversed
    }

    [TestMethod]
    [TestTag("zener:Zener<Int>")]
    public void TestFunctions_zener_Zener_Int() {
        Slate slate = new Slate().Perform("in int A, Min, Max; B := zener(A, Min, Max);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Zener<bool>");
        slate.Perform("A = 0; Min = 0; Max = 0;").CheckValue(false, "B");
        slate.Perform("A = 0; Min = 1; Max = 3;").CheckValue(false, "B");
        slate.Perform("A = 1; Min = 1; Max = 3;").CheckValue(false, "B");
        slate.Perform("A = 2; Min = 1; Max = 3;").CheckValue(false, "B");
        slate.Perform("A = 3; Min = 1; Max = 3;").CheckValue(true,  "B");
        slate.Perform("A = 4; Min = 1; Max = 3;").CheckValue(true,  "B");
        slate.Perform("A = 3; Min = 1; Max = 3;").CheckValue(true,  "B");
        slate.Perform("A = 2; Min = 1; Max = 3;").CheckValue(true,  "B");
        slate.Perform("A = 1; Min = 1; Max = 3;").CheckValue(false, "B");
        slate.Perform("A = 0; Min = 1; Max = 3;").CheckValue(false, "B");
        slate.Perform("A = 2; Min = 3; Max = 1;").CheckValue(false, "B"); // Min/max reversed
    }

    [TestMethod]
    [TestTag("zener:Zener<Uint>")]
    public void TestFunctions_zener_Zener_Uint() {
        Slate slate = new Slate().Perform("in uint A, Min, Max; B := zener(A, Min, Max);");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Zener<bool>");
        slate.Perform("A = (uint)0; Min = (uint)0; Max = (uint)0;").CheckValue(false, "B");
        slate.Perform("A = (uint)0; Min = (uint)1; Max = (uint)3;").CheckValue(false, "B");
        slate.Perform("A = (uint)1; Min = (uint)1; Max = (uint)3;").CheckValue(false, "B");
        slate.Perform("A = (uint)2; Min = (uint)1; Max = (uint)3;").CheckValue(false, "B");
        slate.Perform("A = (uint)3; Min = (uint)1; Max = (uint)3;").CheckValue(true,  "B");
        slate.Perform("A = (uint)4; Min = (uint)1; Max = (uint)3;").CheckValue(true,  "B");
        slate.Perform("A = (uint)3; Min = (uint)1; Max = (uint)3;").CheckValue(true,  "B");
        slate.Perform("A = (uint)2; Min = (uint)1; Max = (uint)3;").CheckValue(true,  "B");
        slate.Perform("A = (uint)1; Min = (uint)1; Max = (uint)3;").CheckValue(false, "B");
        slate.Perform("A = (uint)0; Min = (uint)1; Max = (uint)3;").CheckValue(false, "B");
        slate.Perform("A = (uint)2; Min = (uint)3; Max = (uint)1;").CheckValue(false, "B"); // Min/max reversed
    }
}
