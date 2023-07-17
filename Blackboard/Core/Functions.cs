using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;

namespace Blackboard.Core;

/// <summary>Group of all initial methods for Blackboard.</summary>
static internal class Functions {

    /// <summary>This adds all global initial methods for Blackboard.</summary>
    /// <param name="global">The global namespace for the slate.</param>
    static public void Add(Namespace global) {
        void add(string name, params IFuncDef[] defs) =>
            global[name] = new FuncGroup(defs);

        add("abs",
            Abs<Int>.Factory,
            Abs<Float>.Factory,
            Abs<Double>.Factory);
        
        add("acos",
            UnaryFloatingPoint<Float>.Acos,
            UnaryFloatingPoint<Double>.Acos);
        
        add("acosh",
            UnaryFloatingPoint<Float>.Acosh,
            UnaryFloatingPoint<Double>.Acosh);
        
        add("all",
            All.Factory);
        
        add("and",
            And.Factory,
            BitwiseAnd<Int>.Factory);
        
        add("any",
            Any.Factory);
        
        add("asin",
            UnaryFloatingPoint<Float>.Asin,
            UnaryFloatingPoint<Double>.Asin);
        
        add("asinh",
            UnaryFloatingPoint<Float>.Asinh,
            UnaryFloatingPoint<Double>.Asinh);
        
        add("atan",
            UnaryFloatingPoint<Float>.Atan,
            UnaryFloatingPoint<Double>.Atan);
        
        add("atanh",
            UnaryFloatingPoint<Float>.Atanh,
            UnaryFloatingPoint<Double>.Atanh);
        
        add("average",
            Average.Factory);
        
        add("cbrt",
            UnaryFloatingPoint<Float>.Cbrt,
            UnaryFloatingPoint<Double>.Cbrt);
        
        add("ceiling",
            UnaryFloatingPoint<Float>.Ceiling,
            UnaryFloatingPoint<Double>.Ceiling);
        
        add("clamp",
            TernaryComparable<Int>.Clamp,
            TernaryComparable<Uint>.Clamp,
            TernaryComparable<Float>.Clamp,
            TernaryComparable<Double>.Clamp);
        
        add("contains",
            Binary.Contains);
        
        add("cos",
            UnaryFloatingPoint<Float>.Cos,
            UnaryFloatingPoint<Double>.Cos);
        
        add("cosh",
            UnaryFloatingPoint<Float>.Cosh,
            UnaryFloatingPoint<Double>.Cosh);
        
        add("endsWith",
            Binary.EndsWith);
        
        add("epsilon",
            EpsilonEqual<Float>.Factory,
            EpsilonEqual<Double>.Factory);
        
        add("exp",
            UnaryFloatingPoint<Float>.Exp,
            UnaryFloatingPoint<Double>.Exp);
        
        add("floor",
            UnaryFloatingPoint<Float>.Floor,
            UnaryFloatingPoint<Double>.Floor);
        
        add("format",
            Format.Factory);
        
        add("implies",
            Implies.Factory);
        
        add("indexOf",
            Binary.IndexOf,
            Ternary.IndexOf);
        
        add("inRange",
            TernaryComparable<Int>.InRange,
            TernaryComparable<Uint>.InRange,
            TernaryComparable<Float>.InRange,
            TernaryComparable<Double>.InRange);
        
        add("insert",
            Ternary.Insert);
        
        add("isEmpty",
            Unary.IsEmpty);
        
        add("isFinite",
            UnaryFloatingPoint<Float>.IsFinite,
            UnaryFloatingPoint<Double>.IsFinite);
        
        add("isInf",
            UnaryFloatingPoint<Float>.IsInfinity,
            UnaryFloatingPoint<Double>.IsInfinity);
        
        add("isNaN",
            UnaryFloatingPoint<Float>.IsNaN,
            UnaryFloatingPoint<Double>.IsNaN);
        
        add("isNeg",
            UnarySigned<Int>.IsNegative,
            UnarySigned<Float>.IsNegative,
            UnarySigned<Double>.IsNegative);
        
        add("isNegInf",
            UnaryFloatingPoint<Float>.IsNegativeInfinity,
            UnaryFloatingPoint<Double>.IsNegativeInfinity);
        
        add("isNormal",
            UnaryFloatingPoint<Float>.IsNormal,
            UnaryFloatingPoint<Double>.IsNormal);
        
        add("isNull",
            UnaryNullable<Object>.IsNull);
        
        add("isPosInf",
            UnaryFloatingPoint<Float>.IsPositiveInfinity,
            UnaryFloatingPoint<Double>.IsPositiveInfinity);
        
        add("isSubnormal",
            UnaryFloatingPoint<Float>.IsSubnormal,
            UnaryFloatingPoint<Double>.IsSubnormal);
        
        add("latch",
            Latch<Bool>.Factory,
            Latch<Int>.Factory,
            Latch<Uint>.Factory,
            Latch<Float>.Factory,
            Latch<Double>.Factory,
            Latch<String>.Factory,
            Latch<Object>.Factory);
        
        add("lerp",
            Lerp<Float>.Factory,
            Lerp<Double>.Factory);
        
        add("length",
            Unary.Length);
        
        add("log",
            BinaryFloatingPoint<Float>.Log,
            BinaryFloatingPoint<Double>.Log,
            UnaryFloatingPoint<Float>.Log,
            UnaryFloatingPoint<Double>.Log);
        
        add("log10",
            UnaryFloatingPoint<Float>.Log10,
            UnaryFloatingPoint<Double>.Log10);
        
        add("log2",
            UnaryFloatingPoint<Float>.Log2,
            UnaryFloatingPoint<Double>.Log2);
        
        add("max",
            Max<Int>.Factory,
            Max<Uint>.Factory,
            Max<Float>.Factory,
            Max<Double>.Factory);
        
        add("min",
            Min<Int>.Factory,
            Min<Uint>.Factory,
            Min<Float>.Factory,
            Min<Double>.Factory);
        
        add("mul",
            Mul<Int>.Factory,
            Mul<Uint>.Factory,
            Mul<Float>.Factory,
            Mul<Double>.Factory);
        
        add("on",
            OnTrue.Factory);
        
        add("onChange",
            OnChange.Factory);
        
        add("onFalse",
            OnFalse.Factory);
        
        add("onlyOne",
            OnlyOne.Factory);
        
        add("onTrue",
            OnTrue.Factory);
        
        add("or",
            BitwiseOr<Int>.Factory,
            BitwiseOr<Uint>.Factory,
            Or.Factory);
        
        add("padLeft",
            Binary.PadLeft,
            Ternary.PadLeft);
        
        add("padRight",
            Binary.PadRight,
            Ternary.PadRight);
        
        add("pow",
            BinaryFloatingPoint<Float>.Pow,
            BinaryFloatingPoint<Double>.Pow);
        
        add("remainder",
            BinaryFloatingPoint<Float>.IEEERemainder,
            BinaryFloatingPoint<Double>.IEEERemainder);
        
        add("remove",
            Ternary.Remove);
        
        add("round",
            BinaryFloatingPoint<Float>.Round,
            BinaryFloatingPoint<Double>.Round,
            UnaryFloatingPoint<Float>.Round,
            UnaryFloatingPoint<Double>.Round);
        
        add("select",
            SelectValue<Bool>.Factory,
            SelectValue<Int>.Factory,
            SelectValue<Uint>.Factory,
            SelectValue<Float>.Factory,
            SelectValue<Double>.Factory,
            SelectValue<String>.Factory,
            SelectValue<Object>.Factory,
            SelectTrigger.Factory);
        
        add("sin",
            UnaryFloatingPoint<Float>.Sin,
            UnaryFloatingPoint<Double>.Sin);
        
        add("sinh",
            UnaryFloatingPoint<Float>.Sinh,
            UnaryFloatingPoint<Double>.Sinh);
        
        add("sqrt",
            UnaryFloatingPoint<Float>.Sqrt,
            UnaryFloatingPoint<Double>.Sqrt);
        
        add("startsWith",
            Binary.StartsWith);
        
        add("substring",
            Ternary.Substring);
        
        add("sum",
            Sum<Int>.Factory(),
            Sum<Uint>.Factory(),
            Sum<Float>.Factory(),
            Sum<Double>.Factory(),
            Sum<String>.Factory(true));
        
        add("tan",
            UnaryFloatingPoint<Float>.Tan,
            UnaryFloatingPoint<Double>.Tan);
        
        add("tanh",
            UnaryFloatingPoint<Float>.Tanh,
            UnaryFloatingPoint<Double>.Tanh);
        
        add("trim",
            Unary.Trim,
            Binary.Trim);
        
        add("trimEnd",
            Unary.TrimEnd,
            Binary.TrimEnd);
        
        add("trimStart",
            Unary.TrimStart,
            Binary.TrimStart);
        
        add("trunc",
            UnaryFloatingPoint<Float>.Truncate,
            UnaryFloatingPoint<Double>.Truncate);
        
        add("xor",
            BitwiseXor<Int>.Factory,
            BitwiseXor<Uint>.Factory,
            Xor.Factory,
            XorTrigger.Factory);
     
        add("zener",
            Zener<Int>.Factory,
            Zener<Uint>.Factory,
            Zener<Float>.Factory,
            Zener<Double>.Factory);
    }
}
