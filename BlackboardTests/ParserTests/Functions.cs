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






    }
}
