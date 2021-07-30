using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes;
using Blackboard.Core.Functions;
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

        /// <summary>This adds all the operators used by the language.</summary>
        private void addOperators() {
            Namespace operators = new();
            this.Global["operators"] = operators;
            void addOp(string name, params IFunction[] funcs) =>
                operators[name] = new Collection(funcs);

            addOp("and",
                And.Factory,
                BitwiseAnd<Int>.Factory,
                All.Factory);
            addOp("cast-trigger");
            addOp("cast-bool");
            addOp("cast-int",
                Explicit<Double, Int>.Factory);
            addOp("cast-double",
                Implicit<Int, Double>.Factory);
            addOp("cast-string",
                Implicit<Bool, String>.Factory,
                Implicit<Int, String>.Factory,
                Implicit<Double, String>.Factory);
            addOp("divide",
                Div<Int>.Factory,
                Div<Double>.Factory);
            addOp("equal",
                Equal<Bool>.Factory,
                Equal<Int>.Factory,
                Equal<Double>.Factory,
                Equal<String>.Factory);
            addOp("greater",
                GreaterThan<Bool>.Factory,
                GreaterThan<Int>.Factory,
                GreaterThan<Double>.Factory,
                GreaterThan<String>.Factory);
            addOp("greater-equal",
                GreaterThanOrEqual<Bool>.Factory,
                GreaterThanOrEqual<Int>.Factory,
                GreaterThanOrEqual<Double>.Factory,
                GreaterThanOrEqual<String>.Factory);
            addOp("invert",
                BitwiseNot<Int>.Factory);
            addOp("less",
                LessThan<Bool>.Factory,
                LessThan<Int>.Factory,
                LessThan<Double>.Factory,
                LessThan<String>.Factory);
            addOp("less-equal",
                LessThanOrEqual<Bool>.Factory,
                LessThanOrEqual<Int>.Factory,
                LessThanOrEqual<Double>.Factory,
                LessThanOrEqual<String>.Factory);
            addOp("logical-and",
                And.Factory,
                All.Factory);
            addOp("logical-or",
                Or.Factory,
                Any.Factory);
            addOp("logical-xor",
                Xor.Factory,
                OnlyOne.Factory);
            addOp("modulo",
                Mod<Int>.Factory,
                Mod<Double>.Factory);
            addOp("multiply",
                Mul<Int>.Factory,
                Mul<Double>.Factory);
            addOp("negate",
                Neg<Int>.Factory,
                Neg<Double>.Factory);
            addOp("not",
                Not.Factory);
            addOp("not-equal",
                NotEqual<Bool>.Factory,
                NotEqual<Int>.Factory,
                NotEqual<Double>.Factory,
                NotEqual<String>.Factory);
            addOp("or",
                Or.Factory,
                BitwiseOr<Int>.Factory,
                Any.Factory);
            addOp("power",
                Power<Int>.Factory,
                Power<Double>.Factory);
            addOp("remainder",
                Rem<Int>.Factory,
                Rem<Double>.Factory);
            addOp("shift-left",
                LeftShift<Int>.Factory);
            addOp("shift-right",
                RightShift<Int>.Factory);
            addOp("subtract",
                Sub<Int>.Factory,
                Sub<Double>.Factory);
            addOp("sum",
                Sum<Int>.Factory,
                Sum<Double>.Factory,
                Sum<String>.Factory);
            addOp("trinary",
                Select<Bool>.Factory,
                Select<Int>.Factory,
                Select<Double>.Factory);
            addOp("xor",
                Xor.Factory,
                BitwiseXor<Int>.Factory,
                OnlyOne.Factory);
        }

        /// <summary>This adds all global initial methods for Blackboard.</summary>
        private void addFunctions() {
            void addFunc(string name, params IFunction[] funcs) =>
                this.Global[name] = new Collection(funcs);

            addFunc("abs",
                Abs<Int>.Factory,
                Abs<Double>.Factory);
            addFunc("acos",
                DoubleMath<Double>.Acos);
            addFunc("acosh",
                DoubleMath<Double>.Acosh);
            addFunc("all",
                All.Factory);
            addFunc("and",
                And.Factory,
                BitwiseAnd<Int>.Factory);
            addFunc("any",
                Any.Factory);
            addFunc("asin",
                DoubleMath<Double>.Asin);
            addFunc("asinh",
                DoubleMath<Double>.Asinh);
            addFunc("atan",
                Atan2<Double>.Factory,
                DoubleMath<Double>.Atan);
            addFunc("atanh",
                DoubleMath<Double>.Atanh);
            addFunc("average",
                Average.Factory);
            addFunc("cbrt",
                DoubleMath<Double>.Cbrt);
            addFunc("ceiling",
                DoubleMath<Double>.Ceiling);
            addFunc("clamp",
                Clamp<Int>.Factory,
                Clamp<Double>.Factory);
            addFunc("cos",
                DoubleMath<Double>.Cos);
            addFunc("cosh",
                DoubleMath<Double>.Cosh);
            addFunc("exp",
                DoubleMath<Double>.Exp);
            addFunc("floor",
                DoubleMath<Double>.Floor);
            addFunc("implies",
                Implies.Factory);
            addFunc("latch",
                Latch<Bool>.Factory,
                Latch<Int>.Factory,
                Latch<Double>.Factory,
                Latch<String>.Factory);
            addFunc("lerp",
                Lerp<Double>.Factory);
            addFunc("log",
                DoubleMath<Double>.Log,
                Log<Double>.Factory);
            addFunc("log10",
                DoubleMath<Double>.Log10);
            addFunc("log2",
                DoubleMath<Double>.Log2);
            addFunc("max",
                Max<Int>.Factory,
                Max<Double>.Factory);
            addFunc("min",
                Min<Int>.Factory,
                Min<Double>.Factory);
            addFunc("mul",
                Mul<Int>.Factory,
                Mul<Double>.Factory);
            addFunc("on",
                OnTrue.Factory);
            addFunc("onChange",
                OnChange.Factory);
            addFunc("onFalse",
                OnFalse.Factory);
            addFunc("onlyOne",
                OnlyOne.Factory);
            addFunc("onTrue",
                OnTrue.Factory);
            addFunc("or",
                BitwiseOr<Int>.Factory,
                Or.Factory);
            addFunc("round",
                DoubleMath<Double>.Round,
                Round<Double>.Factory);
            addFunc("sin",
                DoubleMath<Double>.Sin);
            addFunc("sinh",
                DoubleMath<Double>.Sinh);
            addFunc("sqrt",
                DoubleMath<Double>.Sqrt);
            addFunc("sum",
                Sum<Int>.Factory,
                Sum<Double>.Factory);
            addFunc("tan",
                DoubleMath<Double>.Tan);
            addFunc("tanh",
                DoubleMath<Double>.Tanh);
            addFunc("trunc",
                DoubleMath<Double>.Truncate);
            addFunc("xor",
                BitwiseXor<Int>.Factory,
                Xor.Factory);
        }

        /// <summary>This adds all the initial constanst for Blackboard.</summary>
        private void addConstants() {
            this.Global["e"]     = Literal.Double(S.Math.E);
            this.Global["pi"]    = Literal.Double(S.Math.PI);
            this.Global["tau"]   = Literal.Double(S.Math.Tau);
            this.Global["sqrt2"] = Literal.Double(S.Math.Sqrt(2.0));
        }
        
        /// <summary>An optional log to keep track of which nodes and what order they are evaluated.</summary>
        public TextWriter Log;

        /// <summary>The base set of named nodes to access the total node structure.</summary>
        public Namespace Global { get; }

        /// <summary>This indicates if any changes are pending evaluation.</summary>
        public bool HasPending => this.touched.Count > 0;

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <typeparam name="T">The type of the value to set to the input.</typeparam>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        /// <returns>True if named input node is found and is the correct type, false otherwise.</returns>
        public bool SetValue<T>(T value, params string[] names) where T : IData {
            if (this.Global.Find(names) is IValueInput<T> node) {
                this.SetValue(node, value);
                return true;
            }
            return false;
        }

        /// <summary>Sets the value of the given input node.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="input">The input node to set the value of.</param>
        /// <param name="value">The value to set to the given input.</param>
        public void SetValue<T>(IValueInput<T> input, T value) where T : IData {
            if (input.SetValue(value)) this.touched.Add(input);
        }

        /// <summary>Gets the value of from an named output node.</summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="name">The name of the nde to read the value from.</param>
        /// <returns>
        /// The value from the node or the default value if the node
        /// by that name doesn't exists and the found node is the incorrect type.
        /// </returns>
        public T GetValue<T>(params string[] names) where T : IData =>
            this.Global.Find(names) is IValue<T> node ? node.Value : default;

        /// <summary>This will trigger the node with the given name.</summary>
        /// <param name="names">The name of trigger node to trigger.</param>
        /// <returns>True if a node by that name is found and it was a trigger, false otherwise.</returns>
        public bool Trigger(params string[] names) {
            if (this.Global.Find(names) is ITriggerInput node) {
                this.Trigger(node);
                return true;
            }
            return false;
        }

        /// <summary>This will trigger the given trigger node.</summary>
        /// <param name="input">The input trigger node to trigger.</param>
        public void Trigger(ITriggerInput input) {
            input.Trigger();
            this.touched.Add(input);
        }

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
