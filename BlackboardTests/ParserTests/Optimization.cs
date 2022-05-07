using Blackboard.Core;
using Blackboard.Core.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Optimization {

        [TestMethod]
        public void TestParseOptimization_Constants() {
            Slate slate = new();
            Formula formula = slate.LogRead(
                "in A = 21.0 + (1.0 + 3.0*2.0)*3.0;");
            formula.Check(
                "[",
                "  Global.A := Input<double>[0] {};",
                "  Input<double>[0] = <double>[42] {};",
                "  Finish",
                "]");
        }

        [TestMethod]
        public void TestParseOptimization_IncorporateParents() {
            Slate slate = new();
            slate.Read("in int A, B, C, D;").Perform();
            Formula formula = slate.LogRead(
                "E := (A + B + 2) + ((C + D) + 3);");
            formula.Check(
                "[",
                "  Global.E := Sum<int>[0](A[0], B[0], C[0], D[0], <int>[5]) {Sum<int>[0]};",
                "  Finish",
                "]");
        }

        [TestMethod]
        public void TestParseOptimization_StringReduction() {
            Slate slate = new();
            slate.Read("in string A = '! '; B := 3;").Perform();
            Formula formula = slate.LogRead(
                "E := ('Hello' + ' ' + 'World') + A + ('The ' + B + ' hot dogs cost $' + 12.0 + '.');");
            formula.Check(
                "[",
                "  Global.E := Sum<string>[](<string>[Hello World], A[! ], <string>[The 3 hot dogs cost $12.]) {Sum<string>[]};",
                "  Finish",
                "]");
        }
    }
}
