using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Unary nodes for floating point mathematics.</summary>
    /// <typeparam name="T">The type of floating point to perform the math on.</typeparam>
    static public class UnaryFloatingPoint<T> where T : IFloatingPoint<T>, IEquatable<T> {

        /// <summary>This is a factory for creating a new IsNaN instance of this node.</summary>
        /// <remarks>Determines if the double is not a number, such as when something is divided by zero.</remarks>
        static public readonly IFuncDef IsNaN = UnaryFuncs<T, Bool>.Factory(nameof(IsNaN), v => v.IsNaN());

        /// <summary>This is a factory for creating a new IsFinite instance of this node.</summary>
        /// <remarks>Determines if the value is not infinite.</remarks>
        static public readonly IFuncDef IsFinite = UnaryFuncs<T, Bool>.Factory(nameof(IsFinite), v => v.IsFinite());

        /// <summary>This is a factory for creating a new IsInfinity instance of this node.</summary>
        /// <remarks>Determines if the value is positive or negative infinity.</remarks>
        static public readonly IFuncDef IsInfinity = UnaryFuncs<T, Bool>.Factory(nameof(IsInfinity), v => v.IsInfinity());

        /// <summary>This is a factory for creating a new IsPositiveInfinity instance of this node.</summary>
        /// <remarks>Determines if the value is positive infinity.</remarks>
        static public readonly IFuncDef IsPositiveInfinity = UnaryFuncs<T, Bool>.
            Factory(nameof(IsPositiveInfinity), v => v.IsPositiveInfinity());

        /// <summary>This is a factory for creating a new IsNegativeInfinity instance of this node.</summary>
        /// <remarks>Determines if the value is negative infinity.</remarks>
        static public readonly IFuncDef IsNegativeInfinity = UnaryFuncs<T, Bool>.
            Factory(nameof(IsNegativeInfinity), v => v.IsNegativeInfinity());

        /// <summary>This is a factory for creating a new IsNormal instance of this node.</summary>
        /// <remarks>Determines if the value is in the normal range of doubles with full precision.</remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Subnormal_number"/>
        static public readonly IFuncDef IsNormal = UnaryFuncs<T, Bool>.Factory(nameof(IsNormal), v => v.IsNormal());

        /// <summary>This is a factory for creating a new IsSubnormal instance of this node.</summary>
        /// <remarks>Determines if the value is not in the normal range of double so have reduced precision.</remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Subnormal_number"/>
        static public readonly IFuncDef IsSubnormal = UnaryFuncs<T, Bool>.Factory(nameof(IsSubnormal), v => v.IsSubnormal());

        /// <summary>This is a factory for creating a new Acos instance of this node.</summary>
        /// <remarks>The angle whose cosine is the specified number.</remarks>
        static public readonly IFuncDef Acos = factoryDoubleMath(nameof(Acos), S.Math.Acos);

        /// <summary>This is a factory for creating a new Acosh instance of this node.</summary>
        /// <remarks>The angle whose hyperbolic cosine is the specified number.</remarks>
        static public readonly IFuncDef Acosh = factoryDoubleMath(nameof(Acosh), S.Math.Acosh);

        /// <summary>This is a factory for creating a new Asin instance of this node.</summary>
        /// <remarks>The angle whose sine is the specified number.</remarks>
        static public readonly IFuncDef Asin = factoryDoubleMath(nameof(Asin), S.Math.Asin);

        /// <summary>This is a factory for creating a new Asinh instance of this node.</summary>
        /// <remarks>The angle whose hyperbolic sine is the specified number.</remarks>
        static public readonly IFuncDef Asinh = factoryDoubleMath(nameof(Asinh), S.Math.Asinh);

        /// <summary>This is a factory for creating a new Atan instance of this node.</summary>
        /// <remarks>The angle whose tangent is the specified number.</remarks>
        static public readonly IFuncDef Atan = factoryDoubleMath(nameof(Atan), S.Math.Atan);

        /// <summary>This is a factory for creating a new Atanh instance of this node.</summary>
        /// <remarks>The angle whose hyperbolic tangent is the specified number.</remarks>
        static public readonly IFuncDef Atanh = factoryDoubleMath(nameof(Atanh), S.Math.Atanh);

        /// <summary>This is a factory for creating a new Cbrt instance of this node.</summary>
        /// <remarks>The cube root of the given value.</remarks>
        static public readonly IFuncDef Cbrt = factoryDoubleMath(nameof(Cbrt), S.Math.Cbrt);

        /// <summary>This is a factory for creating a new Ceiling instance of this node.</summary>
        /// <remarks>The smallest integral value that is greater than or equal to the given value.</remarks>
        static public readonly IFuncDef Ceiling = factoryDoubleMath(nameof(Ceiling), S.Math.Ceiling);

        /// <summary>This is a factory for creating a new Cos instance of this node.</summary>
        /// <remarks>The cosine of the specified angle.</remarks>
        static public readonly IFuncDef Cos = factoryDoubleMath(nameof(Cos), S.Math.Cos);

        /// <summary>This is a factory for creating a new Cosh instance of this node.</summary>
        /// <remarks>The hyperbolic cosine of the specified angle.</remarks>
        static public readonly IFuncDef Cosh = factoryDoubleMath(nameof(Cosh), S.Math.Cosh);

        /// <summary>This is a factory for creating a new Exp instance of this node.</summary>
        /// <remarks>'e' raised to the specified power.</remarks>
        static public readonly IFuncDef Exp = factoryDoubleMath(nameof(Exp), S.Math.Exp);

        /// <summary>This is a factory for creating a new Floor instance of this node.</summary>
        /// <remarks>The largest integral value less than or equal to the given value.</remarks>
        static public readonly IFuncDef Floor = factoryDoubleMath(nameof(Floor), S.Math.Floor);

        /// <summary>This is a factory for creating a new Log instance of this node.</summary>
        /// <remarks>The natural (base 'e') logarithm of a specified number.</remarks>
        static public readonly IFuncDef Log = factoryDoubleMath(nameof(Log), S.Math.Log);

        /// <summary>This is a factory for creating a new Log10 instance of this node.</summary>
        /// <remarks>The base 10 logarithm of a specified number.</remarks>
        static public readonly IFuncDef Log10 = factoryDoubleMath(nameof(Log10), S.Math.Log10);

        /// <summary>This is a factory for creating a new Log2 instance of this node.</summary>
        /// <remarks>The base 2 logarithm of a specified number.</remarks>
        static public readonly IFuncDef Log2 = factoryDoubleMath(nameof(Log2), S.Math.Log2);

        /// <summary>This is a factory for creating a new Round instance of this node.</summary>
        /// <remarks>
        /// The value to the nearest integral value and
        /// rounds midpoint values to the nearest even number.
        /// </remarks>
        static public readonly IFuncDef Round =factoryDoubleMath(nameof(Round), S.Math.Round);

        /// <summary>This is a factory for creating a new Sin instance of this node.</summary>
        /// <remarks>The sine of the specified angle.</remarks>
        static public readonly IFuncDef Sin = factoryDoubleMath(nameof(Sin), S.Math.Sin);

        /// <summary>This is a factory for creating a new Sinh instance of this node.</summary>
        /// <remarks>The hyperbolic sine of the specified angle.</remarks>
        static public readonly IFuncDef Sinh = factoryDoubleMath(nameof(Sinh), S.Math.Sinh);

        /// <summary>This is a factory for creating a new Sqrt instance of this node.</summary>
        /// <remarks>The square root of a specified number.</remarks>
        static public readonly IFuncDef Sqrt = factoryDoubleMath(nameof(Sqrt), S.Math.Sqrt);

        /// <summary>This is a factory for creating a new Tan instance of this node.</summary>
        /// <remarks>The tangent of the specified angle.</remarks>
        static public readonly IFuncDef Tan = factoryDoubleMath(nameof(Tan), S.Math.Tan);

        /// <summary>This is a factory for creating a new Tanh instance of this node.</summary>
        /// <remarks>The hyperbolic tangent of the specified angle.</remarks>
        static public readonly IFuncDef Tanh = factoryDoubleMath(nameof(Tanh), S.Math.Tanh);

        /// <summary>This is a factory for creating a new Truncate instance of this node.</summary>
        /// <remarks>The integral part of the given number.</remarks>
        static public readonly IFuncDef Truncate = factoryDoubleMath(nameof(Truncate), S.Math.Truncate);

        /// <summary>Creates a new factory to performs the given function which returns a double.</summary>
        /// <param name="name">The name of the double math function for the nodes name.</param>
        /// <param name="func">The function to perform in the node.</param>
        /// <returns>The new factory for the node.</returns>
        static private IFuncDef factoryDoubleMath(string name, S.Func<double, double> func) =>
            UnaryFuncs<T, T>.Factory(name, v => v.DoubleMath(func));
    }

    /// <summary>Unary nodes for signed value mathematics.</summary>
    /// <typeparam name="T">The type of signed number to perform the methods on.</typeparam>
    static public class UnarySigned<T> where T : ISigned<T>, IEquatable<T> {

        /// <summary>This is a factory for creating a new IsNegative instance of this node.</summary>
        /// <remarks>Determines if the value is negative.</remarks>
        static public readonly IFuncDef IsNegative = UnaryFuncs<T, Bool>.Factory(nameof(IsNegative), v => v.IsNegative());
    }

    /// <summary>Unary nodes for nullable values.</summary>
    /// <typeparam name="T">The type of nullable object to perform the methods on.</typeparam>
    static public class UnaryNullable<T> where T: INullable, IEquatable<T> {

        /// <summary>This is a factory for creating a new IsNull instance of this node.</summary>
        /// <remarks>Determines if the object is null or not.</remarks>
        static public readonly IFuncDef IsNull = UnaryFuncs<T, Bool>.Factory(nameof(IsNull), v => v.IsNull());
    }

    /// <summary>Unary nodes for specific hard-coded data types.</summary>
    static public class Unary {

        /// <summary>This is a factory for creating a new IsEmpty instance of this node.</summary>
        /// <remarks>Determines if the string is empty.</remarks>
        static public readonly IFuncDef IsEmpty = UnaryFuncs<String, Bool>.
            Factory(nameof(IsEmpty), v => new Bool(string.IsNullOrEmpty(v.Value)));

        /// <summary>This is a factory for creating a new Length instance of this node.</summary>
        /// <remarks>Determines if the length of the string.</remarks>
        static public readonly IFuncDef Length = UnaryFuncs<String, Int>.
            Factory(nameof(Length), v => new Int(v.Value?.Length ?? 0 ));

        /// <summary>This is a factory for creating a new Trim instance of this node.</summary>
        /// <remarks>Trims the given string of whitespace.</remarks>
        static public readonly IFuncDef Trim = UnaryFuncs<String, String>.
            Factory(nameof(Trim), v => new String(v.Value?.Trim()));

        /// <summary>This is a factory for creating a new TrimStart instance of this node.</summary>
        /// <remarks>Trims the start of the given string of whitespace.</remarks>
        static public readonly IFuncDef TrimStart = UnaryFuncs<String, String>.
            Factory(nameof(TrimStart), v => new String(v.Value?.TrimStart()));

        /// <summary>This is a factory for creating a new TrimEnd instance of this node.</summary>
        /// <remarks>Trims the end of the given string of whitespace.</remarks>
        static public readonly IFuncDef TrimEnd = UnaryFuncs<String, String>.
            Factory(nameof(TrimEnd), v => new String(v.Value?.TrimEnd()));
    }

    /// <summary>This gets the mathematical function for performing some function on the value from the parent.</summary>
    /// <remarks>
    /// This uses a little more computation time and more memory that hard coded nodes,
    /// therefor this should be used to perform less commonly used nodes.
    /// </remarks>
    sealed public class UnaryFuncs<T1, TResult> : UnaryValue<T1, TResult>
        where T1 : IEquatable<T1>
        where TResult : IEquatable<TResult> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        /// <param name="funcName">The display name for this function.</param>
        /// <param name="func">The function to perform for this node.</param>
        static public IFuncDef Factory(string funcName, S.Func<T1, TResult> func) =>
            new Function<IValueParent<T1>, UnaryFuncs<T1, TResult>>((value) =>
                new UnaryFuncs<T1, TResult>(funcName, func, value));

        /// <summary>The name of the function for this mathematics.</summary>
        private readonly string funcName;

        /// <summary>The function to perform on this node's value.</summary>
        private readonly S.Func<T1, TResult> func;

        /// <summary>Creates a double mathematical function value node.</summary>
        /// <param name="funcName">The name of the function to perform.</param>
        /// <param name="func">This is the function to apply to the parent.</param>
        /// <param name="source">This is the single parent for the source value.</param>
        public UnaryFuncs(string funcName, S.Func<T1, TResult> func, IValueParent<T1> source = null) :
            base(source) {
            this.funcName = funcName;
            this.func = func;
        }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new UnaryFuncs<T1, TResult>(this.funcName, this.func);

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => this.funcName;

        /// <summary>The result of the double mathematical function the parent's value during evaluation.</summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>The new data with the double value.</returns>
        protected override TResult OnEval(T1 value) => this.func is null ? default : this.func(value);
    }
}
