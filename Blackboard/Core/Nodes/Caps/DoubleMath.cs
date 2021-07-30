using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This gets the double mathmatical function value from the parent.</summary>
    sealed public class DoubleMath<T>: Unary<T, T>
        where T : IFloatingPoint<T>, IComparable<T>, new() {

        /// <summary>This is a factory for creating a new Acos instance of this node.</summary>
        /// <remarks>The angle whose cosine is the specified number.</remarks>
        static public readonly IFunction Acos = Factory("Acos", S.Math.Acos);

        /// <summary>This is a factory for creating a new Acosh instance of this node.</summary>
        /// <remarks>The angle whose hyperbolic cosine is the specified number.</remarks>
        static public readonly IFunction Acosh = Factory("Acosh", S.Math.Acosh);

        /// <summary>This is a factory for creating a new Asin instance of this node.</summary>
        /// <remarks>The angle whose sine is the specified number.</remarks>
        static public readonly IFunction Asin = Factory("Asin", S.Math.Asin);

        /// <summary>This is a factory for creating a new Asinh instance of this node.</summary>
        /// <remarks>The angle whose hyperbolic sine is the specified number.</remarks>
        static public readonly IFunction Asinh = Factory("Asinh", S.Math.Asinh);

        /// <summary>This is a factory for creating a new Atan instance of this node.</summary>
        /// <remarks>The angle whose tangent is the specified number.</remarks>
        static public readonly IFunction Atan = Factory("Atan", S.Math.Atan);

        /// <summary>This is a factory for creating a new Atanh instance of this node.</summary>
        /// <remarks>The angle whose hyperbolic tangent is the specified number.</remarks>
        static public readonly IFunction Atanh = Factory("Atanh", S.Math.Atanh);

        /// <summary>This is a factory for creating a new Cbrt instance of this node.</summary>
        /// <remarks>The cube root of the given value.</remarks>
        static public readonly IFunction Cbrt = Factory("Cbrt", S.Math.Cbrt);

        /// <summary>This is a factory for creating a new Ceiling instance of this node.</summary>
        /// <remarks>The smallest integral value that is greater than or equal to the given value.</remarks>
        static public readonly IFunction Ceiling = Factory("Ceiling", S.Math.Ceiling);

        /// <summary>This is a factory for creating a new Cos instance of this node.</summary>
        /// <remarks>The cosine of the specified angle.</remarks>
        static public readonly IFunction Cos = Factory("Cos", S.Math.Cos);

        /// <summary>This is a factory for creating a new Cosh instance of this node.</summary>
        /// <remarks>The hyperbolic cosine of the specified angle.</remarks>
        static public readonly IFunction Cosh = Factory("Cosh", S.Math.Cosh);

        /// <summary>This is a factory for creating a new Exp instance of this node.</summary>
        /// <remarks>'e' raised to the specified power.</remarks>
        static public readonly IFunction Exp = Factory("Exp", S.Math.Exp);

        /// <summary>This is a factory for creating a new Floor instance of this node.</summary>
        /// <remarks>The largest integral value less than or equal to the given value.</remarks>
        static public readonly IFunction Floor = Factory("Floor", S.Math.Floor);

        /// <summary>This is a factory for creating a new Log instance of this node.</summary>
        /// <remarks>The natural (base 'e') logarithm of a specified number.</remarks>
        static public readonly IFunction Log = Factory("Log", S.Math.Log);

        /// <summary>This is a factory for creating a new Log10 instance of this node.</summary>
        /// <remarks>The base 10 logarithm of a specified number.</remarks>
        static public readonly IFunction Log10 = Factory("Log10", S.Math.Log10);

        /// <summary>This is a factory for creating a new Log2 instance of this node.</summary>
        /// <remarks>The base 2 logarithm of a specified number.</remarks>
        static public readonly IFunction Log2 = Factory("Log2", S.Math.Log2);

        /// <summary>This is a factory for creating a new Round instance of this node.</summary>
        /// <remarks>The value to the nearest integral value and rounds midpoint values to the nearest even number.</remarks>
        static public readonly IFunction Round = Factory("Round", S.Math.Round);

        /// <summary>This is a factory for creating a new Sin instance of this node.</summary>
        /// <remarks>The sine of the specified angle.</remarks>
        static public readonly IFunction Sin = Factory("Sin", S.Math.Sin);

        /// <summary>This is a factory for creating a new Sinh instance of this node.</summary>
        /// <remarks>The hyperbolic sine of the specified angle.</remarks>
        static public readonly IFunction Sinh = Factory("Sinh", S.Math.Sinh);

        /// <summary>This is a factory for creating a new Sqrt instance of this node.</summary>
        /// <remarks>The square root of a specified number.</remarks>
        static public readonly IFunction Sqrt = Factory("Sqrt", S.Math.Sqrt);

        /// <summary>This is a factory for creating a new Tan instance of this node.</summary>
        /// <remarks>The tangent of the specified angle.</remarks>
        static public readonly IFunction Tan = Factory("Tan", S.Math.Tan);

        /// <summary>This is a factory for creating a new Tanh instance of this node.</summary>
        /// <remarks>The hyperbolic tangent of the specified angle.</remarks>
        static public readonly IFunction Tanh = Factory("Tanh", S.Math.Tanh);

        /// <summary>This is a factory for creating a new Tan instance of this node.</summary>
        /// <remarks>The integral part of the given number.</remarks>
        static public readonly IFunction Truncate = Factory("Truncate", S.Math.Truncate);

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        /// <param name="funcName">The display name for this function.</param>
        /// <param name="func">The function to perform for this node.</param>
        static public IFunction Factory(string funcName, S.Func<double, double> func) =>
            new Func<IValue<T>>((value) => new DoubleMath<T>(funcName, func, value));

        /// <summary>The name of the function for this mathmatics.</summary>
        private string funcName;

        /// <summary>The function to perform on this node's value.</summary>
        private S.Func<double, double> func;

        /// <summary>Creates a double mathmatical function value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public DoubleMath(string funcName, S.Func<double, double> func, IValue<T> source = null, T value = default) :
            base(source, value) {
            this.funcName = funcName;
            this.func = func;
        }

        /// <summary>The result of the double mathmatical function the parent's value during evaluation.</summary>
        /// <param name="value">The value to ceiling.</param>
        /// <returns>The ceiling value.</returns>
        protected override T OnEval(T value) => value.DoubleMath(this.func);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => this.funcName + base.ToString();
    }
}
