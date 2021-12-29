using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Inspect;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core {

    /// <summary>
    /// The slate stores all blackboard data via a node graph and
    /// perform evaluations/updates of change the values of the nodes.
    /// </summary>
    public class Slate {

        /// <summary>The namespace for all the operators.</summary>
        public const string OperatorNamespace = "$operators";

        /// <summary>The nodes which have had one or more parent modified and they need to have their depth updated.</summary>
        private LinkedList<IEvaluable> pendingUpdate;

        /// <summary>The nodes which have had one or more parent modified and they need to be reevaluated.</summary>
        private LinkedList<IEvaluable> pendingEval;

        /// <summary>The set of provoked triggers which need to be reset.</summary>
        private HashSet<ITrigger> needsReset;

        /// <summary>Creates a new slate.</summary>
        /// <param name="addFuncs">Indicates that built-in functions should be added.</param>
        /// <param name="addConsts">Indicates that constants should be added.</param>
        public Slate(bool addFuncs = true, bool addConsts = true) {
            this.pendingUpdate = new LinkedList<IEvaluable>();
            this.pendingEval   = new LinkedList<IEvaluable>();
            this.needsReset    = new HashSet<ITrigger>();
            this.Global = new Namespace();

            this.addOperators();
            if (addFuncs)  this.addFunctions();
            if (addConsts) this.addConstants();
        }

        #region Built-in Functions and Constants...

        /// <summary>This adds all the operators used by the language.</summary>
        private void addOperators() {
            Namespace operators = new();
            this.Global[OperatorNamespace] = operators;
            void add(string name, params IFuncDef[] defs) =>
                operators[name] = new FuncGroup(defs);

            add("and",
                And.Factory,
                BitwiseAnd<Int>.Factory,
                All.Factory);
            add("castTrigger");
            add("castBool");
            add("castInt",
                Explicit<Double, Int>.Factory);
            add("castDouble",
                Implicit<Int, Double>.Factory);
            add("castString",
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
            add("greaterEqual",
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
            add("lessEqual",
                LessThanOrEqual<Bool>.Factory,
                LessThanOrEqual<Int>.Factory,
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
            add("notEqual",
                NotEqual<Bool>.Factory,
                NotEqual<Int>.Factory,
                NotEqual<Double>.Factory,
                NotEqual<String>.Factory);
            add("or",
                Or.Factory,
                BitwiseOr<Int>.Factory,
                Any.Factory);
            add("power",
                BinaryDoubleMath<Double>.Pow);
            add("shiftLeft",
                LeftShift<Int>.Factory);
            add("shiftRight",
                RightShift<Int>.Factory);
            add("subtract",
                Sub<Int>.Factory,
                Sub<Double>.Factory);
            add("sum",
                Sum<Int>.Factory(),
                Sum<Double>.Factory(),
                Sum<String>.Factory(true));
            add("ternary",
                SelectValue<Bool>.Factory,
                SelectValue<Int>.Factory,
                SelectValue<Double>.Factory,
                SelectValue<String>.Factory,
                SelectTrigger.Factory);
            add("xor",
                Xor.Factory,
                BitwiseXor<Int>.Factory,
                OnlyOne.Factory);
        }

        /// <summary>This adds all global initial methods for Blackboard.</summary>
        private void addFunctions() {
            void add(string name, params IFuncDef[] defs) =>
                this.Global[name] = new FuncGroup(defs);

            add("abs",
                Abs<Int>.Factory,
                Abs<Double>.Factory);
            add("acos",
                UnaryDoubleMath<Double>.Acos);
            add("acosh",
                UnaryDoubleMath<Double>.Acosh);
            add("all",
                All.Factory);
            add("and",
                And.Factory,
                BitwiseAnd<Int>.Factory);
            add("any",
                Any.Factory);
            add("asin",
                UnaryDoubleMath<Double>.Asin);
            add("asinh",
                UnaryDoubleMath<Double>.Asinh);
            add("atan",
                BinaryDoubleMath<Double>.Atan2,
                UnaryDoubleMath<Double>.Atan);
            add("atanh",
                UnaryDoubleMath<Double>.Atanh);
            add("average",
                Average.Factory);
            add("cbrt",
                UnaryDoubleMath<Double>.Cbrt);
            add("ceiling",
                UnaryDoubleMath<Double>.Ceiling);
            add("clamp",
                Clamp<Int>.Factory,
                Clamp<Double>.Factory);
            add("cos",
                UnaryDoubleMath<Double>.Cos);
            add("cosh",
                UnaryDoubleMath<Double>.Cosh);
            add("exp",
                UnaryDoubleMath<Double>.Exp);
            add("epsilon",
                EpsilonEqual<Double>.Factory);
            add("floor",
                UnaryDoubleMath<Double>.Floor);
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
                BinaryDoubleMath<Double>.Log,
                UnaryDoubleMath<Double>.Log);
            add("log10",
                UnaryDoubleMath<Double>.Log10);
            add("log2",
                UnaryDoubleMath<Double>.Log2);
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
                UnaryDoubleMath<Double>.Round,
                Round<Double>.Factory);
            add("select",
                SelectValue<Bool>.Factory,
                SelectValue<Int>.Factory,
                SelectValue<Double>.Factory,
                SelectValue<String>.Factory,
                SelectTrigger.Factory);
            add("sin",
                UnaryDoubleMath<Double>.Sin);
            add("sinh",
                UnaryDoubleMath<Double>.Sinh);
            add("sqrt",
                UnaryDoubleMath<Double>.Sqrt);
            add("sum",
                Sum<Int>.Factory(),
                Sum<Double>.Factory(),
                Sum<String>.Factory(true));
            add("tan",
                UnaryDoubleMath<Double>.Tan);
            add("tanh",
                UnaryDoubleMath<Double>.Tanh);
            add("trunc",
                UnaryDoubleMath<Double>.Truncate);
            add("xor",
                BitwiseXor<Int>.Factory,
                Xor.Factory);
        }

        /// <summary>This adds all the initial constants for Blackboard.</summary>
        private void addConstants() {
            void add(string name, double value) =>
                this.Global[name] = Literal.Double(value);

            add("e", S.Math.E);
            add("pi", S.Math.PI);
            add("tau", S.Math.Tau);
            add("sqrt2", S.Math.Sqrt(2.0));
        }

        #endregion
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
        /// <remarks>
        /// This will not cause an evaluation right away.
        /// If the value is changed, then the children of this node will be pending evaluation
        /// so that they are pending for the next evaluation.
        /// </remarks>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="input">The input node to set the value of.</param>
        /// <param name="value">The value to set to the given input.</param>
        public void SetValue<T>(T value, IValueInput<T> input) where T : IData {
            if (input.SetValue(value)) this.PendEval(input.Children);
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
        /// <remarks>
        /// This will not cause an evaluation right away.
        /// If the value is changed, then the children of this node will be pending evaluation
        /// so that they are pending for the next evaluation.
        /// </remarks>
        /// <param name="input">The input trigger node to provoke.</param>
        /// <param name="value">The provoke state to set, typically this will be true.</param>
        public void Provoke(ITriggerInput input, bool value = true) {
            if (input.Provoke(value)) {
                this.PendEval(input.Children);
                this.NeedsReset(input);
            }
        }

        /// <summary>Indicates if the trigger is currently provoked while waiting to be evaluated.</summary>
        /// <param name="names">The name of trigger node to get the state from.</param>
        /// <returns>True if a node by that name is found and it is provoked, false otherwise.</returns>
        public bool Provoked(params string[] names) =>
            this.Provoked(names as IEnumerable<string>);

        /// <summary>Indicates if the trigger is currently provoked while waiting to be evaluated.</summary>
        /// <param name="names">The name of trigger node to get the state from.</param>
        /// <returns>True if a node by that name is found and it is provoked, false otherwise.</returns>
        public bool Provoked(IEnumerable<string> names) =>
            this.Global.Find(names) is ITrigger node && node.Provoked;

        #endregion
        #region Value Getters...

        /// <summary>Gets the type of the value or trigger at the given node.</summary>
        /// <param name="names">The name of the node to get the type of.</param>
        /// <returns>The type of the node or null if doesn't exist or not a node type.</returns>
        public Type GetType(IEnumerable<string> names) {
            object obj = this.Global.Find(names);
            return obj is null ? null : Type.FromType(obj.GetType());
        }

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
        /// <remarks>This will throw an exception if no node by that name exists or the found node is the incorrect type.</remarks>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value from the node.</returns>
        public T GetValue<T>(IEnumerable<string> names) where T : IData {
            object obj = this.Global.Find(names);
            return obj is null ?
                    throw new Exception("Unable to get a value by the given name.").
                        With("Name", names.Join(".")).
                        With("Value Type", typeof(T)) :
                obj is not IValue<T> node ?
                    throw new Exception("The value found by the given name is not the expected type.").
                        With("Name", names.Join(".")).
                        With("Found Type", obj.GetType()).
                        With("Expected Type", typeof(T)) :
                node.Value;
        }

        #endregion

        /// <summary>The base set of named nodes to access the total node structure.</summary>
        public Namespace Global { get; }

        #region Update...

        /// <summary> This indicates that the given nodes have had parents added or removed and need to be updated. </summary>
        /// <param name="nodes">The nodes to pend evaluation for.</param>
        public void PendUpdate(params INode[] nodes) =>
            this.PendUpdate(nodes as IEnumerable<INode>);

        /// <summaryThis indicates that the given nodes have had parents added or removed and need to be updated.</summary>
        /// <param name="nodes">The nodes to pend evaluation for.</param>
        public void PendUpdate(IEnumerable<INode> nodes) =>
            this.pendingUpdate.SortInsertUnique(nodes.NotNull().OfType<IEvaluable>());

        /// <summary>This gets all the nodes pending update.</summary>
        public IEnumerable<INode> PendingUpdate => this.pendingEval;

        /// <summary>This indicates if any nodes are pending update.</summary>
        public bool HasPendingUpdate => this.pendingUpdate.Count > 0;

        /// <summary>
        /// This updates the depth values of the given pending nodes and
        /// propagates the updating through the Blackboard nodes.
        /// </summary>
        /// <remarks>By performing the update the pending update list will be cleared.</remarks>
        /// <param name="logger">An optional logger for debugging this update.</param>
        public void PerformUpdates(ILogger logger = null) =>
            this.pendingUpdate.UpdateDepths(logger);

        #endregion
        #region Evaluate...

        /// <summary>
        /// This indicates that the given nodes have had parents changed
        /// and need to be recalculated during evaluation.
        /// </summary>
        /// <param name="nodes">The nodes to pend evaluation for.</param>
        public void PendEval(params INode[] nodes) =>
            this.PendEval(nodes as IEnumerable<INode>);

        /// <summary>
        /// This indicates that the given nodes have had parents changed
        /// and need to be recalculated during evaluation.
        /// </summary>
        /// <param name="nodes">The nodes to pend evaluation for.</param>
        public void PendEval(IEnumerable<INode> nodes) =>
            this.pendingEval.SortInsertUnique(nodes.NotNull().OfType<IEvaluable>());

        /// <summary>This gets all the nodes pending evaluation.</summary>
        public IEnumerable<INode> PendingEval => this.pendingEval;

        /// <summary>This indicates if any changes are pending evaluation.</summary>
        public bool HasPendingEval => this.pendingEval.Count > 0;

        /// <summary>
        /// Performs an evaluation of all pending nodes and
        /// propagates the changes through the Blackboard nodes.
        /// </summary>
        /// <remarks>By performing the update the pending evaluation list will be cleared.</remarks>
        /// <param name="logger">An optional logger for debugging this evaluation.</param>
        public void PerformEvaluation(ILogger logger = null) =>
            this.NeedsReset(this.pendingEval.Evaluate(logger));

        #endregion
        #region Needs Reset...

        /// <summary>This adds provoked trigger nodes which need to be reset.</summary>
        /// <param name="nodes">The provoked trigger nodes to add.</param>
        public void NeedsReset(params ITrigger[] nodes) =>
            this.PendEval(nodes as IEnumerable<ITrigger>);

        /// <summary>This adds provoked trigger nodes which need to be reset.</summary>
        /// <param name="nodes">The provoked trigger nodes to add.</param>
        public void NeedsReset(IEnumerable<ITrigger> nodes) => nodes.NotNull().
            Where(trig => trig.Provoked).Foreach(node => this.needsReset.Add(node));

        /// <summary>This will reset all provoked trigger nodes which have been added.</summary>
        /// <remarks>The needs reset set will be cleared by this call.</remarks>
        public void ResetTriggers() {
            this.needsReset.Reset();
            this.needsReset.Clear();
        }

        /// <summary>Indicates if there are any provoked triggers which need results.</summary>
        public bool HasTriggersNeedingReset => this.needsReset.Count > 0;

        #endregion

        /// <summary>Gets the string for the graph of this slate without functions.</summary>
        /// <returns>The human readable debug string for the slate.</returns>
        public override string ToString() => Stringifier.GraphString(this);
    }
}
