using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Assignments {

        [TestMethod]
        public void TestBasicParses_TypedInput() {
            Driver driver = new();
            driver.ReadCommit(
                "in int A = 2, B = 3;",
                "in bool C = true;",
                "",
                "namespace D {",
                "   in double E = 3.14;",
                "   in int F, G;",
                "   in bool H;",
                "   in double I;",
                "}");

            driver.CheckValue(2, "A");
            driver.CheckValue(3, "B");
            driver.CheckValue(true, "C");
            driver.CheckValue(3.14, "D", "E");
            driver.CheckValue(0, "D", "F");
            driver.CheckValue(0, "D", "G");
            driver.CheckValue(false, "D", "H");
            driver.CheckValue(0.0, "D", "I");
        }

        [TestMethod]
        public void TestBasicParses_VarInput() {
            Driver driver = new();
            driver.ReadCommit(
                "in A = 2, B = 3;",
                "in C = true;",
                "",
                "namespace D {",
                "   in E = 3.14;",
                "   in var F = 0, G = 0.0, H = false;",
                "}");

            driver.CheckValue(2, "A");
            driver.CheckValue(3, "B");
            driver.CheckValue(true, "C");
            driver.CheckValue(3.14, "D", "E");
            driver.CheckValue(0, "D", "F");
            driver.CheckValue(0.0, "D", "G");
            driver.CheckValue(false, "D", "H");
        }

        [TestMethod]
        public void TestBasicParses_DoubleLiteral() {
            Driver driver = new();
            IAction formula = driver.Read(
                "in double A = 3.0;",
                "in double B = 0.003;",
                "in double C = 3.0e-3;",
                "in double D = 3e-3;",
                "in double E = 0.3e-2;",
                "in double F = 3;",
                "in double G = 0;",
                "in double H = 0.0;",
                "in double I = 1.0;",
                "in double J = 0e-5;",
                "in double K = 28.0;");
            formula.Check(
                "Namespace.A := Input<double>[0];", "Input<double>[0] = Literal<double>[3];",
                "Namespace.B := Input<double>[0];", "Input<double>[0] = Literal<double>[0.003];",
                "Namespace.C := Input<double>[0];", "Input<double>[0] = Literal<double>[0.003];",
                "Namespace.D := Input<double>[0];", "Input<double>[0] = Literal<double>[0.003];",
                "Namespace.E := Input<double>[0];", "Input<double>[0] = Literal<double>[0.003];",
                "Namespace.F := Input<double>[0];", "Input<double>[0] = Implicit<double>[0](Literal<int>);",
                "Namespace.G := Input<double>[0];", "Input<double>[0] = Implicit<double>[0](Literal<int>);",
                "Namespace.H := Input<double>[0];", "Input<double>[0] = Literal<double>[0];",
                "Namespace.I := Input<double>[0];", "Input<double>[0] = Literal<double>[1];",
                "Namespace.J := Input<double>[0];", "Input<double>[0] = Literal<double>[0];",
                "Namespace.K := Input<double>[0];", "Input<double>[0] = Literal<double>[28];");
            formula.Perform(driver);

            driver.CheckValue(3.0, "A");
            driver.CheckValue(0.003, "B");
            driver.CheckValue(0.003, "C");
            driver.CheckValue(0.003, "D");
            driver.CheckValue(0.003, "E");
            driver.CheckValue(3.0, "F");
            driver.CheckValue(0.0, "G");
            driver.CheckValue(0.0, "H");
            driver.CheckValue(1.0, "I");
            driver.CheckValue(0.0, "J");
            driver.CheckValue(28.0, "K");
        }

        [TestMethod]
        public void TestBasicParses_LiteralMath() {
            Driver driver = new();
            driver.ReadCommit(
                "in double A = 3.0 + 0.07 * 2;",
                "in double B = floor(A), C = round(A), D = round(A, 1);",
                "in double E = (B ** C) / 2;",
                "in double F = -E + -3;");

            driver.CheckValue(3.14, "A");
            driver.CheckValue(3.0, "B");
            driver.CheckValue(3.0, "C");
            driver.CheckValue(3.1, "D");
            driver.CheckValue(13.5, "E");
            driver.CheckValue(-16.5, "F");
        }

        [TestMethod]
        public void TestBasicParses_ModRemAndStrings() {
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.math.ieeeremainder?view=net-5.0
            Driver driver = new();
            driver.ReadCommit(
                "in string A = (  3.0 %%  2.0) + ', ' + (  3.0 %  2.0);",
                "in string B = (  4.0 %%  2.0) + ', ' + (  4.0 %  2.0);",
                "in string C = ( 10.0 %%  3.0) + ', ' + ( 10.0 %  3.0);",
                "in string D = ( 11.0 %%  3.0) + ', ' + ( 11.0 %  3.0);",
                "in string E = ( 27.0 %%  4.0) + ', ' + ( 27.0 %  4.0);",
                "in string F = ( 28.0 %%  5.0) + ', ' + ( 28.0 %  5.0);",
                "in string G = ( 17.8 %%  4.0) + ', ' + ( 17.8 %  4.0);",
                "in string H = ( 17.8 %%  4.1) + ', ' + ( 17.8 %  4.1);",
                "in string I = (-16.3 %%  4.1) + ', ' + (-16.3 %  4.1);",
                "in string J = ( 17.8 %% -4.1) + ', ' + ( 17.8 % -4.1);",
                "in string K = (-17.8 %% -4.1) + ', ' + (-17.8 % -4.1);");

            driver.CheckValue("-1, 1", "A");
            driver.CheckValue("0, 0", "B");
            driver.CheckValue("1, 1", "C");
            driver.CheckValue("-1, 2", "D");
            driver.CheckValue("-1, 3", "E");
            driver.CheckValue("-2, 3", "F");
            driver.CheckValue("1.8000000000000007, 1.8000000000000007", "G");
            driver.CheckValue("1.4000000000000021, 1.4000000000000021", "H");
            driver.CheckValue("0.09999999999999787, -4.000000000000002", "I");
            driver.CheckValue("1.4000000000000021, 1.4000000000000021", "J");
            driver.CheckValue("-1.4000000000000021, -1.4000000000000021", "K");
        }

        [TestMethod]
        public void TestBasicParses_Assignment() {
            Driver driver = new();
            driver.ReadCommit(
                "in int A = 2, B = 5;",
                "in int C = A = 8;",
                "B = A = 14;",
                "A = 6;");

            driver.CheckValue(6, "A");
            driver.CheckValue(14, "B");
            driver.CheckValue(8, "C");
        }

        [TestMethod]
        public void TestBasicParses_NamespaceAssignment() {
            Driver driver = new();
            driver.ReadCommit(
                "namespace X {",
                "   in int a = 2;",
                "   in int b = 3;",
                "   in int c = 4;",
                "   namespace Y {",
                "      in int d = 5;",
                "      in int e = 6;",
                "      in int f = 7;",
                "      c = 444;",
                "      f = 777;",
                "   }",
                "   Y.e = 666;",
                "   b = 333;",
                "}",
                "X.Y.d = 555;",
                "X.a = 222;");

            driver.CheckValue(222, "X", "a");
            driver.CheckValue(333, "X", "b");
            driver.CheckValue(444, "X", "c");
            driver.CheckValue(555, "X", "Y", "d");
            driver.CheckValue(666, "X", "Y", "e");
            driver.CheckValue(777, "X", "Y", "f");
        }

        [TestMethod]
        public void TestBasicParses_DoubleToIntAssignError() {
            Driver driver = new();
            Parser parser = new(driver);
            TestTools.CheckException(() => parser.Read("in int A = 3.14;"),
                "Error occurred while parsing input code.",
               "May not assign the value to that type of input.",
               "[Location: Unnamed:1, 15, 15]",
               "[Input Type: int]",
               "[Value Type: double]");
        }
    }
}
