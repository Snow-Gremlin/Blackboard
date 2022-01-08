using Blackboard.Core;
using Blackboard.Core.Inspect;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Assignments {

        [TestMethod]
        public void TestBasicParses_TypedInput() {
            Slate slate = new();
            slate.Read(
                "in int A = 2, B = 3;",
                "in bool C = true;",
                "",
                "namespace D {",
                "   in double E = 3.14;",
                "   in int F, G;",
                "   in bool H;",
                "   in double I;",
                "}").
                Perform();

            slate.CheckValue(2, "A");
            slate.CheckValue(3, "B");
            slate.CheckValue(true, "C");
            slate.CheckValue(3.14, "D", "E");
            slate.CheckValue(0, "D", "F");
            slate.CheckValue(0, "D", "G");
            slate.CheckValue(false, "D", "H");
            slate.CheckValue(0.0, "D", "I");
        }

        [TestMethod]
        public void TestBasicParses_VarInput() {
            Slate slate = new();
            slate.Read(
                "in A = 2, B = 3;",
                "in C = true;",
                "",
                "namespace D {",
                "   in E = 3.14;",
                "   in var F = 0, G = 0.0, H = false;",
                "}").
                Perform();

            slate.CheckValue(2, "A");
            slate.CheckValue(3, "B");
            slate.CheckValue(true, "C");
            slate.CheckValue(3.14, "D", "E");
            slate.CheckValue(0, "D", "F");
            slate.CheckValue(0.0, "D", "G");
            slate.CheckValue(false, "D", "H");
        }

        [TestMethod]
        public void TestBasicParses_DoubleLiteral() {
            Slate slate = new();
            slate.Read(
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
                "in double K = 28.0;").
                Perform();

            slate.CheckValue( 3.0,   "A");
            slate.CheckValue( 0.003, "B");
            slate.CheckValue( 0.003, "C");
            slate.CheckValue( 0.003, "D");
            slate.CheckValue( 0.003, "E");
            slate.CheckValue( 3.0,   "F");
            slate.CheckValue( 0.0,   "G");
            slate.CheckValue( 0.0,   "H");
            slate.CheckValue( 1.0,   "I");
            slate.CheckValue( 0.0,   "J");
            slate.CheckValue(28.0,   "K");
        }

        [TestMethod]
        public void TestBasicParses_LiteralMath() {
            Slate slate = new();
            slate.Read(
                "in double A = 3.0 + 0.07 * 2;",
                "in double B = floor(A), C = round(A), D = round(A, 1);",
                "in double E = (B ** C) / 2;",
                "in double F = -E + -3;").
                Perform();

            slate.CheckValue(  3.14, "A");
            slate.CheckValue(  3.0,  "B");
            slate.CheckValue(  3.0,  "C");
            slate.CheckValue(  3.1,  "D");
            slate.CheckValue( 13.5,  "E");
            slate.CheckValue(-16.5,  "F");
        }

        [TestMethod]
        public void TestBasicParses_ModAndStrings() {
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.math.ieeeremainder?view=net-5.0
            Slate slate = new();
            slate.Read(
                "in string A =   3.0 + ' % ' +  2.0 + ' = ' + (  3.0 %  2.0);",
                "in string B =   4.0 + ' % ' +  2.0 + ' = ' + (  4.0 %  2.0);",
                "in string C =  10.0 + ' % ' +  3.0 + ' = ' + ( 10.0 %  3.0);",
                "in string D =  11.0 + ' % ' +  3.0 + ' = ' + ( 11.0 %  3.0);",
                "in string E =  27.0 + ' % ' +  4.0 + ' = ' + ( 27.0 %  4.0);",
                "in string F =  28.0 + ' % ' +  5.0 + ' = ' + ( 28.0 %  5.0);",
                "in string G =  17.8 + ' % ' +  4.0 + ' = ' + ( 17.8 %  4.0);",
                "in string H =  17.8 + ' % ' +  4.1 + ' = ' + ( 17.8 %  4.1);",
                "in string I = -16.3 + ' % ' +  4.1 + ' = ' + (-16.3 %  4.1);",
                "in string J =  17.8 + ' % ' + -4.1 + ' = ' + ( 17.8 % -4.1);",
                "in string K = -17.8 + ' % ' + -4.1 + ' = ' + (-17.8 % -4.1);").
                Perform();

            slate.CheckValue("3 % 2 = 1", "A");
            slate.CheckValue("4 % 2 = 0", "B");
            slate.CheckValue("10 % 3 = 1", "C");
            slate.CheckValue("11 % 3 = 2", "D");
            slate.CheckValue("27 % 4 = 3", "E");
            slate.CheckValue("28 % 5 = 3", "F");
            slate.CheckValue("17.8 % 4 = 1.8000000000000007", "G");
            slate.CheckValue("17.8 % 4.1 = 1.4000000000000021", "H");
            slate.CheckValue("-16.3 % 4.1 = -4.000000000000002", "I");
            slate.CheckValue("17.8 % -4.1 = 1.4000000000000021", "J");
            slate.CheckValue("-17.8 % -4.1 = -1.4000000000000021", "K");
        }

        [TestMethod]
        public void TestBasicParses_Assignment() {
            Slate slate = new();
            slate.Read(
                "in int A = 2, B = 5;",
                "in int C = A = 8;",
                "B = A = 14;",
                "A = 6;").
                Perform();

            slate.CheckValue(6, "A");
            slate.CheckValue(14, "B");
            slate.CheckValue(8, "C");
        }

        [TestMethod]
        public void TestBasicParses_NamespaceAssignment() {
            Slate slate = new();
            slate.Read(
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
                "X.a = 222;").
                Perform();

            slate.CheckValue(222, "X", "a");
            slate.CheckValue(333, "X", "b");
            slate.CheckValue(444, "X", "c");
            slate.CheckValue(555, "X", "Y", "d");
            slate.CheckValue(666, "X", "Y", "e");
            slate.CheckValue(777, "X", "Y", "f");
        }

        [TestMethod]
        public void TestBasicParses_DoubleToIntAssignError() {
            Slate slate = new();
            Parser parser = new(slate);
            TestTools.CheckException(() => parser.Read("in int A = 3.14;"),
               "Error occurred while parsing input code.",
               "The value type can not be cast to the given type.",
               "[Location: Unnamed:1, 15, 15]",
               "[Target: int]",
               "[Type: double]",
               "[Value: Literal<double>[3.14]]");
        }
    }
}
