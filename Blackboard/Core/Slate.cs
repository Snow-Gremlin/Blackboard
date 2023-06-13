using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Collections;
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
        private readonly LinkedList<IEvaluable> pendingUpdate;

        /// <summary>The nodes which have had one or more parent modified and they need to be reevaluated.</summary>
        private readonly LinkedList<IEvaluable> pendingEval;

        /// <summary>The set of provoked triggers which need to be reset.</summary>
        private readonly HashSet<ITrigger> needsReset;

        /// <summary>A collection of literals and constants used in the graph.</summary>
        /// <remarks>
        /// This is to help reduce overhead of duplicate constants and
        /// help with optimization of constant branches in the graph.
        /// </remarks>
        private readonly HashSet<IConstant> constants;

        /// <summary>Creates a new slate.</summary>
        /// <param name="addFuncs">Indicates that built-in functions should be added.</param>
        /// <param name="addConsts">Indicates that constants should be added.</param>
        public Slate(bool addFuncs = true, bool addConsts = true) {
            this.pendingUpdate = new LinkedList<IEvaluable>();
            this.pendingEval   = new LinkedList<IEvaluable>();
            this.needsReset    = new HashSet<ITrigger>();
            this.constants     = new HashSet<IConstant>(new NodeValueComparer<IConstant>());
            this.Global        = new Namespace();

            this.addOperators();
            if (addFuncs)  this.addFunctions();
            if (addConsts) this.addConstants();
        }

        /// <summary>The base set of named nodes to access the total node structure.</summary>
        public Namespace Global { get; }

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
            add("castTrigger",
                BoolAsTrigger.Factory);
            add("castBool",
                Explicit<Object, Bool>.Factory);
            add("castInt",
                Explicit<Double, Int>.Factory,
                Explicit<Object, Int>.Factory);
            add("castDouble",
                Implicit<Int, Double>.Factory,
                Explicit<Object, Double>.Factory);
            add("castObject",
                Implicit<Bool, Object>.Factory,
                Implicit<Int, Object>.Factory,
                Implicit<Double, Object>.Factory,
                Implicit<String, Object>.Factory);
            add("castString",
                Implicit<Bool, String>.Factory,
                Implicit<Int, String>.Factory,
                Implicit<Double, String>.Factory,
                Implicit<Object, String>.Factory);
            add("divide",
                Div<Int>.Factory,
                Div<Double>.Factory);
            add("equal",
                Equal<Bool>.Factory,
                Equal<Int>.Factory,
                Equal<Double>.Factory,
                Equal<Object>.Factory,
                Equal<String>.Factory);
            add("greater",
                GreaterThan<Int>.Factory,
                GreaterThan<Double>.Factory,
                GreaterThan<String>.Factory);
            add("greaterEqual",
                GreaterThanOrEqual<Int>.Factory,
                GreaterThanOrEqual<Double>.Factory,
                GreaterThanOrEqual<String>.Factory);
            add("invert",
                BitwiseNot<Int>.Factory);
            add("less",
                LessThan<Int>.Factory,
                LessThan<Double>.Factory,
                LessThan<String>.Factory);
            add("lessEqual",
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
                XorTrigger.Factory);
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
                NotEqual<Object>.Factory,
                NotEqual<String>.Factory);
            add("or",
                Or.Factory,
                BitwiseOr<Int>.Factory,
                Any.Factory);
            add("power",
                BinaryFloatingPoint<Double>.Pow);
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
                SelectValue<Object>.Factory,
                SelectValue<String>.Factory,
                SelectTrigger.Factory);
            add("xor",
                Xor.Factory,
                BitwiseXor<Int>.Factory,
                XorTrigger.Factory);
        }

        /// <summary>This adds all global initial methods for Blackboard.</summary>
        private void addFunctions() {
            void add(string name, params IFuncDef[] defs) =>
                this.Global[name] = new FuncGroup(defs);

            add("abs",
                Abs<Int>.Factory,
                Abs<Double>.Factory);
            add("acos",
                UnaryFloatingPoint<Double>.Acos);
            add("acosh",
                UnaryFloatingPoint<Double>.Acosh);
            add("all",
                All.Factory);
            add("and",
                And.Factory,
                BitwiseAnd<Int>.Factory);
            add("any",
                Any.Factory);
            add("asin",
                UnaryFloatingPoint<Double>.Asin);
            add("asinh",
                UnaryFloatingPoint<Double>.Asinh);
            add("atan",
                BinaryFloatingPoint<Double>.Atan2,
                UnaryFloatingPoint<Double>.Atan);
            add("atanh",
                UnaryFloatingPoint<Double>.Atanh);
            add("average",
                Average.Factory);
            add("cbrt",
                UnaryFloatingPoint<Double>.Cbrt);
            add("ceiling",
                UnaryFloatingPoint<Double>.Ceiling);
            add("clamp",
                TernaryComparable<Int>.Clamp,
                TernaryComparable<Double>.Clamp);
            add("contains",
                Binary.Contains);
            add("cos",
                UnaryFloatingPoint<Double>.Cos);
            add("cosh",
                UnaryFloatingPoint<Double>.Cosh);
            add("endsWith",
                Binary.EndsWith);
            add("epsilon",
                EpsilonEqual<Double>.Factory);
            add("exp",
                UnaryFloatingPoint<Double>.Exp);
            add("floor",
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
                TernaryComparable<Double>.InRange);
            add("insert",
                Ternary.Insert);
            add("isEmpty",
                Unary.IsEmpty);
            add("isFinite",
                UnaryFloatingPoint<Double>.IsFinite);
            add("isInf",
                UnaryFloatingPoint<Double>.IsInfinity);
            add("isNaN",
                UnaryFloatingPoint<Double>.IsNaN);
            add("isNeg",
                UnarySigned<Int>.IsNegative,
                UnarySigned<Double>.IsNegative);
            add("isNegInf",
                UnaryFloatingPoint<Double>.IsNegativeInfinity);
            add("isNormal",
                UnaryFloatingPoint<Double>.IsNormal);
            add("isNull",
                UnaryNullable<Object>.IsNull);
            add("isPosInf",
                UnaryFloatingPoint<Double>.IsPositiveInfinity);
            add("isSubnormal",
                UnaryFloatingPoint<Double>.IsSubnormal);
            add("latch",
                Latch<Bool>.Factory,
                Latch<Int>.Factory,
                Latch<Double>.Factory,
                Latch<String>.Factory,
                Latch<Object>.Factory);
            add("lerp",
                Lerp<Double>.Factory);
            add("length",
                Unary.Length);
            add("log",
                BinaryFloatingPoint<Double>.Log,
                UnaryFloatingPoint<Double>.Log);
            add("log10",
                UnaryFloatingPoint<Double>.Log10);
            add("log2",
                UnaryFloatingPoint<Double>.Log2);
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
            add("padLeft",
                Binary.PadLeft,
                Ternary.PadLeft);
            add("padRight",
                Binary.PadRight,
                Ternary.PadRight);
            add("pow",
                BinaryFloatingPoint<Double>.Pow);
            add("remainder",
                BinaryFloatingPoint<Double>.IEEERemainder);
            add("remove",
                Ternary.Remove);
            add("round",
                BinaryFloatingPoint<Double>.Round,
                UnaryFloatingPoint<Double>.Round);
            add("select",
                SelectValue<Bool>.Factory,
                SelectValue<Int>.Factory,
                SelectValue<Double>.Factory,
                SelectValue<String>.Factory,
                SelectValue<Object>.Factory,
                SelectTrigger.Factory);
            add("sin",
                UnaryFloatingPoint<Double>.Sin);
            add("sinh",
                UnaryFloatingPoint<Double>.Sinh);
            add("sqrt",
                UnaryFloatingPoint<Double>.Sqrt);
            add("startsWith",
                Binary.StartsWith);
            add("substring",
                Ternary.Substring);
            add("sum",
                Sum<Int>.Factory(),
                Sum<Double>.Factory(),
                Sum<String>.Factory(true));
            add("tan",
                UnaryFloatingPoint<Double>.Tan);
            add("tanh",
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
                UnaryFloatingPoint<Double>.Truncate);
            add("xor",
                BitwiseXor<Int>.Factory,
                Xor.Factory,
                XorTrigger.Factory);
            add("zener",
                Zener<Int>.Factory,
                Zener<Double>.Factory);
        }

        /// <summary>This adds all the initial constants for Blackboard.</summary>
        private void addConstants() {
            this.Global["e"    ] = Literal.Double(S.Math.E);
            this.Global["pi"   ] = Literal.Double(S.Math.PI);
            this.Global["tau"  ] = Literal.Double(S.Math.Tau);
            this.Global["sqrt2"] = Literal.Double(S.Math.Sqrt(2.0));
            this.Global["nan"  ] = Literal.Data(Double.NaN);
            this.Global["inf"  ] = Literal.Data(Double.Infinity);
            this.Global["null" ] = Literal.Data(Object.Null);
        }

        #endregion
        #region Value Setters...

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        public void SetBool(bool value, params string[] names) =>
            this.SetValue(new Bool(value), names);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        public void SetInt(int value, params string[] names) =>
            this.SetValue(new Int(value), names);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        public void SetDouble(double value, params string[] names) =>
            this.SetValue(new Double(value), names);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        public void SetString(string value, params string[] names) =>
            this.SetValue(new String(value), names);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        public void SetObject(object value, params string[] names) =>
            this.SetValue(new Object(value), names);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <typeparam name="T">The type of the value to set to the input.</typeparam>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        public void SetValue<T>(T value, params string[] names) where T : IData =>
            this.SetValue(value, names as IEnumerable<string>);

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <typeparam name="T">The type of the value to set to the input.</typeparam>
        /// <param name="value">The value to set to that node.</param>
        /// <param name="names">The name of the input node to set.</param>
        public void SetValue<T>(T value, IEnumerable<string> names) where T : IData =>
            this.SetValue(value, this.GetNode<IValueInput<T>>(names));

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

        /// <summary>This will provoke the node with the given name.</summary>
        /// <param name="names">The name of trigger node to provoke.</param>
        public void Provoke(params string[] names) =>
            this.Provoke(names as IEnumerable<string>);

        /// <summary>This will provoke the node with the given name.</summary>
        /// <param name="names">The name of trigger node to provoke.</param>
        public void Provoke(IEnumerable<string> names) =>
            this.Provoke(this.GetNode<ITriggerInput>(names));

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

        #endregion
        #region Value Getters...

        /// <summary>Gets the type of the value at the given node.</summary>
        /// <param name="names">The name of the node to get the type of.</param>
        /// <returns>The type of the node or null if doesn't exist or not a node type.</returns>
        public Type GetType(IEnumerable<string> names) {
            object obj = this.Global.Find(names);
            return obj is null ? null : Type.FromType(obj.GetType());
        }

        /// <summary>Gets the value from a named node.</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value from the node.</returns>
        public bool GetBool(params string[] names) =>
            this.GetValue<Bool>(names).Value;

        /// <summary>Gets the value from a named node.</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value from the node.</returns>
        public int GetInt(params string[] names) =>
            this.GetValue<Int>(names).Value;

        /// <summary>Gets the value from a named node.</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value from the node.</returns>
        public double GetDouble(params string[] names) =>
            this.GetValue<Double>(names).Value;

        /// <summary>Gets the value from a named node.</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value from the node.</returns>
        public string GetString(params string[] names) =>
            this.GetValue<String>(names).Value;

        /// <summary>Gets the value from a named node.</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value from the node.</returns>
        public object GetObject(params string[] names) =>
            this.GetValue<Object>(names).Value;

        /// <summary>Gets the value from a named node.</summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value from the node.</returns>
        public T GetValue<T>(params string[] names) where T : IData =>
            this.GetValue<T>(names as IEnumerable<string>);

        /// <summary>Gets the value from a named node.</summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value from the node.</returns>
        public T GetValue<T>(IEnumerable<string> names) where T : IData =>
            this.GetNode<IValue<T>>(names).Value;

        /// <summary>Gets the value as an object from a named node</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value as an object from the node.</returns>
        public object GetValueAsObject(params string[] names) =>
            this.GetValueAsObject(names as IEnumerable<string>);

        /// <summary>Gets the value as an object from a named node</summary>
        /// <param name="name">The name of the node to read the value from.</param>
        /// <returns>The value as an object from the node.</returns>
        public object GetValueAsObject(IEnumerable<string> names) =>
            this.GetNode<IDataNode>(names).Data.ValueAsObject;

        /// <summary>Indicates if the trigger is currently provoked while waiting to be evaluated.</summary>
        /// <param name="names">The name of trigger node to get the state from.</param>
        /// <returns>True if a node by that name is found and it is provoked, false otherwise.</returns>
        public bool Provoked(params string[] names) =>
            this.Provoked(names as IEnumerable<string>);

        /// <summary>Indicates if the trigger is currently provoked while waiting to be evaluated.</summary>
        /// <param name="names">The name of trigger node to get the state from.</param>
        /// <returns>True if a node by that name is found and it is provoked, false otherwise.</returns>
        public bool Provoked(IEnumerable<string> names) =>
            this.GetNode<ITrigger>(names).Provoked;

        /// <summary>Gets the node with the given name.</summary>
        /// <typeparam name="T">The expected type of node to get.</typeparam>
        /// <param name="names">The name of the node to get.</param>
        /// <returns>The node with the given name and type.</returns>
        public T GetNode<T>(params string[] names) where T : INode =>
            this.GetNode<T>(names as IEnumerable<string>);

        /// <summary>Gets the node with the given name.</summary>
        /// <remarks>This will throw an exception if no node by that name exists or the found node is the incorrect type.</remarks>
        /// <typeparam name="T">The expected type of node to get.</typeparam>
        /// <param name="names">The name of the node to get.</param>
        /// <returns>The node with the given name and type.</returns>
        public T GetNode<T>(IEnumerable<string> names) where T : INode {
            object obj = this.Global.Find(names);
            return obj is null ?
                    throw new Message("Unable to get a node by the given name.").
                        With("Name", names.Join(".")).
                        With("Value Type", typeof(T)) :
                obj is not T node ?
                    throw new Message("The node found by the given name is not the expected type.").
                        With("Name", names.Join(".")).
                        With("Found Type", obj.GetType()).
                        With("Expected Type", typeof(T)) :
                node;
        }

        #endregion
        #region Output...

        /// <summary>Gets or creates a new output value on the node with the given name.</summary>
        /// <typeparam name="T">The data type of the value to output.</typeparam>
        /// <param name="names">The name of the node to look up.</param>
        /// <returns>The new or existing value output.</returns>
        public IValueOutput<T> GetOutputValue<T>(params string[] names)
            where T : struct, IEquatable<T> =>
            this.GetOutputValue<T>(names as IEnumerable<string>);

        /// <summary>Gets or creates a new output value on the node with the given name.</summary>
        /// <typeparam name="T">The data type of the value to output.</typeparam>
        /// <param name="names">The name of the node to look up.</param>
        /// <returns>The new or existing value output.</returns>
        public IValueOutput<T> GetOutputValue<T>(IEnumerable<string> names)
            where T : struct, IEquatable<T> {
            IValueParent<T> parent = this.GetNode<IValueParent<T>>(names);
            IValueOutput<T>? output = parent.Children.OfType<IValueOutput<T>>().FirstOrDefault();
            if (output is not null) return output;

            output = new OutputValue<T>(parent);
            output.Legitimatize();
            return output;
        }

        /// <summary>Gets or creates a new output trigger on the node with the given name.</summary>
        /// <param name="names">The name of the node to look up.</param>
        /// <returns>The new or existing trigger output.</returns>
        public ITriggerOutput GetOutputTrigger(params string[] names) =>
            this.GetOutputTrigger(names as IEnumerable<string>);

        /// <summary>Gets or creates a new output trigger on the node with the given name.</summary>
        /// <param name="names">The name of the node to look up.</param>
        /// <returns>The new or existing trigger output.</returns>
        public ITriggerOutput GetOutputTrigger(IEnumerable<string> names) {
            ITriggerParent parent = this.GetNode<ITriggerParent>(names);
            ITriggerOutput output = parent.Children.OfType<ITriggerOutput>().FirstOrDefault();
            if (output is not null) return output;

            output = new OutputTrigger(parent);
            output.Legitimatize();
            return output;
        }
        
        #endregion
        #region Update...

        /// <summary>This indicates that the given nodes have had parents added or removed and need to be updated.</summary>
        /// <param name="nodes">The nodes to pend evaluation for.</param>
        public void PendUpdate(params INode[] nodes) =>
            this.PendUpdate(nodes as IEnumerable<INode>);

        /// <summaryThis indicates that the given nodes have had parents added or removed and need to be updated.</summary>
        /// <remarks>This will pend the given nodes to update the depths prior to evaluation.</remarks>
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
        public void PerformUpdates(Logger? logger = null) =>
            this.pendingUpdate.UpdateDepths(logger.SubGroup(nameof(PerformUpdates)));

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
        public void PerformEvaluation(Logger? logger = null) =>
            this.NeedsReset(this.pendingEval.Evaluate(logger.SubGroup(nameof(PerformEvaluation))));

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
        #region Constants...

        /// <summary>Determines if the given constant reference is in the set of constants.</summary>
        /// <typeparam name="T">The type of the constant to check for.</typeparam>
        /// <param name="con">The constant to check for.</param>
        /// <returns>True if the given constant reference is in the set, false otherwise.</returns>
        public bool ContainsConstant<T>(T con) where T : class, IConstant =>
            this.constants.TryGetValue(con, out IConstant? result) && ReferenceEquals(result, con);

        /// <summary>Find or add the given constant in the set of constants.</summary>
        /// <remarks>
        /// Constants should only be checked like this if we know it is going to be used.
        /// Constants with zero children should not exist in this storage.
        /// </remarks>
        /// <typeparam name="T">The type of the constant to add or find.</typeparam>
        /// <param name="con">The constant to find already stored or to add.</param>
        /// <returns>The already existing constant or the passed in constant if added.</returns>
        public T? FindAddConstant<T>(T con)
            where T : class, IConstant {
            if (this.constants.TryGetValue(con, out IConstant? result)) return result as T;
            this.constants.Add(con);
            return con;
        }

        #endregion

        /// <summary>Gets the string for the graph of this slate without functions.</summary>
        /// <returns>The human readable debug string for the slate.</returns>
        public override string ToString() => Stringifier.GraphString(this);
    }
}
