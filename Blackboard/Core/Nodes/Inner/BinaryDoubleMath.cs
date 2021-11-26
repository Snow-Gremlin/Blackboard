using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This gets the double mathmatical function value from two parents.</summary>
    sealed public class BinaryDoubleMath<T>: BinaryValue<T, T, T>
        where T : IFloatingPoint<T>, IComparable<T>, new() {

        /// <summary>This is a factory for creating a new Atan2 instance of this node.</summary>
        /// <remarks>
        /// This returns the angle whose tangent is the quotient of two specified numbers
        /// where the first parent is the y value and the second parent is the x vavlue.
        /// </remarks>
        static public readonly IFuncDef Atan2 = Factory("Atan2", S.Math.Atan2);

        /// <summary>This is a factory for creating a new IEEERemainder instance of this node.</summary>
        /// <remarks>
        /// This returns the remainder resulting from the division of a specified number by another
        /// specified number. The first parent is the dividend and the second parent is the divisor.
        /// This is different from a modulo because it uses the IEEE 754 standard remainder algorithm.
        /// </remarks>
        static public readonly IFuncDef IEEERemainder = Factory("IEEERemainder", S.Math.IEEERemainder);

        /// <summary>This is a factory for creating a new Log instance of this node.</summary>
        /// <remarks>This returns the log of the first parent with the base of the second parent.</remarks>
        static public readonly IFuncDef Log = Factory("Log", S.Math.Log);

        /// <summary>This is a factory for creating a new Pow instance of this node.</summary>
        /// <remarks>This returns the first parent raised to the power of the second parent.</remarks>
        static public readonly IFuncDef Pow = Factory("Pow", S.Math.Pow);

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        /// <param name="funcName">The display name for this function.</param>
        /// <param name="func">The function to perform for this node.</param>
        static public IFuncDef Factory(string funcName, S.Func<double, double, double> func) =>
            new Function<IValueParent<T>, IValueParent<T>, BinaryDoubleMath<T>>((value1, value2) =>
                new BinaryDoubleMath<T>(funcName, func, value1, value2));

        /// <summary>The name of the function for this mathmatics.</summary>
        private readonly string funcName;

        /// <summary>The function to perform on this node's value.</summary>
        private readonly S.Func<double, double, double> func;

        /// <summary>Creates a double mathmatical function value node.</summary>
        /// <param name="funcName">The name of the function to perform.</param>
        /// <param name="func">This is the function to apply to the parents.</param>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public BinaryDoubleMath(string funcName, S.Func<double, double, double> func,
            IValueParent<T> source1 = null, IValueParent<T> source2 = null, T value = default) :
            base(source1, source2, value) {
            this.funcName = funcName;
            this.func = func;
            this.UpdateValue();
        }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => this.funcName;

        /// <summary>The result of the double mathmatical function the parents' value during evaluation.</summary>
        /// <param name="value1">The first value to evaluate.</param>
        /// <param name="value2">The second value to evaluate.</param>
        /// <returns>The ceiling value.</returns>
        protected override T OnEval(T value1, T value2) =>
            this.func is null ? value1 : value1.DoubleMath(value2, this.func);
    }
}
