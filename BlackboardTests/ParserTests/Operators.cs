using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
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
            HashSet<string> testedTags = TestTools.TestTags(typeof(Operators));

            HashSet<string> opTags = new();
            Slate slate = new();
            Namespace opList = slate.Global[Slate.OperatorNamespace] as Namespace;
            foreach (KeyValuePair<string, INode> pair in opList.Fields) {
                IFuncGroup group = pair.Value as IFuncGroup;
                foreach (IFuncDef def in group.Definitions)
                    opTags.Add(pair.Key+":"+def.ReturnType.FormattedTypeName());
            }

            List<string> notTested = opTags.WhereNot(testedTags.Contains).ToList();
            List<string> notAnOp = testedTags.WhereNot(opTags.Contains).ToList();

            if (notAnOp.Count > 0 || notTested.Count > 0) {
                Assert.Fail("Tests do not match the existing operations:\n" +
                    "Not Tested (" + notTested.Count + "):\n  " + notTested.Join("\n  ") + "\n" +
                    "Not an Op (" + notAnOp.Count + "):\n  " + notAnOp.Join("\n  "));
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







    }
}
