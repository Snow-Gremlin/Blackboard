using Blackboard.Core;
using Blackboard.Core.Record;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests;

[TestClass]
public class Literals {

    static private void checkLit<T>(string input, T exp) {
        Slate slate = new();
        Result r = slate.Read("get a = "+input+";").Perform();
        T? value = (T?)r.GetValueAsObject("a");
        Assert.AreEqual(exp, value, "For: "+input);
    }

    [TestMethod]
    public void TestLiteral_Bool() {
        checkLit("true", true);
        checkLit("false", false);
    }
    
    [TestMethod]
    public void TestLiteral_ZeroInt() {
        checkLit("0", 0);
        checkLit("00", 0);
        checkLit("0_0", 0);
        checkLit("0b", 0);
        checkLit("00b", 0);
        checkLit("0_0b", 0);
        checkLit("0o", 0);
        checkLit("00o", 0);
        checkLit("0_0o", 0);
        checkLit("0d", 0);
        checkLit("00d", 0);
        checkLit("0_0d", 0);
        checkLit("0x0", 0);
        checkLit("0x00", 0);
        checkLit("0x0_0", 0);
    }
    
    [TestMethod]
    public void TestLiteral_ZeroDouble() {
        checkLit("0.0", 0.0);
        checkLit("00.0", 0.0);
        checkLit("0_0.0", 0.0);
        checkLit("0.00", 0.0);
        checkLit("0.0_0", 0.0);
        checkLit("00.00", 0.0);
        checkLit("0_0.0_0", 0.0);

        checkLit("0e0", 0.0);
        checkLit("0e00", 0.0);
        checkLit("0e0_0", 0.0);
        checkLit("0.0e0", 0.0);
        checkLit("00.00e00", 0.0);
        checkLit("0_0.0_0e0_0", 0.0);
    }

    [TestMethod]
    public void TestLiteral_Binary() {
        checkLit("0b", 0);
        checkLit("1b", 1);

        checkLit("00b", 0);
        checkLit("01b", 1);
        checkLit("10b", 2);
        checkLit("11b", 3);
    
        checkLit("0000_0001b", 1);
        checkLit("0000_0010b", 2);
        checkLit("0000_0100b", 4);
        checkLit("0000_1000b", 8);
        checkLit("0001_0000b", 16);
        checkLit("0010_0000b", 32);
        checkLit("0100_0000b", 64);
        checkLit("1000_0000b", 128);
        
        checkLit("1001_0110_1010_0101b", 0x96A5);
        checkLit("1001011010100101b", 0x96A5);
        checkLit("1_0_01_011_010_100101b", 0x96A5);
    }

    [TestMethod]
    public void TestLiteral_Octal() {
        checkLit("0o", 0);
        checkLit("1o", 1);
        checkLit("2o", 2);
        checkLit("7o", 7);
        checkLit("10o", 8);
        checkLit("100o", 64);
        checkLit("1_000o", 512);
    }

    [TestMethod]
    public void TestLiteral_Decimal() {
        checkLit("0", 0);
        checkLit("1", 1);
        checkLit("12", 12);
        checkLit("123", 123);
        checkLit("1_234", 1234);
        checkLit("12_345", 12345);
        checkLit("123_456", 123456);
        checkLit("1_234_567", 1234567);
        checkLit("12_345_678", 12345678);
        checkLit("123_456_789", 123456789);
        checkLit("987_654_321", 987654321);
        checkLit("9_8765_4_32_1", 987654321);
        checkLit("987654321", 987654321);

        checkLit("0d", 0);
        checkLit("1d", 1);
        checkLit("12d", 12);
        checkLit("123d", 123);
        checkLit("1_234d", 1234);
        checkLit("12_345d", 12345);
        checkLit("123_456d", 123456);
        checkLit("1_234_567d", 1234567);
        checkLit("12_345_678d", 12345678);
        checkLit("123_456_789d", 123456789);
        checkLit("987_654_321d", 987654321);
    }

    [TestMethod]
    public void TestLiteral_Hexadecimal() {
        checkLit("0x0", 0);
        checkLit("0x0000", 0);
        checkLit("0x000F", 15);
        checkLit("0x0010", 16);
        checkLit("0x10_10", 0x1010);
        checkLit("0x5577FF00", 0x5577FF00);
        checkLit("0x5577_FF00", 0x5577FF00);
        checkLit("0x55_77_FF_00", 0x5577FF00);
        checkLit("0x5_5_77FF0_0", 0x5577FF00);
        checkLit("0x7FFF_FFFF", 0x7FFF_FFFF);
    }

    [TestMethod]
    public void TestLiteral_Double() {
        checkLit("0.0", 0.0);
        checkLit("1.23", 1.23);
        checkLit("12.34", 12.34);
        checkLit("0e0", 0.0);
        checkLit("1e0", 1.0);
        checkLit("1e1", 10.0);
        checkLit("1e+1", 10.0);
        checkLit("1e-1", 0.1);
        checkLit("1.234e0", 1.234);
        checkLit("1.234e2", 123.4);
        checkLit("1.23_4e0_2", 123.4);
        checkLit("12.34e56", 12.34e56);
        checkLit("1_2.3_4e5_6", 12.34e56);
    }
    
    [TestMethod]
    public void TestLiteral_String() {
        checkLit("\"\"",            "");
        checkLit("''",              "");
        checkLit("\"   \"",         "   ");
        checkLit("'   '",           "   ");
        checkLit("\"Hello\"",       "Hello");
        checkLit("'Hello'",         "Hello");
        checkLit("\"'ello\"",       "'ello");
        checkLit("'\\'ello'",       "'ello");
        checkLit("\"\\\"ello\"",    "\"ello");
        checkLit("'\\\"ello'",      "\"ello");
        checkLit("\"🎃☠️\"",        "🎃☠️");
        checkLit("'🎃☠️'",          "🎃☠️");
        checkLit("\"\\\\ \\0\"",    "\\ \0");
        checkLit("'\\\\ \\0'",      "\\ \0");
        checkLit("\"\\b\\f\\v\"",   "\b\f\v");
        checkLit("'\\b\\f\\v'",     "\b\f\v");
        checkLit("\"\\n\\r\\t\"",   "\n\r\t");
        checkLit("'\\n\\r\\t'",     "\n\r\t");
        checkLit("\"\\x0A\"",       "\n");
        checkLit("'\\x0A'",         "\n");
        checkLit("\"\\u000A\"",     "\n");
        checkLit("'\\u000A'",       "\n");
        checkLit("\"\\u2029\"",     "\x2029");
        checkLit("'\\u2029'",       "\x2029");
        checkLit("\"\\U0001F47D\"", "👽");
        checkLit("'\\U0001F47D'",   "👽");
    }
}
