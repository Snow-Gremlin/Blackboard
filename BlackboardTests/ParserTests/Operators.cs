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
            slate.PerformWithoutReset("A =  0.1;").CheckValue( 0, "B").ResetTriggers();
            slate.PerformWithoutReset("A =  0.9;").CheckValue( 0, "B").ResetTriggers();
            slate.PerformWithoutReset("A =  2.1;").CheckValue( 2, "B").ResetTriggers();
            slate.PerformWithoutReset("A =  2.9;").CheckValue( 2, "B").ResetTriggers();
            slate.PerformWithoutReset("A = -4.2;").CheckValue(-4, "B").ResetTriggers();
        }

        [TestMethod]
        [TestTag("castDouble:Implicit<Int, Double>")]
        public void TestOperators_castDouble_Implicit_Int_Double() {
            Slate slate = new Slate().Perform("in int A; double B := A;");
            slate.CheckNodeString(Stringifier.Basic(), "B", "B: Implicit<double>");
            slate.PerformWithoutReset("A =  0;").CheckValue( 0.0, "B").ResetTriggers();
            slate.PerformWithoutReset("A =  1;").CheckValue( 1.0, "B").ResetTriggers();
            slate.PerformWithoutReset("A =  2;").CheckValue( 2.0, "B").ResetTriggers();
            slate.PerformWithoutReset("A = -1;").CheckValue(-1.0, "B").ResetTriggers();
        }

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
                    "Not Tested: [" + notTested.Join(", ") + "]\n" +
                    "Not an Op:  [" + notAnOp.Join(", ") + "]");
            }
        }
    }
}
