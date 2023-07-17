using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;

namespace Blackboard.Core;

/// <summary>Group of all operators for Blackboard.</summary>
static internal class Operators {

    /// <summary>The namespace for all the operators.</summary>
    public const string Namespace = "$operators";

    /// <summary>This adds all the operators used by the language.</summary>
    /// <param name="global">The global namespace for the slate.</param>
    static public void Add(Namespace global) {
        Namespace operators = new();
        global[Namespace] = operators;
        void add(string name, params IFuncDef[] defs) =>
            operators[name] = new FuncGroup(defs);

        add("and",
            And.Factory,
            BitwiseAnd<Int>.Factory,
            BitwiseAnd<Uint>.Factory,
            All.Factory);
        
        add("castTrigger",
            BoolAsTrigger.Factory);
        
        add("castBool",
            Explicit<Object, Bool>.Factory);
        
        add("castInt",
            Explicit<Uint,   Int>.Factory,
            Explicit<Float,  Int>.Factory,
            Explicit<Double, Int>.Factory,
            Explicit<Object, Int>.Factory);
        
        add("castFloat",
            Implicit<Int,    Float>.Factory,
            Implicit<Uint,   Float>.Factory,
            Explicit<Double, Float>.Factory,
            Explicit<Object, Float>.Factory);
        
        add("castDouble",
            Implicit<Int,    Double>.Factory,
            Implicit<Uint,   Double>.Factory,
            Implicit<Float,  Double>.Factory,
            Explicit<Object, Double>.Factory);
        
        add("castObject",
            Implicit<Bool,   Object>.Factory,
            Implicit<Int,    Object>.Factory,
            Implicit<Uint,   Object>.Factory,
            Implicit<Float,  Object>.Factory,
            Implicit<Double, Object>.Factory,
            Implicit<String, Object>.Factory);
        
        add("castString",
            Implicit<Bool,   String>.Factory,
            Implicit<Int,    String>.Factory,
            Implicit<Uint,   String>.Factory,
            Implicit<Float,  String>.Factory,
            Implicit<Double, String>.Factory,
            Implicit<Object, String>.Factory);
        
        add("castUint",
            Explicit<Int,    Uint>.Factory,
            Explicit<Float,  Uint>.Factory,
            Explicit<Double, Uint>.Factory,
            Explicit<Object, Uint>.Factory);
        
        add("divide",
            Div<Int>.Factory,
            Div<Uint>.Factory,
            Div<Float>.Factory,
            Div<Double>.Factory);
        
        add("equal",
            Equal<Bool>.Factory,
            Equal<Int>.Factory,
            Equal<Uint>.Factory,
            Equal<Float>.Factory,
            Equal<Double>.Factory,
            Equal<Object>.Factory,
            Equal<String>.Factory);
        
        add("greater",
            GreaterThan<Int>.Factory,
            GreaterThan<Uint>.Factory,
            GreaterThan<Float>.Factory,
            GreaterThan<Double>.Factory,
            GreaterThan<String>.Factory);
        
        add("greaterEqual",
            GreaterThanOrEqual<Int>.Factory,
            GreaterThanOrEqual<Uint>.Factory,
            GreaterThanOrEqual<Float>.Factory,
            GreaterThanOrEqual<Double>.Factory,
            GreaterThanOrEqual<String>.Factory);
        
        add("invert",
            BitwiseNot<Int>.Factory,
            BitwiseNot<Uint>.Factory);
        
        add("less",
            LessThan<Int>.Factory,
            LessThan<Uint>.Factory,
            LessThan<Float>.Factory,
            LessThan<Double>.Factory,
            LessThan<String>.Factory);
        
        add("lessEqual",
            LessThanOrEqual<Int>.Factory,
            LessThanOrEqual<Uint>.Factory,
            LessThanOrEqual<Float>.Factory,
            LessThanOrEqual<Double>.Factory,
            LessThanOrEqual<String>.Factory);
        
        add("logicalAnd",
            And.Factory,
            All.Factory);
        
        add("logicalOr",
            Or.Factory,
            Any.Factory);
        
        add("logicalXor",
            Xor.Factory,
            XorTrigger.Factory);
        
        add("modulo",
            Mod<Int>.Factory,
            Mod<Uint>.Factory,
            Mod<Float>.Factory,
            Mod<Double>.Factory);
        
        add("multiply",
            Mul<Int>.Factory,
            Mul<Uint>.Factory,
            Mul<Float>.Factory,
            Mul<Double>.Factory);
        
        add("negate",
            Neg<Int>.Factory,
            Neg<Float>.Factory,
            Neg<Double>.Factory);
        
        add("not",
            Not.Factory);
        
        add("notEqual",
            NotEqual<Bool>.Factory,
            NotEqual<Int>.Factory,
            NotEqual<Uint>.Factory,
            NotEqual<Float>.Factory,
            NotEqual<Double>.Factory,
            NotEqual<Object>.Factory,
            NotEqual<String>.Factory);
        
        add("or",
            Or.Factory,
            BitwiseOr<Int>.Factory,
            BitwiseOr<Uint>.Factory,
            Any.Factory);
        
        add("power",
            BinaryFloatingPoint<Float>.Pow,
            BinaryFloatingPoint<Double>.Pow);
        
        add("shiftLeft",
            LeftShift<Int>.Factory,
            LeftShift<Uint>.Factory);
        
        add("shiftRight",
            RightShift<Int>.Factory,
            RightShift<Uint>.Factory);
        
        add("subtract",
            Sub<Int>.Factory,
            Sub<Uint>.Factory,
            Sub<Float>.Factory,
            Sub<Double>.Factory);
        
        add("sum",
            Sum<Int>.Factory(),
            Sum<Uint>.Factory(),
            Sum<Float>.Factory(),
            Sum<Double>.Factory(),
            Sum<String>.Factory(true));
        
        add("ternary",
            SelectValue<Bool>.Factory,
            SelectValue<Int>.Factory,
            SelectValue<Uint>.Factory,
            SelectValue<Float>.Factory,
            SelectValue<Double>.Factory,
            SelectValue<Object>.Factory,
            SelectValue<String>.Factory,
            SelectTrigger.Factory);

        add("xor",
            Xor.Factory,
            BitwiseXor<Int>.Factory,
            BitwiseXor<Uint>.Factory,
            XorTrigger.Factory);
    }
}
