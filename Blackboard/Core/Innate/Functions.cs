using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;

namespace Blackboard.Core.Innate;

/// <summary>Group of all initial methods for Blackboard.</summary>
static internal class Functions {

    /// <summary>This adds all global initial methods for Blackboard.</summary>
    /// <param name="global">The global namespace for the slate.</param>
    static public void Add(Namespace global) {
        void add(string name, params IFuncDef[] defs) =>
            global[name] = new FuncGroup(defs);

        add("abs",
            Abs<Int>.Factory,
            Abs<Double>.Factory,
            Abs<Float>.Factory);
        
        add("acos",
            UnaryFloatingPoint<Double>.Acos,
            UnaryFloatingPoint<Float>.Acos);
        
        add("acosh",
            UnaryFloatingPoint<Double>.Acosh,
            UnaryFloatingPoint<Float>.Acosh);
        
        add("all",
            All.Factory);
        
        add("and",
            And.Factory,
            BitwiseAnd<Int>.Factory);
        
        add("any",
            Any.Factory);
        
        add("asin",
            UnaryFloatingPoint<Double>.Asin,
            UnaryFloatingPoint<Float>.Asin);
        
        add("asinh",
            UnaryFloatingPoint<Double>.Asinh,
            UnaryFloatingPoint<Float>.Asinh);
        
        add("atan",
            BinaryFloatingPoint<Double>.Atan2,
            UnaryFloatingPoint<Double>.Atan,
            BinaryFloatingPoint<Float>.Atan2,
            UnaryFloatingPoint<Float>.Atan);
        
        add("atanh",
            UnaryFloatingPoint<Double>.Atanh,
            UnaryFloatingPoint<Float>.Atanh);
        
        add("average",
            Average.Factory);
        
        add("cbrt",
            UnaryFloatingPoint<Double>.Cbrt,
            UnaryFloatingPoint<Float>.Cbrt);
        
        add("ceiling",
            UnaryFloatingPoint<Double>.Ceiling,
            UnaryFloatingPoint<Float>.Ceiling);
        
        add("clamp",
            TernaryComparable<Int>.Clamp,
            TernaryComparable<Uint>.Clamp,
            TernaryComparable<Double>.Clamp,
            TernaryComparable<Float>.Clamp);
        
        add("contains",
            Binary.Contains);
        
        add("cos",
            UnaryFloatingPoint<Double>.Cos,
            UnaryFloatingPoint<Float>.Cos);
        
        add("cosh",
            UnaryFloatingPoint<Double>.Cosh,
            UnaryFloatingPoint<Float>.Cosh);
        
        add("endsWith",
            Binary.EndsWith);
        
        add("epsilon",
            EpsilonEqual<Double>.Factory,
            EpsilonEqual<Float>.Factory);
        
        add("exp",
            UnaryFloatingPoint<Double>.Exp,
            UnaryFloatingPoint<Float>.Exp);
        
        add("floor",
            UnaryFloatingPoint<Double>.Floor,
            UnaryFloatingPoint<Float>.Floor);
        
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
            TernaryComparable<Double>.InRange,
            TernaryComparable<Float>.InRange);
        
        add("insert",
            Ternary.Insert);
        
        add("isEmpty",
            Unary.IsEmpty);
        
        add("isFinite",
            UnaryFloatingPoint<Double>.IsFinite,
            UnaryFloatingPoint<Float>.IsFinite);
        
        add("isInf",
            UnaryFloatingPoint<Double>.IsInfinity,
            UnaryFloatingPoint<Float>.IsInfinity);
        
        add("isNaN",
            UnaryFloatingPoint<Double>.IsNaN,
            UnaryFloatingPoint<Float>.IsNaN);
        
        add("isNeg",
            UnarySigned<Int>.IsNegative,
            UnarySigned<Double>.IsNegative,
            UnarySigned<Float>.IsNegative);
        
        add("isNegInf",
            UnaryFloatingPoint<Double>.IsNegativeInfinity,
            UnaryFloatingPoint<Float>.IsNegativeInfinity);
        
        add("isNormal",
            UnaryFloatingPoint<Double>.IsNormal,
            UnaryFloatingPoint<Float>.IsNormal);
        
        add("isNull",
            UnaryNullable<Object>.IsNull);
        
        add("isPosInf",
            UnaryFloatingPoint<Double>.IsPositiveInfinity,
            UnaryFloatingPoint<Float>.IsPositiveInfinity);
        
        add("isSubnormal",
            UnaryFloatingPoint<Double>.IsSubnormal,
            UnaryFloatingPoint<Float>.IsSubnormal);
        
        add("latch",
            Latch<Bool>.Factory,
            Latch<Int>.Factory,
            Latch<Uint>.Factory,
            Latch<Double>.Factory,
            Latch<Float>.Factory,
            Latch<String>.Factory,
            Latch<Object>.Factory);
        
        add("lerp",
            Lerp<Double>.Factory,
            Lerp<Float>.Factory);
        
        add("length",
            Unary.Length);
        
        add("log",
            BinaryFloatingPoint<Double>.Log,
            BinaryFloatingPoint<Float>.Log,
            UnaryFloatingPoint<Double>.Log,
            UnaryFloatingPoint<Float>.Log);
        
        add("log10",
            UnaryFloatingPoint<Double>.Log10,
            UnaryFloatingPoint<Float>.Log10);
        
        add("log2",
            UnaryFloatingPoint<Double>.Log2,
            UnaryFloatingPoint<Float>.Log2);
        
        add("max",
            Max<Int>.Factory,
            Max<Uint>.Factory,
            Max<Double>.Factory,
            Max<Float>.Factory);
        
        add("min",
            Min<Int>.Factory,
            Min<Uint>.Factory,
            Min<Double>.Factory,
            Min<Float>.Factory);
        
        add("mul",
            Mul<Int>.Factory,
            Mul<Uint>.Factory,
            Mul<Double>.Factory,
            Mul<Float>.Factory);
        
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
            BinaryFloatingPoint<Double>.Pow,
            BinaryFloatingPoint<Float>.Pow);
        
        add("remainder",
            BinaryFloatingPoint<Double>.IEEERemainder,
            BinaryFloatingPoint<Float>.IEEERemainder);
        
        add("remove",
            Ternary.Remove);
        
        add("round",
            BinaryFloatingPoint<Double>.Round,
            BinaryFloatingPoint<Float>.Round,
            UnaryFloatingPoint<Double>.Round,
            UnaryFloatingPoint<Float>.Round);
        
        add("select",
            SelectValue<Bool>.Factory,
            SelectValue<Int>.Factory,
            SelectValue<Uint>.Factory,
            SelectValue<Double>.Factory,
            SelectValue<Float>.Factory,
            SelectValue<String>.Factory,
            SelectValue<Object>.Factory,
            SelectTrigger.Factory);
        
        add("sin",
            UnaryFloatingPoint<Double>.Sin,
            UnaryFloatingPoint<Float>.Sin);
        
        add("sinh",
            UnaryFloatingPoint<Double>.Sinh,
            UnaryFloatingPoint<Float>.Sinh);
        
        add("sqrt",
            UnaryFloatingPoint<Double>.Sqrt,
            UnaryFloatingPoint<Float>.Sqrt);
        
        add("startsWith",
            Binary.StartsWith);
        
        add("substring",
            Ternary.Substring);
        
        add("sum",
            Sum<Int>.Factory(),
            Sum<Uint>.Factory(),
            Sum<Double>.Factory(),
            Sum<Float>.Factory(),
            Sum<String>.Factory(true));
        
        add("tan",
            UnaryFloatingPoint<Double>.Tan,
            UnaryFloatingPoint<Float>.Tan);
        
        add("tanh",
            UnaryFloatingPoint<Double>.Tanh,
            UnaryFloatingPoint<Float>.Tanh);
        
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
            UnaryFloatingPoint<Double>.Truncate,
            UnaryFloatingPoint<Float>.Truncate);
        
        add("xor",
            BitwiseXor<Int>.Factory,
            BitwiseXor<Uint>.Factory,
            Xor.Factory,
            XorTrigger.Factory);
     
        add("zener",
            Zener<Int>.Factory,
            Zener<Uint>.Factory,
            Zener<Double>.Factory,
            Zener<Float>.Factory);
    }
}
