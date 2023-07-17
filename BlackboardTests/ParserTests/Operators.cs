using Blackboard.Core;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Outer;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests;

[TestClass]
public class Operators {

    [TestMethod]
    public void CheckAllOperatorsAreTested() =>
        TestTools.SetEntriesMatch(
            TestTools.FuncDefTags(new Slate().Global[Blackboard.Core.Innate.Operators.Namespace] as Namespace),
            TestTools.TestTags(this.GetType()),
            "Tests do not match the existing operators");

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
    [TestTag("and:BitwiseAnd<Uint>")]
    public void TestOperators_and_BitwiseAnd_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A & B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: BitwiseAnd<uint>");
        slate.Perform("A = (uint)0000b; B = (uint)0000b;").CheckValue((uint)0b0000, "C");
        slate.Perform("A = (uint)1100b; B = (uint)0011b;").CheckValue((uint)0b0000, "C");
        slate.Perform("A = (uint)1110b; B = (uint)0111b;").CheckValue((uint)0b0110, "C");
        slate.Perform("A = (uint)1111b; B = (uint)0101b;").CheckValue((uint)0b0101, "C");
        slate.Perform("A = (uint)1010b; B = (uint)1111b;").CheckValue((uint)0b1010, "C");
        slate.Perform("A = (uint)1111b; B = (uint)1111b;").CheckValue((uint)0b1111, "C");
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
    [TestTag("castBool:Explicit<Object, Bool>")]
    public void TestOperators_castBool_Explicit_Object_Bool() {
        Slate slate = new Slate().Perform("in object A = true; B := (bool)A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Explicit<bool>");
        slate.Perform("A = false;").CheckValue(false, "B");
        slate.Perform("A = true; ").CheckValue(true,  "B");
        TestTools.CheckException(() => slate.Perform("A = 'Hello';"),
            "Unable to cast object value (System.String) to bool type.");
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
    [TestTag("castInt:Explicit<Uint, Int>")]
    public void TestOperators_castInt_Explicit_Uint_Int() {
        Slate slate = new Slate().Perform("in uint A; B := (int)A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Explicit<int>");
        slate.Perform("A = (uint)0;").CheckValue(0, "B");
        slate.Perform("A = (uint)1;").CheckValue(1, "B");
        slate.Perform("A = (uint)2;").CheckValue(2, "B");
        slate.Perform("A = (uint)3;").CheckValue(3, "B");
        //slate.Perform("A = (uint) -1;").CheckValue(-1, "B"); // TODO: Check roll-over and fix parser issue
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
    [TestTag("castInt:Explicit<Object, Int>")]
    public void TestOperators_castInt_Explicit_Object_Int() {
        Slate slate = new Slate().Perform("in object A = 0; B := (int)A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Explicit<int>");
        slate.Perform("A =  0;").CheckValue( 0, "B");
        slate.Perform("A =  1;").CheckValue( 1, "B");
        slate.Perform("A =  2;").CheckValue( 2, "B");
        slate.Perform("A =  3;").CheckValue( 3, "B");
        slate.Perform("A = -4;").CheckValue(-4, "B");
        TestTools.CheckException(() => slate.Perform("A = 'Hello';"),
            "Unable to cast object value (System.String) to int type.");
    }

    [TestMethod]
    [TestTag("castUint:Explicit<Int, Uint>")]
    public void TestOperators_castUint_Explicit_Int_Uint() {
        Slate slate = new Slate().Perform("in int A; B := (uint)A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Explicit<uint>");
        slate.Perform("A = 0;").CheckValue((uint)0, "B");
        slate.Perform("A = 1;").CheckValue((uint)1, "B");
        slate.Perform("A = 2;").CheckValue((uint)2, "B");
        slate.Perform("A = 3;").CheckValue((uint)3, "B");
    }

    [TestMethod]
    [TestTag("castUint:Explicit<Double, Uint>")]
    public void TestOperators_castUint_Explicit_Double_Uint() {
        Slate slate = new Slate().Perform("in double A; B := (uint)A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Explicit<uint>");
        slate.Perform("A =  0.1;").CheckValue((uint)0, "B");
        slate.Perform("A =  0.9;").CheckValue((uint)0, "B");
        slate.Perform("A =  2.1;").CheckValue((uint)2, "B");
        slate.Perform("A =  2.9;").CheckValue((uint)2, "B");
        slate.Perform("A = -4.2;").CheckValue(4294967292, "B");
    }
    
    [TestMethod]
    [TestTag("castUint:Explicit<Object, Uint>")]
    public void TestOperators_castUint_Explicit_Object_Uint() {
        Slate slate = new Slate().Perform("in object A = (uint)0; B := (uint)A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Explicit<uint>");
        slate.Perform("A = (uint)0;").CheckValue((uint)0, "B");
        slate.Perform("A = (uint)1;").CheckValue((uint)1, "B");
        slate.Perform("A = (uint)2;").CheckValue((uint)2, "B");
        TestTools.CheckException(() => slate.Perform("A = 'Hello';"),
            "Unable to cast object value (System.String) to uint type.");
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
    [TestTag("castDouble:Implicit<Uint, Double>")]
    public void TestOperators_castDouble_Implicit_Uint_Double() {
        Slate slate = new Slate().Perform("in uint A; double B := A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<double>");
        slate.Perform("A = (uint)0;").CheckValue( 0.0, "B");
        slate.Perform("A = (uint)1;").CheckValue( 1.0, "B");
        slate.Perform("A = (uint)2;").CheckValue( 2.0, "B");
    }

    [TestMethod]
    [TestTag("castDouble:Explicit<Object, Double>")]
    public void TestOperators_castDouble_Explicit_Object_Double() {
        Slate slate = new Slate().Perform("in object A = 0.0; B := (double)A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Explicit<double>");
        slate.Perform("A =  0.1;").CheckValue( 0.1, "B");
        slate.Perform("A =  0.9;").CheckValue( 0.9, "B");
        slate.Perform("A =  2.1;").CheckValue( 2.1, "B");
        slate.Perform("A =  2.9;").CheckValue( 2.9, "B");
        slate.Perform("A = -4.2;").CheckValue(-4.2, "B");
        slate.Perform("A =  inf;").CheckValue(double.PositiveInfinity, "B");
        slate.Perform("A = -inf;").CheckValue(double.NegativeInfinity, "B");
        slate.Perform("A =  nan;").CheckValue(double.NaN, "B");
        TestTools.CheckException(() => slate.Perform("A = 'Hello';"),
            "Unable to cast object value (System.String) to double type.");
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
    [TestTag("castString:Implicit<Uint, String>")]
    public void TestOperators_castString_Implicit_Uint_String() {
        Slate slate = new Slate().Perform("in uint A; string B := A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<string>");
        slate.Perform("A = (uint)0;").CheckValue("0",  "B");
        slate.Perform("A = (uint)1;").CheckValue("1",  "B");
        slate.Perform("A = (uint)2;").CheckValue("2" , "B");
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
    [TestTag("castString:Implicit<Object, String>")]
    public void TestOperators_castString_Implicit_Object_String() {
        Slate slate = new Slate().Perform("in object A = ''; string B := A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<string>");
        slate.Perform("A = 'Hello';").CheckValue("Hello", "B");
        slate.Perform("A = 'World';").CheckValue("World", "B");
        slate.Perform("A = 3.14;   ").CheckValue("3.14",  "B");
        slate.Perform("A = true;   ").CheckValue("True",  "B");
    }

    [TestMethod]
    [TestTag("castObject:Implicit<Bool, Object>")]
    public void TestOperators_castObject_Implicit_Bool_Object() {
        Slate slate = new Slate().Perform("in bool A; object B := A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<object>");
        slate.Perform("A = true; ").CheckObject(true,  "B");
        slate.Perform("A = false;").CheckObject(false, "B");
    }

    [TestMethod]
    [TestTag("castObject:Implicit<Int, Object>")]
    public void TestOperators_castObject_Implicit_Int_Object() {
        Slate slate = new Slate().Perform("in int A; object B := A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<object>");
        slate.Perform("A =  0;").CheckObject( 0, "B");
        slate.Perform("A =  1;").CheckObject( 1, "B");
        slate.Perform("A =  2;").CheckObject( 2, "B");
        slate.Perform("A = -1;").CheckObject(-1, "B");
    }

    [TestMethod]
    [TestTag("castObject:Implicit<Uint, Object>")]
    public void TestOperators_castObject_Implicit_Uint_Object() {
        Slate slate = new Slate().Perform("in uint A; object B := A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<object>");
        slate.Perform("A = (uint)0;").CheckObject((uint)0, "B");
        slate.Perform("A = (uint)1;").CheckObject((uint)1, "B");
        slate.Perform("A = (uint)2;").CheckObject((uint)2, "B");
    }

    [TestMethod]
    [TestTag("castObject:Implicit<Double, Object>")]
    public void TestOperators_castObject_Implicit_Double_Object() {
        Slate slate = new Slate().Perform("in double A; object B := A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<object>");
        slate.Perform("A =  0.0;     ").CheckObject(  0.0,      "B");
        slate.Perform("A =  1.0;     ").CheckObject(  1.0,      "B");
        slate.Perform("A =  2.1;     ").CheckObject(  2.1,      "B");
        slate.Perform("A = -1.24;    ").CheckObject( -1.24,     "B");
        slate.Perform("A =  1e3;     ").CheckObject(  1e3,      "B");
        slate.Perform("A =  0.123e-9;").CheckObject(  0.123e-9, "B");
        slate.Perform("A =  inf;     ").CheckObject(double.PositiveInfinity, "B");
        slate.Perform("A = -inf;     ").CheckObject(double.NegativeInfinity, "B");
        slate.Perform("A =  nan;     ").CheckObject(double.NaN,              "B");
    }

    [TestMethod]
    [TestTag("castObject:Implicit<String, Object>")]
    public void TestOperators_castObject_Implicit_String_Object() {
        Slate slate = new Slate().Perform("in string A; object B := A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<object>");
        slate.Perform("A = 'Hello';").CheckObject("Hello", "B");
        slate.Perform("A = 'World';").CheckObject("World", "B");
        slate.Perform("A = '';     ").CheckObject("",      "B");
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
        TestTools.CheckException(() => slate.Perform("B = 0;"),
            "Attempted to divide by zero.");
    }

    [TestMethod]
    [TestTag("divide:Div<Uint>")]
    public void TestOperators_divide_Div_Uint() {
        Slate slate = new Slate().Perform("in uint A, B = (uint)1; C := A/B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Div<uint>");
        slate.Perform("A = (uint)4; B = (uint)2;").CheckValue((uint)2, "C");
        slate.Perform("A = (uint)3; B = (uint)2;").CheckValue((uint)1, "C");
        slate.Perform("A = (uint)9; B = (uint)3;").CheckValue((uint)3, "C");
        slate.Perform("A = (uint)1; B = (uint)3;").CheckValue((uint)0, "C");
        slate.Perform("A = (uint)8; B = (uint)3;").CheckValue((uint)2, "C");
        TestTools.CheckException(() => slate.Perform("B = (uint)0;"),
            "Attempted to divide by zero.");
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
    [TestTag("equal:Equal<Uint>")]
    public void TestOperators_equal_Equal_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A == B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Equal<bool>");
        slate.Perform("A = (uint) 0; B = (uint) 0;").CheckValue(true,  "C");
        slate.Perform("A = (uint) 2; B = (uint) 1;").CheckValue(false, "C");
        slate.Perform("A = (uint) 1; B = (uint) 2;").CheckValue(false, "C");
        slate.Perform("A = (uint)42; B = (uint)42;").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("equal:Equal<Double>")]
    public void TestOperators_equal_Equal_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A == B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Equal<bool>");
        slate.Perform("A =  0.0;     B =  0.0;    ").CheckValue(true,  "C");
        slate.Perform("A = -1.0;     B =  1.0;    ").CheckValue(false, "C");
        slate.Perform("A =  1.00004; B =  1.00005;").CheckValue(false, "C");
        slate.Perform("A =  0.001;   B =  1.0e-3; ").CheckValue(true,  "C");

        slate.Perform("A =  nan;     B =  nan;    ").CheckValue(true,  "C");
        slate.Perform("A =  nan;     B =  1.0;    ").CheckValue(false, "C");
        slate.Perform("A = -inf;     B =  inf;    ").CheckValue(false, "C");
        slate.Perform("A =  inf;     B =  inf;    ").CheckValue(true,  "C");
        slate.Perform("A =  inf;     B = -inf;    ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("equal:Equal<Object>")]
    public void TestOperators_equal_Equal_Object() {
        Slate slate = new Slate().Perform("in object A, B; C := A == B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Equal<bool>");
        slate.Perform("A = '';      B = '';     ").CheckValue(true,  "C");
        slate.Perform("A = 'World'; B = 'Hello';").CheckValue(false, "C");
        slate.Perform("A = false;   B = false;  ").CheckValue(true,  "C");
        slate.Perform("A = false;   B = true;   ").CheckValue(false, "C");
        slate.Perform("A = true;    B = false;  ").CheckValue(false, "C");
        slate.Perform("A = true;    B = true;   ").CheckValue(true,  "C");
        slate.Perform("A = 1;       B = 1;      ").CheckValue(true,  "C");
        slate.Perform("A = 1;       B = 2;      ").CheckValue(false, "C");
        slate.Perform("A = 2;       B = 1;      ").CheckValue(false, "C");
        slate.Perform("A = 1.0;     B = 1;      ").CheckValue(false, "C");
        slate.Perform("A = 1.0;     B = 1.0;    ").CheckValue(true,  "C");
        slate.Perform("A = 1.0;     B = 1.1;    ").CheckValue(false, "C");
        slate.Perform("A = 1.1;     B = 1.0;    ").CheckValue(false, "C");

        slate.Perform("A = 1.0;     B = nan;    ").CheckValue(false, "C");
        slate.Perform("A = nan;     B = nan;    ").CheckValue(true,  "C");
        slate.Perform("A = 1.1;     B = inf;    ").CheckValue(false, "C");
        slate.Perform("A = inf;     B = inf;    ").CheckValue(true,  "C");
        slate.Perform("A = inf;     B = 1.0;    ").CheckValue(false, "C");
        slate.Perform("A = 'inf';   B = inf;    ").CheckValue(false, "C");
        slate.Perform("A = false;   B = null;   ").CheckValue(false, "C");
        slate.Perform("A = 0;       B = null;   ").CheckValue(false, "C");
        slate.Perform("A = '';      B = null;   ").CheckValue(false, "C");
        slate.Perform("A = null;    B = null;   ").CheckValue(true,  "C");
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
        slate.Perform("A =  0; B =  0;").CheckValue(false, "C");
        slate.Perform("A = -1; B =  1;").CheckValue(false, "C");
        slate.Perform("A =  1; B = -1;").CheckValue(true,  "C");
        slate.Perform("A = 42; B = 42;").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("greater:GreaterThan<Uint>")]
    public void TestOperators_greater_GreaterThan_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A > B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThan<bool>");
        slate.Perform("A = (uint) 0; B = (uint) 0;").CheckValue(false, "C");
        slate.Perform("A = (uint) 1; B = (uint) 2;").CheckValue(false, "C");
        slate.Perform("A = (uint) 2; B = (uint) 1;").CheckValue(true,  "C");
        slate.Perform("A = (uint)42; B = (uint)42;").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("greater:GreaterThan<Double>")]
    public void TestOperators_greater_GreaterThan_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A > B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThan<bool>");
        slate.Perform("A =  0.0;     B =  0.0;    ").CheckValue(false, "C");
        slate.Perform("A = -1.0;     B =  1.0;    ").CheckValue(false, "C");
        slate.Perform("A =  1.0;     B = -1.0;    ").CheckValue(true,  "C");
        slate.Perform("A =  1.00004; B =  1.00005;").CheckValue(false, "C");
        slate.Perform("A =  1.00005; B =  1.00004;").CheckValue(true,  "C");
        slate.Perform("A = -inf;     B =  inf;    ").CheckValue(false, "C");
        slate.Perform("A =  inf;     B =  inf;    ").CheckValue(false, "C");
        slate.Perform("A =  inf;     B = -inf;    ").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("greater:GreaterThan<String>")]
    public void TestOperators_greater_GreaterThan_String() {
        Slate slate = new Slate().Perform("in string A, B; C := A > B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThan<bool>");
        slate.Perform("A = '';            B = '';           ").CheckValue(false, "C");
        slate.Perform("A = 'Hello World'; B = 'Hello World';").CheckValue(false, "C");
        slate.Perform("A = 'mop';         B = 'pop';        ").CheckValue(false, "C");
        slate.Perform("A = 'mop';         B = 'map';        ").CheckValue(true,  "C");
        slate.Perform("A = 'mop';         B = 'Mop';        ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("greaterEqual:GreaterThanOrEqual<Int>")]
    public void TestOperators_greaterEqual_GreaterThanOrEqual_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A >= B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThanOrEqual<bool>");
        slate.Perform("A =  0; B =  0;").CheckValue(true,  "C");
        slate.Perform("A = -1; B =  1;").CheckValue(false, "C");
        slate.Perform("A =  1; B = -1;").CheckValue(true,  "C");
        slate.Perform("A = 42; B = 42;").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("greaterEqual:GreaterThanOrEqual<Uint>")]
    public void TestOperators_greaterEqual_GreaterThanOrEqual_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A >= B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThanOrEqual<bool>");
        slate.Perform("A = (uint) 0; B = (uint) 0;").CheckValue(true,  "C");
        slate.Perform("A = (uint) 1; B = (uint) 2;").CheckValue(false, "C");
        slate.Perform("A = (uint) 2; B = (uint) 1;").CheckValue(true,  "C");
        slate.Perform("A = (uint)42; B = (uint)42;").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("greaterEqual:GreaterThanOrEqual<Double>")]
    public void TestOperators_greaterEqual_GreaterThanOrEqual_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A >= B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThanOrEqual<bool>");
        slate.Perform("A =  0.0;     B =  0.0;    ").CheckValue(true,  "C");
        slate.Perform("A = -1.0;     B =  1.0;    ").CheckValue(false, "C");
        slate.Perform("A =  1.0;     B = -1.0;    ").CheckValue(true,  "C");
        slate.Perform("A =  1.00004; B =  1.00005;").CheckValue(false, "C");
        slate.Perform("A =  1.00005; B =  1.00004;").CheckValue(true,  "C");
        slate.Perform("A = -inf;     B =  inf;    ").CheckValue(false, "C");
        slate.Perform("A =  inf;     B =  inf;    ").CheckValue(true,  "C");
        slate.Perform("A =  inf;     B = -inf;    ").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("greaterEqual:GreaterThanOrEqual<String>")]
    public void TestOperators_greaterEqual_GreaterThanOrEqual_String() {
        Slate slate = new Slate().Perform("in string A, B; C := A >= B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: GreaterThanOrEqual<bool>");
        slate.Perform("A = '';            B = '';           ").CheckValue(true,  "C");
        slate.Perform("A = 'Hello World'; B = 'Hello World';").CheckValue(true,  "C");
        slate.Perform("A = 'mop';         B = 'pop';        ").CheckValue(false, "C");
        slate.Perform("A = 'mop';         B = 'map';        ").CheckValue(true,  "C");
        slate.Perform("A = 'mop';         B = 'Mop';        ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("invert:BitwiseNot<Int>")]
    public void TestOperators_invert_BitwiseNot_Int() {
        Slate slate = new Slate().Perform("in int A; B := ~A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: BitwiseNot<int>");
        slate.Perform("A = 0x00000000;").CheckValue(unchecked((int)0xFFFF_FFFF), "B");
        slate.Perform("A = 0x00000001;").CheckValue(unchecked((int)0xFFFF_FFFE), "B");
        slate.Perform("A = 0x66666666;").CheckValue(unchecked((int)0x9999_9999), "B");
        slate.Perform("A = 0x0F0F0F0F;").CheckValue(unchecked((int)0xF0F0_F0F0), "B");
        slate.Perform("A = 0xF0F0F0F0;").CheckValue(unchecked((int)0x0F0F_0F0F), "B");
        slate.Perform("A = 0x80000000;").CheckValue(unchecked((int)0x7FFF_FFFF), "B");
        slate.Perform("A = 0x88888888;").CheckValue(unchecked((int)0x7777_7777), "B");
        slate.Perform("A = 0x77777777;").CheckValue(unchecked((int)0x8888_8888), "B");
    }

    [TestMethod]
    [TestTag("invert:BitwiseNot<Uint>")]
    public void TestOperators_invert_BitwiseNot_Uint() {
        Slate slate = new Slate().Perform("in uint A; B := ~A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: BitwiseNot<uint>");
        slate.Perform("A = (uint)0x00000000;").CheckValue(unchecked((uint)0xFFFF_FFFF), "B");
        slate.Perform("A = (uint)0x00000001;").CheckValue(unchecked((uint)0xFFFF_FFFE), "B");
        slate.Perform("A = (uint)0x66666666;").CheckValue(unchecked((uint)0x9999_9999), "B");
        slate.Perform("A = (uint)0x0F0F0F0F;").CheckValue(unchecked((uint)0xF0F0_F0F0), "B");
        slate.Perform("A = (uint)0xF0F0F0F0;").CheckValue(unchecked((uint)0x0F0F_0F0F), "B");
        slate.Perform("A = (uint)0x80000000;").CheckValue(unchecked((uint)0x7FFF_FFFF), "B");
        slate.Perform("A = (uint)0x88888888;").CheckValue(unchecked((uint)0x7777_7777), "B");
        slate.Perform("A = (uint)0x77777777;").CheckValue(unchecked((uint)0x8888_8888), "B");
    }

    [TestMethod]
    [TestTag("less:LessThan<Int>")]
    public void TestOperators_less_LessThan_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A < B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThan<bool>");
        slate.Perform("A =  0; B =  0;").CheckValue(false, "C");
        slate.Perform("A = -1; B =  1;").CheckValue(true,  "C");
        slate.Perform("A =  1; B = -1;").CheckValue(false, "C");
        slate.Perform("A = 42; B = 42;").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("less:LessThan<Uint>")]
    public void TestOperators_less_LessThan_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A < B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThan<bool>");
        slate.Perform("A = (uint) 0; B = (uint) 0;").CheckValue(false, "C");
        slate.Perform("A = (uint) 1; B = (uint) 2;").CheckValue(true,  "C");
        slate.Perform("A = (uint) 2; B = (uint) 1;").CheckValue(false, "C");
        slate.Perform("A = (uint)42; B = (uint)42;").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("less:LessThan<Double>")]
    public void TestOperators_less_LessThan_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A < B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThan<bool>");
        slate.Perform("A =  0.0;     B =  0.0;    ").CheckValue(false, "C");
        slate.Perform("A = -1.0;     B =  1.0;    ").CheckValue(true,  "C");
        slate.Perform("A =  1.0;     B = -1.0;    ").CheckValue(false, "C");
        slate.Perform("A =  1.00004; B =  1.00005;").CheckValue(true,  "C");
        slate.Perform("A =  1.00005; B =  1.00004;").CheckValue(false, "C");
        slate.Perform("A = -inf;     B =  inf;    ").CheckValue(true,  "C");
        slate.Perform("A =  inf;     B =  inf;    ").CheckValue(false, "C");
        slate.Perform("A =  inf;     B = -inf;    ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("less:LessThan<String>")]
    public void TestOperators_less_LessThan_String() {
        Slate slate = new Slate().Perform("in string A, B; C := A < B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThan<bool>");
        slate.Perform("A = '';            B = '';           ").CheckValue(false, "C");
        slate.Perform("A = 'Hello World'; B = 'Hello World';").CheckValue(false, "C");
        slate.Perform("A = 'mop';         B = 'pop';        ").CheckValue(true,  "C");
        slate.Perform("A = 'mop';         B = 'map';        ").CheckValue(false, "C");
        slate.Perform("A = 'mop';         B = 'Mop';        ").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("lessEqual:LessThanOrEqual<Int>")]
    public void TestOperators_lessEqual_LessThanOrEqual_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A <= B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThanOrEqual<bool>");
        slate.Perform("A =  0; B =  0;").CheckValue(true,  "C");
        slate.Perform("A = -1; B =  1;").CheckValue(true,  "C");
        slate.Perform("A =  1; B = -1;").CheckValue(false, "C");
        slate.Perform("A = 42; B = 42;").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("lessEqual:LessThanOrEqual<Uint>")]
    public void TestOperators_lessEqual_LessThanOrEqual_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A <= B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThanOrEqual<bool>");
        slate.Perform("A = (uint) 0; B = (uint) 0;").CheckValue(true,  "C");
        slate.Perform("A = (uint) 1; B = (uint) 2;").CheckValue(true,  "C");
        slate.Perform("A = (uint) 2; B = (uint) 1;").CheckValue(false, "C");
        slate.Perform("A = (uint)42; B = (uint)42;").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("lessEqual:LessThanOrEqual<Double>")]
    public void TestOperators_lessEqual_LessThanOrEqual_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A <= B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThanOrEqual<bool>");
        slate.Perform("A =  0.0;     B =  0.0;    ").CheckValue(true,  "C");
        slate.Perform("A = -1.0;     B =  1.0;    ").CheckValue(true,  "C");
        slate.Perform("A =  1.0;     B = -1.0;    ").CheckValue(false, "C");
        slate.Perform("A =  1.00004; B =  1.00005;").CheckValue(true,  "C");
        slate.Perform("A =  1.00005; B =  1.00004;").CheckValue(false, "C");
        slate.Perform("A = -inf;     B =  inf;    ").CheckValue(true,  "C");
        slate.Perform("A =  inf;     B =  inf;    ").CheckValue(true,  "C");
        slate.Perform("A =  inf;     B = -inf;    ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("lessEqual:LessThanOrEqual<String>")]
    public void TestOperators_lessEqual_LessThanOrEqual_String() {
        Slate slate = new Slate().Perform("in string A, B; C := A <= B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LessThanOrEqual<bool>");
        slate.Perform("A = '';            B = '';           ").CheckValue(true,  "C");
        slate.Perform("A = 'Hello World'; B = 'Hello World';").CheckValue(true,  "C");
        slate.Perform("A = 'mop';         B = 'pop';        ").CheckValue(true,  "C");
        slate.Perform("A = 'mop';         B = 'map';        ").CheckValue(false, "C");
        slate.Perform("A = 'mop';         B = 'Mop';        ").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("logicalAnd:And")]
    public void TestOperators_logicalAnd_And() {
        Slate slate = new Slate().Perform("in bool A, B; C := A && B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: And<bool>");
        slate.Perform("A = false; B = false;").CheckValue(false, "C");
        slate.Perform("A = false; B = true; ").CheckValue(false, "C");
        slate.Perform("A = true;  B = false;").CheckValue(false, "C");
        slate.Perform("A = true;  B = true; ").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("logicalAnd:All")]
    public void TestOperators_logicalAnd_All() {
        Slate slate = new Slate().Perform("in trigger A, B; C := A && B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: All<trigger>");
        slate.PerformWithoutReset("false -> A; false -> B;").CheckProvoked(false, "C").ResetTriggers();
        slate.PerformWithoutReset("false -> A; true  -> B;").CheckProvoked(false, "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; false -> B;").CheckProvoked(false, "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; true  -> B;").CheckProvoked(true,  "C").ResetTriggers();
    }

    [TestMethod]
    [TestTag("logicalOr:Or")]
    public void TestOperators_logicalOr_Or() {
        Slate slate = new Slate().Perform("in bool A, B; C := A || B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Or<bool>");
        slate.Perform("A = false; B = false;").CheckValue(false, "C");
        slate.Perform("A = false; B = true; ").CheckValue(true,  "C");
        slate.Perform("A = true;  B = false;").CheckValue(true,  "C");
        slate.Perform("A = true;  B = true; ").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("logicalOr:Any")]
    public void TestOperators_logicalOr_Any() {
        Slate slate = new Slate().Perform("in trigger A, B; C := A || B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Any<trigger>");
        slate.PerformWithoutReset("false -> A; false -> B;").CheckProvoked(false, "C").ResetTriggers();
        slate.PerformWithoutReset("false -> A; true  -> B;").CheckProvoked(true,  "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; false -> B;").CheckProvoked(true,  "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; true  -> B;").CheckProvoked(true,  "C").ResetTriggers();
    }

    [TestMethod]
    [TestTag("logicalXor:Xor")]
    public void TestOperators_logicalXor_Xor() {
        Slate slate = new Slate().Perform("in bool A, B; C := A ^^ B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Xor<bool>");
        slate.Perform("A = false; B = false;").CheckValue(false, "C");
        slate.Perform("A = false; B = true; ").CheckValue(true,  "C");
        slate.Perform("A = true;  B = false;").CheckValue(true,  "C");
        slate.Perform("A = true;  B = true; ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("logicalXor:XorTrigger")]
    public void TestOperators_logicalXor_XorTrigger() {
        Slate slate = new Slate().Perform("in trigger A, B; C := A ^^ B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Xor<trigger>");
        slate.PerformWithoutReset("false -> A; false -> B;").CheckProvoked(false, "C").ResetTriggers();
        slate.PerformWithoutReset("false -> A; true  -> B;").CheckProvoked(true,  "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; false -> B;").CheckProvoked(true,  "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; true  -> B;").CheckProvoked(false, "C").ResetTriggers();
    }

    [TestMethod]
    [TestTag("modulo:Mod<Int>")]
    public void TestOperators_modulo_Mod_Int() {
        Slate slate = new Slate().Perform("in int A, B = 1; C := A % B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mod<int>");
        slate.Perform("A =  4; B =  2;").CheckValue( 0, "C");
        slate.Perform("A =  3; B =  2;").CheckValue( 1, "C");
        slate.Perform("A =  9; B =  3;").CheckValue( 0, "C");
        slate.Perform("A =  1; B =  3;").CheckValue( 1, "C");
        slate.Perform("A =  8; B =  3;").CheckValue( 2, "C");
        slate.Perform("A =  8; B = -3;").CheckValue( 2, "C");
        slate.Perform("A = -8; B =  3;").CheckValue(-2, "C");
        slate.Perform("A = -8; B = -3;").CheckValue(-2, "C");
        TestTools.CheckException(() => slate.Perform("B = 0;"),
            "Attempted to divide by zero.");
    }

    [TestMethod]
    [TestTag("modulo:Mod<Uint>")]
    public void TestOperators_modulo_Mod_Uint() {
        Slate slate = new Slate().Perform("in uint A, B = (uint)1; C := A % B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mod<uint>");
        slate.Perform("A = (uint)4; B = (uint)2;").CheckValue((uint)0, "C");
        slate.Perform("A = (uint)3; B = (uint)2;").CheckValue((uint)1, "C");
        slate.Perform("A = (uint)9; B = (uint)3;").CheckValue((uint)0, "C");
        slate.Perform("A = (uint)1; B = (uint)3;").CheckValue((uint)1, "C");
        slate.Perform("A = (uint)8; B = (uint)3;").CheckValue((uint)2, "C");
        TestTools.CheckException(() => slate.Perform("B = (uint)0;"),
            "Attempted to divide by zero.");
    }

    [TestMethod]
    [TestTag("modulo:Mod<Double>")]
    public void TestOperators_modulo_Mod_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A % B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mod<double>");
        slate.Perform("A =  4.0;  B =  2.0; ").CheckValue( 0.0, "C");
        slate.Perform("A =  1.0;  B =  4.0; ").CheckValue( 1.0, "C");
        slate.Perform("A =  8.0;  B =  3.0; ").CheckValue( 2.0, "C");
        slate.Perform("A =  8.0;  B = -3.0; ").CheckValue( 2.0, "C");
        slate.Perform("A = -8.0;  B =  3.0; ").CheckValue(-2.0, "C");
        slate.Perform("A = -8.0;  B = -3.0; ").CheckValue(-2.0, "C");
        slate.Perform("A =  1.0;  B =  0.0; ").CheckValue(double.NaN, "C");
        slate.Perform("A =  0.0;  B =  0.0; ").CheckValue(double.NaN, "C");
        slate.Perform("A = -1.0;  B =  0.0; ").CheckValue(double.NaN, "C");
        slate.Perform("A =  3.13; B =  0.25;").CheckValue(3.13%0.25, "C");
    }

    [TestMethod]
    [TestTag("multiply:Mul<Int>")]
    public void TestOperators_multiply_Mul_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A * B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mul<int>");
        slate.Perform("A =  0; B =  0;").CheckValue(  0, "C");
        slate.Perform("A =  3; B =  0;").CheckValue(  0, "C");
        slate.Perform("A =  9; B =  1;").CheckValue(  9, "C");
        slate.Perform("A =  3; B =  4;").CheckValue( 12, "C");
        slate.Perform("A =  8; B =  3;").CheckValue( 24, "C");
        slate.Perform("A =  8; B = -3;").CheckValue(-24, "C");
        slate.Perform("A = -8; B =  3;").CheckValue(-24, "C");
        slate.Perform("A = -8; B = -3;").CheckValue( 24, "C");
    }

    [TestMethod]
    [TestTag("multiply:Mul<Uint>")]
    public void TestOperators_multiply_Mul_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A * B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mul<uint>");
        slate.Perform("A = (uint)0; B = (uint)0;").CheckValue((uint) 0, "C");
        slate.Perform("A = (uint)3; B = (uint)0;").CheckValue((uint) 0, "C");
        slate.Perform("A = (uint)9; B = (uint)1;").CheckValue((uint) 9, "C");
        slate.Perform("A = (uint)3; B = (uint)4;").CheckValue((uint)12, "C");
        slate.Perform("A = (uint)8; B = (uint)3;").CheckValue((uint)24, "C");
    }

    [TestMethod]
    [TestTag("multiply:Mul<Double>")]
    public void TestOperators_multiply_Mul_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A * B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Mul<double>");
        slate.Perform("A =  0.0;  B =  0.0; ").CheckValue( 0.0,     "C");
        slate.Perform("A =  1.0;  B =  0.0; ").CheckValue( 0.0,     "C");
        slate.Perform("A =  1.0;  B =  4.0; ").CheckValue( 4.0,     "C");
        slate.Perform("A =  2.0;  B =  4.0; ").CheckValue( 8.0,     "C");
        slate.Perform("A =  8.0;  B = -3.0; ").CheckValue(-24.0,    "C");
        slate.Perform("A = -8.0;  B =  3.0; ").CheckValue(-24.0,    "C");
        slate.Perform("A = -8.0;  B = -3.0; ").CheckValue( 24.0,    "C");
        slate.Perform("A =  1.02; B =  0.03;").CheckValue(  0.0306, "C");
    }

    [TestMethod]
    [TestTag("negate:Neg<Int>")]
    public void TestOperators_negate_Neg_Int() {
        Slate slate = new Slate().Perform("in int A; B := -A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Neg<int>");
        slate.Perform("A =  0;").CheckValue( 0,  "B");
        slate.Perform("A = -1;").CheckValue( 1,  "B");
        slate.Perform("A =  1;").CheckValue(-1,  "B");
        slate.Perform("A = 42;").CheckValue(-42, "B");
    }

    [TestMethod]
    [TestTag("negate:Neg<Double>")]
    public void TestOperators_negate_Neg_Double() {
        Slate slate = new Slate().Perform("in double A; B := -A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Neg<double>");
        slate.Perform("A =  0.0;    ").CheckValue( 0.0,     "B");
        slate.Perform("A = -1.0;    ").CheckValue( 1.0,     "B");
        slate.Perform("A =  1.00004;").CheckValue(-1.00004, "B");
        slate.Perform("A =  0.001;  ").CheckValue(-0.001,   "B");
        slate.Perform("A =  nan;    ").CheckValue(double.NaN, "B");
        slate.Perform("A =  inf;    ").CheckValue(double.NegativeInfinity, "B");
        slate.Perform("A = -inf;    ").CheckValue(double.PositiveInfinity, "B");
    }

    [TestMethod]
    [TestTag("not:Not")]
    public void TestOperators_not_Not() {
        Slate slate = new Slate().Perform("in bool A; B := !A;");
        slate.CheckNodeString(Stringifier.Basic(), "B", "B: Not<bool>");
        slate.Perform("A = false;").CheckValue(true,  "B");
        slate.Perform("A = true; ").CheckValue(false, "B");
    }

    [TestMethod]
    [TestTag("notEqual:NotEqual<Bool>")]
    public void TestOperators_notEqual_NotEqual_Bool() {
        Slate slate = new Slate().Perform("in bool A, B; C := A != B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");
        slate.Perform("A = false; B = false;").CheckValue(false, "C");
        slate.Perform("A = false; B = true; ").CheckValue(true,  "C");
        slate.Perform("A = true;  B = false;").CheckValue(true,  "C");
        slate.Perform("A = true;  B = true; ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("notEqual:NotEqual<Int>")]
    public void TestOperators_notEqual_NotEqual_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A != B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");
        slate.Perform("A =  0; B =  0;").CheckValue(false, "C");
        slate.Perform("A = -1; B =  1;").CheckValue(true,  "C");
        slate.Perform("A =  1; B = -1;").CheckValue(true,  "C");
        slate.Perform("A = 42; B = 42;").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("notEqual:NotEqual<Uint>")]
    public void TestOperators_notEqual_NotEqual_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A != B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");
        slate.Perform("A = (uint) 0; B = (uint) 0;").CheckValue(false, "C");
        slate.Perform("A = (uint) 1; B = (uint) 2;").CheckValue(true,  "C");
        slate.Perform("A = (uint) 2; B = (uint) 1;").CheckValue(true,  "C");
        slate.Perform("A = (uint)42; B = (uint)42;").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("notEqual:NotEqual<Double>")]
    public void TestOperators_notEqual_NotEqual_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A != B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");
        slate.Perform("A =  0.0;     B =  0.0;    ").CheckValue(false, "C");
        slate.Perform("A = -1.0;     B =  1.0;    ").CheckValue(true,  "C");
        slate.Perform("A =  1.0;     B = -1.0;    ").CheckValue(true,  "C");
        slate.Perform("A =  1.00004; B =  1.00005;").CheckValue(true,  "C");
        slate.Perform("A =  1.00005; B =  1.00004;").CheckValue(true,  "C");
        slate.Perform("A = -inf;     B =  inf;    ").CheckValue(true,  "C");
        slate.Perform("A =  inf;     B =  inf;    ").CheckValue(false, "C");
        slate.Perform("A =  inf;     B = -inf;    ").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("notEqual:NotEqual<Object>")]
    public void TestOperators_notEqual_NotEqual_Object() {
        Slate slate = new Slate().Perform("in object A, B; C := A != B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");
        slate.Perform("A = '';      B = '';     ").CheckValue(false, "C");
        slate.Perform("A = 'World'; B = 'Hello';").CheckValue(true,  "C");
        slate.Perform("A = false;   B = false;  ").CheckValue(false, "C");
        slate.Perform("A = false;   B = true;   ").CheckValue(true,  "C");
        slate.Perform("A = true;    B = false;  ").CheckValue(true,  "C");
        slate.Perform("A = true;    B = true;   ").CheckValue(false, "C");
        slate.Perform("A = 1;       B = 1;      ").CheckValue(false, "C");
        slate.Perform("A = 1;       B = 2;      ").CheckValue(true,  "C");
        slate.Perform("A = 2;       B = 1;      ").CheckValue(true,  "C");
        slate.Perform("A = 1.0;     B = 1;      ").CheckValue(true,  "C");
        slate.Perform("A = 1.0;     B = 1.0;    ").CheckValue(false, "C");
        slate.Perform("A = 1.0;     B = 1.1;    ").CheckValue(true,  "C");
        slate.Perform("A = 1.1;     B = 1.0;    ").CheckValue(true,  "C");

        slate.Perform("A = 1.0;     B = nan;    ").CheckValue(true,  "C");
        slate.Perform("A = nan;     B = nan;    ").CheckValue(false, "C");
        slate.Perform("A = 1.1;     B = inf;    ").CheckValue(true,  "C");
        slate.Perform("A = inf;     B = inf;    ").CheckValue(false, "C");
        slate.Perform("A = inf;     B = 1.0;    ").CheckValue(true,  "C");
        slate.Perform("A = 'inf';   B = inf;    ").CheckValue(true,  "C");
        slate.Perform("A = false;   B = null;   ").CheckValue(true,  "C");
        slate.Perform("A = 0;       B = null;   ").CheckValue(true,  "C");
        slate.Perform("A = '';      B = null;   ").CheckValue(true,  "C");
        slate.Perform("A = null;    B = null;   ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("notEqual:NotEqual<String>")]
    public void TestOperators_notEqual_NotEqual_String() {
        Slate slate = new Slate().Perform("in string A, B; C := A != B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: NotEqual<bool>");
        slate.Perform("A = '';            B = '';           ").CheckValue(false, "C");
        slate.Perform("A = 'Hello World'; B = 'Hello World';").CheckValue(false, "C");
        slate.Perform("A = 'mop';         B = 'pop';        ").CheckValue(true,  "C");
        slate.Perform("A = 'mop';         B = 'map';        ").CheckValue(true,  "C");
        slate.Perform("A = 'mop';         B = 'Mop';        ").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("or:Or")]
    public void TestOperators_or_Or() {
        Slate slate = new Slate().Perform("in bool A, B; C := A | B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Or<bool>");
        slate.Perform("A = false; B = false;").CheckValue(false, "C");
        slate.Perform("A = false; B = true; ").CheckValue(true,  "C");
        slate.Perform("A = true;  B = false;").CheckValue(true,  "C");
        slate.Perform("A = true;  B = true; ").CheckValue(true,  "C");
    }

    [TestMethod]
    [TestTag("or:BitwiseOr<Int>")]
    public void TestOperators_or_BitwiseOr_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A | B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: BitwiseOr<int>");
        slate.Perform("A = 0x00000000; B = 0x00000000;").CheckValue(unchecked((int)0x0000_0000), "C");
        slate.Perform("A = 0x11112222; B = 0x22221111;").CheckValue(unchecked((int)0x3333_3333), "C");
        slate.Perform("A = 0xFF00FF00; B = 0x00FF00FF;").CheckValue(unchecked((int)0xFFFF_FFFF), "C");
        slate.Perform("A = 0xC3C3C3C3; B = 0x0033CCFF;").CheckValue(unchecked((int)0xC3F3_CFFF), "C");
    }

    [TestMethod]
    [TestTag("or:BitwiseOr<Uint>")]
    public void TestOperators_or_BitwiseOr_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A | B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: BitwiseOr<uint>");
        slate.Perform("A = (uint)0x00000000; B = (uint)0x00000000;").CheckValue(unchecked((uint)0x0000_0000), "C");
        slate.Perform("A = (uint)0x11112222; B = (uint)0x22221111;").CheckValue(unchecked((uint)0x3333_3333), "C");
        slate.Perform("A = (uint)0xFF00FF00; B = (uint)0x00FF00FF;").CheckValue(unchecked((uint)0xFFFF_FFFF), "C");
        slate.Perform("A = (uint)0xC3C3C3C3; B = (uint)0x0033CCFF;").CheckValue(unchecked((uint)0xC3F3_CFFF), "C");
    }

    [TestMethod]
    [TestTag("or:Any")]
    public void TestOperators_or_Any() {
        Slate slate = new Slate().Perform("in trigger A, B; C := A | B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Any<trigger>");
        slate.PerformWithoutReset("false -> A; false -> B;").CheckProvoked(false, "C").ResetTriggers();
        slate.PerformWithoutReset("false -> A; true  -> B;").CheckProvoked(true,  "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; false -> B;").CheckProvoked(true,  "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; true  -> B;").CheckProvoked(true,  "C").ResetTriggers();
    }

    [TestMethod]
    [TestTag("power:BinaryFunc<Double, Double, Double>")]
    public void TestOperators_power_BinaryFunc_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A ** B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Pow<double>");
        slate.Perform("A =  0.0; B =  0.0;").CheckValue(1.0, "C");
        slate.Perform("A =  0.0; B =  1.0;").CheckValue(0.0, "C");
        slate.Perform("A =  1.2; B =  0.0;").CheckValue(1.0, "C");
        slate.Perform("A =  1.2; B =  1.0;").CheckValue(1.2, "C");
        slate.Perform("A =  1.0; B =  1.2;").CheckValue(1.0, "C");
        slate.Perform("A =  2.0; B =  1.2;").CheckValue(System.Math.Pow(2.0, 1.2), "C");
        slate.Perform("A = 10.0; B =  2.0;").CheckValue( 100.0,  "C");
        slate.Perform("A = 10.0; B = -2.0;").CheckValue(   0.01, "C");
        slate.Perform("A =  2.0; B =  9.0;").CheckValue( 512.0,  "C");
        slate.Perform("A = -2.0; B =  8.0;").CheckValue( 256.0,  "C");
        slate.Perform("A = -2.0; B =  9.0;").CheckValue(-512.0,  "C");
    }

    [TestMethod]
    [TestTag("shiftLeft:LeftShift<Int>")]
    public void TestOperators_shiftLeft_LeftShift_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A << B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LeftShift<int>");
        slate.Perform("A = 0x00000000; B =  3;").CheckValue(unchecked((int)0x0000_0000), "C");
        slate.Perform("A = 0xFFFFFFFF; B =  0;").CheckValue(unchecked((int)0xFFFF_FFFF), "C");
        slate.Perform("A = 0xFFFFFFFF; B =  1;").CheckValue(unchecked((int)0xFFFF_FFFE), "C");
        slate.Perform("A = 0xFFFFFFFF; B =  3;").CheckValue(unchecked((int)0xFFFF_FFF8), "C");
        slate.Perform("A = 0x12345678; B =  4;").CheckValue(unchecked((int)0x2345_6780), "C");
        slate.Perform("A = 0xFEDCBA98; B =  4;").CheckValue(unchecked((int)0xEDCB_A980), "C");
        slate.Perform("A = 0x12345678; B = 24;").CheckValue(unchecked((int)0x7800_0000), "C");
        // Left shifting will change signs as bit is pushed into the sign bit.
        slate.Perform("A = 0x7FFFFFFF; B =  1;").CheckValue(unchecked((int)0xFFFF_FFFE), "C");
        slate.Perform("A = 0x80000001; B =  4;").CheckValue(unchecked((int)0x0000_0010), "C");
        slate.Perform("A = 0x88000000; B =  4;").CheckValue(unchecked((int)0x8000_0000), "C");
        // Left shifting by a negative value is undefined,
        // See: https://en.wikipedia.org/wiki/Bitwise_operation#C-family_and_Python
    }

    [TestMethod]
    [TestTag("shiftLeft:LeftShift<Uint>")]
    public void TestOperators_shiftLeft_LeftShift_Uint() {
        Slate slate = new Slate().Perform("in { uint A; int B; } C := A << B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: LeftShift<uint>");
        slate.Perform("A = (uint)0x00000000; B =  3;").CheckValue(unchecked((uint)0x0000_0000), "C");
        slate.Perform("A = (uint)0xFFFFFFFF; B =  0;").CheckValue(unchecked((uint)0xFFFF_FFFF), "C");
        slate.Perform("A = (uint)0xFFFFFFFF; B =  1;").CheckValue(unchecked((uint)0xFFFF_FFFE), "C");
        slate.Perform("A = (uint)0xFFFFFFFF; B =  3;").CheckValue(unchecked((uint)0xFFFF_FFF8), "C");
        slate.Perform("A = (uint)0x12345678; B =  4;").CheckValue(unchecked((uint)0x2345_6780), "C");
        slate.Perform("A = (uint)0xFEDCBA98; B =  4;").CheckValue(unchecked((uint)0xEDCB_A980), "C");
        slate.Perform("A = (uint)0x12345678; B = 24;").CheckValue(unchecked((uint)0x7800_0000), "C");
        slate.Perform("A = (uint)0x7FFFFFFF; B =  1;").CheckValue(unchecked((uint)0xFFFF_FFFE), "C");
        slate.Perform("A = (uint)0x80000001; B =  4;").CheckValue(unchecked((uint)0x0000_0010), "C");
        slate.Perform("A = (uint)0x88000000; B =  4;").CheckValue(unchecked((uint)0x8000_0000), "C");
        // Left shifting by a negative value is undefined,
        // See: https://en.wikipedia.org/wiki/Bitwise_operation#C-family_and_Python
    }

    [TestMethod]
    [TestTag("shiftRight:RightShift<Int>")]
    public void TestOperators_shiftRight_RightShift_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A >> B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: RightShift<int>");
        slate.Perform("A = 0x00000000; B =  3;").CheckValue(unchecked((int)0x0000_0000), "C");
        slate.Perform("A = 0xFFFFFFFF; B =  0;").CheckValue(unchecked((int)0xFFFF_FFFF), "C");
        slate.Perform("A = 0x12345678; B =  4;").CheckValue(unchecked((int)0x0123_4567), "C");
        slate.Perform("A = 0x12345678; B = 24;").CheckValue(unchecked((int)0x0000_0012), "C");
        // Right shifting a negative value will push in 1's in the highest bit to keep the same sign.
        slate.Perform("A = 0x80000001; B =  4;").CheckValue(unchecked((int)0xF800_0000), "C");
        slate.Perform("A = 0x88000000; B =  4;").CheckValue(unchecked((int)0xF880_0000), "C");
        slate.Perform("A = 0xFFFFFFFF; B =  1;").CheckValue(unchecked((int)0xFFFF_FFFF), "C");
        slate.Perform("A = 0xFFFFFFFF; B =  3;").CheckValue(unchecked((int)0xFFFF_FFFF), "C");
        // Right shifting by a negative value is undefined,
        // See: https://en.wikipedia.org/wiki/Bitwise_operation#C-family_and_Python
    }

    [TestMethod]
    [TestTag("shiftRight:RightShift<Uint>")]
    public void TestOperators_shiftRight_RightShift_Uint() {
        Slate slate = new Slate().Perform("in { uint A; int B; } C := A >> B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: RightShift<uint>");
        slate.Perform("A = (uint)0x00000000; B =  3;").CheckValue(unchecked((uint)0x0000_0000), "C");
        slate.Perform("A = (uint)0xFFFFFFFF; B =  0;").CheckValue(unchecked((uint)0xFFFF_FFFF), "C");
        slate.Perform("A = (uint)0x12345678; B =  4;").CheckValue(unchecked((uint)0x0123_4567), "C");
        slate.Perform("A = (uint)0x12345678; B = 24;").CheckValue(unchecked((uint)0x0000_0012), "C");
        slate.Perform("A = (uint)0x80000001; B =  4;").CheckValue(unchecked((uint)0x0800_0000), "C");
        slate.Perform("A = (uint)0x88000000; B =  4;").CheckValue(unchecked((uint)0x0880_0000), "C");
        slate.Perform("A = (uint)0xFFFFFFFF; B =  1;").CheckValue(unchecked((uint)0x7FFF_FFFF), "C");
        slate.Perform("A = (uint)0xFFFFFFFF; B =  3;").CheckValue(unchecked((uint)0x1FFF_FFFF), "C");
        // Right shifting by a negative value is undefined,
        // See: https://en.wikipedia.org/wiki/Bitwise_operation#C-family_and_Python
    }

    [TestMethod]
    [TestTag("subtract:Sub<Int>")]
    public void TestOperators_subtract_Sub_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A - B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sub<int>");
        slate.Perform("A =   0; B =   0;").CheckValue(   0, "C");
        slate.Perform("A =   1; B =   0;").CheckValue(   1, "C");
        slate.Perform("A =   0; B =   1;").CheckValue(  -1, "C");
        slate.Perform("A =   1; B =   1;").CheckValue(   0, "C");
        slate.Perform("A =   0; B =  -1;").CheckValue(   1, "C");
        slate.Perform("A = 136; B =  24;").CheckValue( 112, "C");
        slate.Perform("A =  24; B = 136;").CheckValue(-112, "C");
    }

    [TestMethod]
    [TestTag("subtract:Sub<Uint>")]
    public void TestOperators_subtract_Sub_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A - B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sub<uint>");
        slate.Perform("A = (uint)  0; B = (uint)  0;").CheckValue(unchecked((uint)   0), "C");
        slate.Perform("A = (uint)  1; B = (uint)  0;").CheckValue(unchecked((uint)   1), "C");
        slate.Perform("A = (uint)  0; B = (uint)  1;").CheckValue(unchecked((uint)  -1), "C");
        slate.Perform("A = (uint)  1; B = (uint)  1;").CheckValue(unchecked((uint)   0), "C");
        slate.Perform("A = (uint)136; B = (uint) 24;").CheckValue(unchecked((uint) 112), "C");
        slate.Perform("A = (uint) 24; B = (uint)136;").CheckValue(unchecked((uint)-112), "C");
    }

    [TestMethod]
    [TestTag("subtract:Sub<Double>")]
    public void TestOperators_subtract_Sub_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A - B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sub<double>");
        slate.Perform("A =  0.0; B =  0.0;").CheckValue(  0.0,    "C");
        slate.Perform("A =  0.0; B = -1.0;").CheckValue(  1.0,    "C");
        slate.Perform("A =  1.2; B =  0.4;").CheckValue(1.2-0.4,  "C");
        slate.Perform("A =  0.4; B =  1.2;").CheckValue(0.4-1.2,  "C");
        slate.Perform("A = -1.4; B =  1.2;").CheckValue(-1.4-1.2, "C");
        slate.Perform("A = -1.4; B = -1.2;").CheckValue(-1.4+1.2, "C");
    }

    [TestMethod]
    [TestTag("sum:Sum<Int>")]
    public void TestOperators_sum_Sum_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A + B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sum<int>");
        slate.Perform("A =   0; B =   0;").CheckValue(  0, "C");
        slate.Perform("A =   1; B =   0;").CheckValue(  1, "C");
        slate.Perform("A =   0; B =   1;").CheckValue(  1, "C");
        slate.Perform("A =   1; B =   1;").CheckValue(  2, "C");
        slate.Perform("A =   0; B =  -1;").CheckValue( -1, "C");
        slate.Perform("A = 136; B =  24;").CheckValue(160, "C");
        slate.Perform("A =  24; B = 136;").CheckValue(160, "C");
    }

    [TestMethod]
    [TestTag("sum:Sum<Uint>")]
    public void TestOperators_sum_Sum_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A + B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sum<uint>");
        slate.Perform("A = (uint)  0; B = (uint)  0;").CheckValue((uint)  0, "C");
        slate.Perform("A = (uint)  1; B = (uint)  0;").CheckValue((uint)  1, "C");
        slate.Perform("A = (uint)  0; B = (uint)  1;").CheckValue((uint)  1, "C");
        slate.Perform("A = (uint)  1; B = (uint)  1;").CheckValue((uint)  2, "C");
        slate.Perform("A = (uint)136; B = (uint) 24;").CheckValue((uint)160, "C");
        slate.Perform("A = (uint) 24; B = (uint)136;").CheckValue((uint)160, "C");
    }

    [TestMethod]
    [TestTag("sum:Sum<Double>")]
    public void TestOperators_sum_Sum_Double() {
        Slate slate = new Slate().Perform("in double A, B; C := A + B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sum<double>");
        slate.Perform("A =  0.0; B =  0.0; ").CheckValue( 0.0,  "C");
        slate.Perform("A = -1.0; B =  0.25;").CheckValue(-0.75, "C");
        slate.Perform("A =  1.0; B =  0.25;").CheckValue( 1.25, "C");
        slate.Perform("A =  inf; B =  -inf;").CheckValue(double.NaN, "C");
        slate.Perform("A =  inf; B =  1e12;").CheckValue(double.PositiveInfinity, "C");
    }

    [TestMethod]
    [TestTag("sum:Sum<String>")]
    public void TestOperators_sum_Sum_String() {
        Slate slate = new Slate().Perform("in string A, B; C := A + B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Sum<string>");
        slate.Perform("A = ''; B = '';").CheckValue("", "C");
        slate.Perform("A = 'Hello'; B = 'World';").CheckValue("HelloWorld", "C");
        slate.Perform("A = 'A'; B = 'B';").CheckValue("AB", "C");
    }

    [TestMethod]
    [TestTag("ternary:SelectTrigger")]
    public void TestOperators_ternary_SelectTrigger() {
        Slate slate = new Slate().Perform("in bool A; in trigger B, C; D := A ? B : C;");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<trigger>");
        slate.PerformWithoutReset("A = false; B = false; C = false;").CheckProvoked(false, "D").ResetTriggers();
        slate.PerformWithoutReset("A = false; B = false; C = true; ").CheckProvoked(true, "D").ResetTriggers();
        slate.PerformWithoutReset("A = false; B = true;  C = false;").CheckProvoked(false, "D").ResetTriggers();
        slate.PerformWithoutReset("A = false; B = true;  C = true; ").CheckProvoked(true, "D").ResetTriggers();
        slate.PerformWithoutReset("A = true;  B = false; C = false;").CheckProvoked(false, "D").ResetTriggers();
        slate.PerformWithoutReset("A = true;  B = false; C = true; ").CheckProvoked(false, "D").ResetTriggers();
        slate.PerformWithoutReset("A = true;  B = true;  C = false;").CheckProvoked(true, "D").ResetTriggers();
        slate.PerformWithoutReset("A = true;  B = true;  C = true; ").CheckProvoked(true, "D").ResetTriggers();
    }

    [TestMethod]
    [TestTag("ternary:SelectValue<Bool>")]
    public void TestOperators_ternary_SelectValue_Bool() {
        Slate slate = new Slate().Perform("in bool A, B, C; D := A ? B : C;");
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
    [TestTag("ternary:SelectValue<Int>")]
    public void TestOperators_ternary_SelectValue_Int() {
        Slate slate = new Slate().Perform("in bool A; in int B, C; D := A ? B : C;");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<int>");
        slate.Perform("A = false; B = 10; C = 32;").CheckValue(32, "D");
        slate.Perform("A = true;  B = 42; C = 87;").CheckValue(42, "D");
    }

    [TestMethod]
    [TestTag("ternary:SelectValue<Uint>")]
    public void TestOperators_ternary_SelectValue_Uint() {
        Slate slate = new Slate().Perform("in bool A; in uint B, C; D := A ? B : C;");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<uint>");
        slate.Perform("A = false; B = (uint)10; C = (uint)32;").CheckValue((uint)32, "D");
        slate.Perform("A = true;  B = (uint)42; C = (uint)87;").CheckValue((uint)42, "D");
    }

    [TestMethod]
    [TestTag("ternary:SelectValue<Double>")]
    public void TestOperators_ternary_SelectValue_Double() {
        Slate slate = new Slate().Perform("in bool A; in double B, C; D := A ? B : C;");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<double>");
        slate.Perform("A = false; B =  1.23; C = 32.1;").CheckValue(32.1, "D");
        slate.Perform("A = true;  B = 42.5;  C = 55.3;").CheckValue(42.5, "D");
    }

    [TestMethod]
    [TestTag("ternary:SelectValue<Object>")]
    public void TestOperators_ternary_SelectValue_Object() {
        Slate slate = new Slate().Perform("in bool A; in object B, C; D := A ? B : C;");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<object>");
        slate.Perform("A = false; B = 'Goodbye'; C = 'Moon'; ").CheckObject("Moon",  "D");
        slate.Perform("A = true;  B = 'Hello';   C = 'World';").CheckObject("Hello", "D");
        slate.Perform("A = false; B = 'Goodbye'; C = 2;      ").CheckObject(2,       "D");
        slate.Perform("A = true;  B = false;     C = 0.2;    ").CheckObject(false,   "D");
        slate.Perform("A = false; B = true;      C = 0.4;    ").CheckObject(0.4,     "D");
        slate.Perform("A = true;  B = null;      C = 42;     ").CheckObject(null,    "D");
    }

    [TestMethod]
    [TestTag("ternary:SelectValue<String>")]
    public void TestOperators_ternary_SelectValue_String() {
        Slate slate = new Slate().Perform("in bool A; in string B, C; D := A ? B : C;");
        slate.CheckNodeString(Stringifier.Basic(), "D", "D: Select<string>");
        slate.Perform("A = false; B = 'Goodbye'; C = 'Moon'; ").CheckValue("Moon",  "D");
        slate.Perform("A = true;  B = 'Hello';   C = 'World';").CheckValue("Hello", "D");
    }

    [TestMethod]
    [TestTag("xor:Xor")]
    public void TestOperators_xor_Xor() {
        Slate slate = new Slate().Perform("in bool A, B; C := A ^ B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Xor<bool>");
        slate.Perform("A = false; B = false;").CheckValue(false, "C");
        slate.Perform("A = false; B = true; ").CheckValue(true,  "C");
        slate.Perform("A = true;  B = false;").CheckValue(true,  "C");
        slate.Perform("A = true;  B = true; ").CheckValue(false, "C");
    }

    [TestMethod]
    [TestTag("xor:BitwiseXor<Int>")]
    public void TestOperators_xor_BitwiseXor_Int() {
        Slate slate = new Slate().Perform("in int A, B; C := A ^ B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: BitwiseXor<int>");
        slate.Perform("A = 0x00000000; B = 0x00000000;").CheckValue(unchecked((int)0x0000_0000), "C");
        slate.Perform("A = 0x11112222; B = 0x22221111;").CheckValue(unchecked((int)0x3333_3333), "C");
        slate.Perform("A = 0xFF00FF00; B = 0x00FF00FF;").CheckValue(unchecked((int)0xFFFF_FFFF), "C");
        slate.Perform("A = 0xFF00FF00; B = 0xFFFFFFFF;").CheckValue(unchecked((int)0x00FF_00FF), "C");
        slate.Perform("A = 0xC3C3C3C3; B = 0x0033CCFF;").CheckValue(unchecked((int)0xC3F0_0F3C), "C");
    }

    [TestMethod]
    [TestTag("xor:BitwiseXor<Uint>")]
    public void TestOperators_xor_BitwiseXor_Uint() {
        Slate slate = new Slate().Perform("in uint A, B; C := A ^ B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: BitwiseXor<uint>");
        slate.Perform("A = (uint)0x00000000; B = (uint)0x00000000;").CheckValue(unchecked((uint)0x0000_0000), "C");
        slate.Perform("A = (uint)0x11112222; B = (uint)0x22221111;").CheckValue(unchecked((uint)0x3333_3333), "C");
        slate.Perform("A = (uint)0xFF00FF00; B = (uint)0x00FF00FF;").CheckValue(unchecked((uint)0xFFFF_FFFF), "C");
        slate.Perform("A = (uint)0xFF00FF00; B = (uint)0xFFFFFFFF;").CheckValue(unchecked((uint)0x00FF_00FF), "C");
        slate.Perform("A = (uint)0xC3C3C3C3; B = (uint)0x0033CCFF;").CheckValue(unchecked((uint)0xC3F0_0F3C), "C");
    }

    [TestMethod]
    [TestTag("xor:XorTrigger")]
    public void TestOperators_xor_XorTrigger() {
        Slate slate = new Slate().Perform("in trigger A, B; C := A ^ B;");
        slate.CheckNodeString(Stringifier.Basic(), "C", "C: Xor<trigger>");
        slate.PerformWithoutReset("false -> A; false -> B;").CheckProvoked(false, "C").ResetTriggers();
        slate.PerformWithoutReset("false -> A; true  -> B;").CheckProvoked(true,  "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; false -> B;").CheckProvoked(true,  "C").ResetTriggers();
        slate.PerformWithoutReset("true  -> A; true  -> B;").CheckProvoked(false, "C").ResetTriggers();
    }
}
