using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes;
using Blackboard.Core.Functions;
using System.Collections.Generic;
using System.IO;

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
            // TODO: Add functions
            // TODO: Add initial consts
        }

        private void addOperators() {
            // TODO: Check for existing.
            Namespace operators = new();
            this.Global["operators"] = operators;

            operators["trinary"] = new Collection(Select<Bool>.Factory, Select<Int>.Factory, Select<Double>.Factory);
            operators["logical-or"] = new Collection(Or.Factory, Any.Factory);
            operators["logical-xor"] = new Collection(Xor.Factory, OnlyOne.Factory);

            operators["logical-and"] = new Collection(
                new Func<IValue<Bool>, IValue<Bool>>((left, right) => new And(left, right)),
                new Func<ITrigger,     ITrigger    >((left, right) => new All(left, right)));

            operators["or"] = new Collection(
                new Func<IValue<Bool>, IValue<Bool>>((left, right) => new Or            (left, right)),
                new Func<IValue<Int>,  IValue<Int> >((left, right) => new BitwiseOr<Int>(left, right)),
                new Func<ITrigger,     ITrigger    >((left, right) => new Any           (left, right)));

            operators["xor"] = new Collection(
                new Func<IValue<Bool>, IValue<Bool>>((left, right) => new Xor            (left, right)),
                new Func<IValue<Int>,  IValue<Int> >((left, right) => new BitwiseXor<Int>(left, right)),
                new Func<ITrigger,     ITrigger    >((left, right) => new OnlyOne        (left, right)));

            operators["and"] = new Collection(
                new Func<IValue<Bool>, IValue<Bool>>((left, right) => new And            (left, right)),
                new Func<IValue<Int>,  IValue<Int> >((left, right) => new BitwiseAnd<Int>(left, right)),
                new Func<ITrigger,     ITrigger    >((left, right) => new All            (left, right)));

            operators["equal"] = new Collection(
                new Func<IValue<Bool>,   IValue<Bool>  >((left, right) => new Equal<Bool>  (left, right)),
                new Func<IValue<Int>,    IValue<Int>   >((left, right) => new Equal<Int>   (left, right)),
                new Func<IValue<Double>, IValue<Double>>((left, right) => new Equal<Double>(left, right)),
                new Func<IValue<String>, IValue<String>>((left, right) => new Equal<String>(left, right)));

            operators["not-equal"] = new Collection(
                new Func<IValue<Bool>,   IValue<Bool>  >((left, right) => new NotEqual<Bool>  (left, right)),
                new Func<IValue<Int>,    IValue<Int>   >((left, right) => new NotEqual<Int>   (left, right)),
                new Func<IValue<Double>, IValue<Double>>((left, right) => new NotEqual<Double>(left, right)),
                new Func<IValue<String>, IValue<String>>((left, right) => new NotEqual<String>(left, right)));

            operators["greater"] = new Collection(
                new Func<IValue<Bool>,   IValue<Bool>  >((left, right) => new GreaterThan<Bool>  (left, right)),
                new Func<IValue<Int>,    IValue<Int>   >((left, right) => new GreaterThan<Int>   (left, right)),
                new Func<IValue<Double>, IValue<Double>>((left, right) => new GreaterThan<Double>(left, right)),
                new Func<IValue<String>, IValue<String>>((left, right) => new GreaterThan<String>(left, right)));

            operators["less"] = new Collection(
                new Func<IValue<Bool>,   IValue<Bool>  >((left, right) => new LessThan<Bool>  (left, right)),
                new Func<IValue<Int>,    IValue<Int>   >((left, right) => new LessThan<Int>   (left, right)),
                new Func<IValue<Double>, IValue<Double>>((left, right) => new LessThan<Double>(left, right)),
                new Func<IValue<String>, IValue<String>>((left, right) => new LessThan<String>(left, right)));

            operators["greater-equal"] = new Collection(
                new Func<IValue<Bool>,   IValue<Bool>  >((left, right) => new GreaterThanOrEqual<Bool>  (left, right)),
                new Func<IValue<Int>,    IValue<Int>   >((left, right) => new GreaterThanOrEqual<Int>   (left, right)),
                new Func<IValue<Double>, IValue<Double>>((left, right) => new GreaterThanOrEqual<Double>(left, right)),
                new Func<IValue<String>, IValue<String>>((left, right) => new GreaterThanOrEqual<String>(left, right)));

            operators["less-equal"] = new Collection(
                new Func<IValue<Bool>,   IValue<Bool>  >((left, right) => new LessThanOrEqual<Bool>  (left, right)),
                new Func<IValue<Int>,    IValue<Int>   >((left, right) => new LessThanOrEqual<Int>   (left, right)),
                new Func<IValue<Double>, IValue<Double>>((left, right) => new LessThanOrEqual<Double>(left, right)),
                new Func<IValue<String>, IValue<String>>((left, right) => new LessThanOrEqual<String>(left, right)));

            /*
            this.addProcess("Shift-Right", 2,
                new Input2<IValue<int>, IValue<int>>((left, right) => new RightShift(left, right)));
            this.addProcess("Shift-Left", 2,
                new Input2<IValue<int>, IValue<int>>((left, right) => new LeftShift(left, right)));
            this.addProcess("Sum", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new SumInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new Sum(left, right)),
                new Input2<IValue<String>, IValue<String>>((left, right) => new Sum(left, right)));

            this.addProcess("Subtract", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new SubInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new Sub(left, right)));
            this.addProcess("Multiply", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new MulInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new Mul(left, right)));
            this.addProcess("Divide", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new DivInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new Div(left, right)));
            this.addProcess("Modulo", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new ModInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new Mod(left, right)));
            this.addProcess("Remainder", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new RemInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new Rem(left, right)));
            this.addProcess("Power", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new PowerInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new Power(left, right)));
            this.addProcess("Negate", 1,
                new Input1<IValue<int>>(   (input) => new NegInt(input)),
                new Input1<IValue<double>>((input) => new Neg(input)));
            this.addProcess("Not", 1,
                new Input1<IValue<bool>>((input) => new Not(input)));
            this.addProcess("Invert", 1,
                new Input1<IValue<int>>((input) => new BitwiseNot(input)));
            */
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
