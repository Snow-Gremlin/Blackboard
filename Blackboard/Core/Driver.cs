using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.IO;
using S = System;

namespace Blackboard.Core {

    /// <summary>
    /// The driver will store a blackboard node structure and
    /// perform evaluations/updates of change to that structure's values.
    /// </summary>
    public class Driver {

        /// <summary>The input nodes which have been modified.</summary>
        private List<INode> touched;

        /// <summary>Creates a new driver.</summary>
        /// <param name="log">The optional log to write debug information to during evaluations.</param>
        public Driver(TextWriter log = null) {
            this.Log = log;
            this.touched = new List<INode>();
            this.Global = new Namespace();

            this.addOperators();
            this.addFunctions();
            this.addConstants();
        }

        #region Built-in Functions and Constants...

        /// <summary>This adds all the operators used by the language.</summary>
        private void addOperators() {
            Namespace operators = new();
            this.Global["operators"] = operators;
            void add(string name, params IFunction[] funcs) =>
                operators[name] = new Collection(funcs);

            add("and",
                And.Factory,
                BitwiseAnd<Int>.Factory,
                All.Factory);
            add("cast-trigger");
            add("cast-bool");
            add("cast-int",
                Explicit<Double, Int>.Factory);
            add("cast-double",
                Implicit<Int, Double>.Factory);
            add("cast-string",
                Implicit<Bool, String>.Factory,
                Implicit<Int, String>.Factory,
                Implicit<Double, String>.Factory);
            add("divide",
                Div<Int>.Factory,
                Div<Double>.Factory);
            add("equal",
                Equal<Bool>.Factory,
                Equal<Int>.Factory,
                Equal<Double>.Factory,
                Equal<String>.Factory);
            add("greater",
                GreaterThan<Bool>.Factory,
                GreaterThan<Int>.Factory,
                GreaterThan<Double>.Factory,
                GreaterThan<String>.Factory);
            add("greater-equal",
                GreaterThanOrEqual<Bool>.Factory,
                GreaterThanOrEqual<Int>.Factory,
                GreaterThanOrEqual<Double>.Factory,
                GreaterThanOrEqual<String>.Factory);
            add("invert",
                BitwiseNot<Int>.Factory);
            add("less",
                LessThan<Bool>.Factory,
                LessThan<Int>.Factory,
                LessThan<Double>.Factory,
                LessThan<String>.Factory);
            add("less-equal",
                LessThanOrEqual<Bool>.Factory,
                LessThanOrEqual<Int>.Factory,
                LessThanOrEqual<Double>.Factory,
                LessThanOrEqual<String>.Factory);
            add("logical-and",
                And.Factory,
                All.Factory);
            add("logical-or",
                Or.Factory,
                Any.Factory);
            add("logical-xor",
                Xor.Factory,
                OnlyOne.Factory);
            add("modulo",
                Mod<Int>.Factory,
                Mod<Double>.Factory);
            add("multiply",
                Mul<Int>.Factory,
                Mul<Double>.Factory);
            add("negate",
                Neg<Int>.Factory,
                Neg<Double>.Factory);
            add("not",
                Not.Factory);
            add("not-equal",
                NotEqual<Bool>.Factory,
                NotEqual<Int>.Factory,
                NotEqual<Double>.Factory,
                NotEqual<String>.Factory);
            add("or",
                Or.Factory,
                BitwiseOr<Int>.Factory,
                Any.Factory);
            add("power",
                Power<Int>.Factory,
                Power<Double>.Factory);
            add("remainder",
                Rem<Int>.Factory,
                Rem<Double>.Factory);
            add("shift-left",
                LeftShift<Int>.Factory);
            add("shift-right",
                RightShift<Int>.Factory);
            add("subtract",
                Sub<Int>.Factory,
                Sub<Double>.Factory);
            add("sum",
                Sum<Int>.Factory,
                Sum<Double>.Factory,
                Sum<String>.Factory);
            add("trinary",
                Select<Bool>.Factory,
                Select<Int>.Factory,
                Select<Double>.Factory);
            add("xor",
                Xor.Factory,
                BitwiseXor<Int>.Factory,
                OnlyOne.Factory);
        }

        /// <summary>This adds all global initial methods for Blackboard.</summary>
        private void addFunctions() {
            void add(string name, params IFunction[] funcs) =>
                this.Global[name] = new Collection(funcs);

            add("abs",
                Abs<Int>.Factory,
                Abs<Double>.Factory);
            add("acos",
                DoubleMath<Double>.Acos);
            add("acosh",
                DoubleMath<Double>.Acosh);
            add("all",
                All.Factory);
            add("and",
                And.Factory,
                BitwiseAnd<Int>.Factory);
            add("any",
                Any.Factory);
            add("asin",
                DoubleMath<Double>.Asin);
            add("asinh",
                DoubleMath<Double>.Asinh);
            add("atan",
                Atan2<Double>.Factory,
                DoubleMath<Double>.Atan);
            add("atanh",
                DoubleMath<Double>.Atanh);
            add("average",
                Average.Factory);
            add("cbrt",
                DoubleMath<Double>.Cbrt);
            add("ceiling",
                DoubleMath<Double>.Ceiling);
            add("clamp",
                Clamp<Int>.Factory,
                Clamp<Double>.Factory);
            add("cos",
                DoubleMath<Double>.Cos);
            add("cosh",
                DoubleMath<Double>.Cosh);
            add("exp",
                DoubleMath<Double>.Exp);
            add("floor",
                DoubleMath<Double>.Floor);
            add("implies",
                Implies.Factory);
            add("latch",
                Latch<Bool>.Factory,
                Latch<Int>.Factory,
                Latch<Double>.Factory,
                Latch<String>.Factory);
            add("lerp",
                Lerp<Double>.Factory);
            add("log",
                DoubleMath<Double>.Log,
                Log<Double>.Factory);
            add("log10",
                DoubleMath<Double>.Log10);
            add("log2",
                DoubleMath<Double>.Log2);
            add("max",
                Max<Int>.Factory,
                Max<Double>.Factory);
            add("min",
                Min<Int>.Factory,
                Min<Double>.Factory);
            add("mul",
                Mul<Int>.Factory,
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
                Or.Factory);
            add("round",
                DoubleMath<Double>.Round,
                Round<Double>.Factory);
            add("sin",
                DoubleMath<Double>.Sin);
            add("sinh",
                DoubleMath<Double>.Sinh);
            add("sqrt",
                DoubleMath<Double>.Sqrt);
            add("sum",
                Sum<Int>.Factory,
                Sum<Double>.Factory);
            add("tan",
                DoubleMath<Double>.Tan);
            add("tanh",
                DoubleMath<Double>.Tanh);
            add("trunc",
                DoubleMath<Double>.Truncate);
            add("xor",
                BitwiseXor<Int>.Factory,
                Xor.Factory);
        }

        /// <summary>This adds all the initial constanst for Blackboard.</summary>
        private void addConstants() {
            void add(string name, double value) =>
                this.Global[name] = Literal.Double(value);

            add("e", S.Math.E);
            add("pi", S.Math.PI);
            add("tau", S.Math.Tau);
            add("sqrt2", S.Math.Sqrt(2.0));
        }

        #endregion

        /// <summary>An optional log to keep track of which nodes and what order they are evaluated.</summary>
        public TextWriter Log;

        /// <summary>The base set of named nodes to access the total node structure.</summary>
        public Namespace Global { get; }

        #region Value Setters...

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        /// <returns>True if named input node is found and is the correct type, false otherwise.</returns>
        public bool SetBool(bool value, params string[] names) =>
            this.SetValue(new Bool(value), names);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        /// <returns>True if named input node is found and is the correct type, false otherwise.</returns>
        public bool SetInt(int value, params string[] names) =>
            this.SetValue(new Int(value), names);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        /// <returns>True if named input node is found and is the correct type, false otherwise.</returns>
        public bool SetDouble(double value, params string[] names) =>
            this.SetValue(new Double(value), names);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        /// <returns>True if named input node is found and is the correct type, false otherwise.</returns>
        public bool SetString(string value, params string[] names) =>
            this.SetValue(new String(value), names);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <typeparam name="T">The type of the value to set to the input.</typeparam>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        /// <returns>True if named input node is found and is the correct type, false otherwise.</returns>
        public bool SetValue<T>(T value, params string[] names) where T : IData =>
            this.SetValue<T>(value, names as IEnumerable<string>);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <typeparam name="T">The type of the value to set to the input.</typeparam>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        /// <returns>True if named input node is found and is the correct type, false otherwise.</returns>
        public bool SetValue<T>(T value, IEnumerable<string> names) where T : IData {
            if (this.Global.Find(names) is IValueInput<T> node) {
                this.SetValue(value, node);
                return true;
            }
            return false;
        }

        /// <summary>Sets the value of the given input node.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="input">The input node to set the value of.</param>
        /// <param name="value">The value to set to the given input.</param>
        public void SetValue<T>(T value, IValueInput<T> input) where T : IData {
            if (input.SetValue(value)) this.touched.Add(input);
        }

        #endregion
        #region Provokers...

        /// <summary>This will provoke the node with the given name.</summary>
        /// <param name="names">The name of trigger node to provoke.</param>
        /// <returns>True if a node by that name is found and it was a trigger, false otherwise.</returns>
        public bool Provoke(params string[] names) =>
            this.Provoke(names as IEnumerable<string>);

        /// <summary>This will provoke the node with the given name.</summary>
        /// <param name="names">The name of trigger node to provoke.</param>
        /// <returns>True if a node by that name is found and it was a trigger, false otherwise.</returns>
        public bool Provoke(IEnumerable<string> names) {
            if (this.Global.Find(names) is ITriggerInput node) {
                this.Provoke(node);
                return true;
            }
            return false;
        }

        /// <summary>This will provoke the given trigger node.</summary>
        /// <param name="input">The input trigger node to provoke.</param>
        public void Provoke(ITriggerInput input) {
            input.Provoke();
            this.touched.Add(input);
        }

        #endregion
        #region Value Getters...

        /// <summary>Gets the value of from an named node.</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>
        /// The value from the node or the default value if the node
        /// by that name doesn't exists and the found node is the incorrect type.
        /// </returns>
        public bool GetBool(params string[] names) =>
            this.GetValue<Bool>(names).Value;

        /// <summary>Gets the value of from an named node.</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>
        /// The value from the node or the default value if the node
        /// by that name doesn't exists and the found node is the incorrect type.
        /// </returns>
        public int GetInt(params string[] names) =>
            this.GetValue<Int>(names).Value;

        /// <summary>Gets the value of from an named node.</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>
        /// The value from the node or the default value if the node
        /// by that name doesn't exists and the found node is the incorrect type.
        /// </returns>
        public double GetDouble(params string[] names) =>
            this.GetValue<Double>(names).Value;

        /// <summary>Gets the value of from an named node.</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>
        /// The value from the node or the default value if the node
        /// by that name doesn't exists and the found node is the incorrect type.
        /// </returns>
        public string GetString(params string[] names) =>
            this.GetValue<String>(names).Value;

        /// <summary>Gets the value of from an named node.</summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>
        /// The value from the node or the default value if the node
        /// by that name doesn't exists and the found node is the incorrect type.
        /// </returns>
        public T GetValue<T>(params string[] names) where T : IData =>
            this.GetValue<T>(names as IEnumerable<string>);

        /// <summary>Gets the value of from an named node.</summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>
        /// The value from the node or the default value if the node
        /// by that name doesn't exists and the found node is the incorrect type.
        /// </returns>
        public T GetValue<T>(IEnumerable<string> names) where T : IData =>
            this.Global.Find(names) is IValue<T> node ? node.Value : default;

        #endregion

        /// <summary>This indicates if any changes are pending evaluation.</summary>
        public bool HasPending => this.touched.Count > 0;

        /// <summary>Updates and propogates the changes from the given inputs through the blackboard nodes.</summary>
        public void Evalate() {
            LinkedList<INode> pending = new();
            LinkedList<ITrigger> needsReset = new();
            pending.SortInsertUnique(this.touched);
            this.touched.Clear();

            while (pending.Count > 0) {
                INode node = pending.TakeFirst();
                this.Log?.WriteLine("Eval("+node.Depth+"): "+node);
                pending.SortInsertUnique(node.Eval());
                if (node is ITrigger trigger)
                    needsReset.AddLast(trigger);
            }

            foreach (ITrigger trigger in needsReset)
                trigger.Reset();
        }
    }
}
