using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Blackboard.Core {

    /// <summary>
    /// The driver will store a blackboard node structure and
    /// perform evaluations/updates of change to that structure's values.
    /// </summary>
    public class Driver {

        /// <summary>The input nodes which have been modified.</summary>
        private List<IInput> touched;

        /// <summary>Creates a new driver.</summary>
        /// <param name="log">The optional log to write debug information to during evaluations.</param>
        public Driver(TextWriter log = null) {
            this.Log = log;
            this.touched = new List<IInput>();
            this.Nodes = new Global();

            // TODO: Come up with a way to specify which nodes need to be persisted (inputs, counters, toggles, etc?).
            // TODO: Add persistends which also keeps track of deltas to recover if crashed before save.
        }

        /// <summary>An optional log to keep track of which nodes and what order they are evaluated.</summary>
        public TextWriter Log;

        /// <summary>The base set of named nodes to access the total node structure.</summary>
        public Global Nodes { get; }

        /// <summary>This indicates if any changes are pending evaluation.</summary>
        public bool HasPending => this.touched.Count > 0;

        /// <summary>Determines if the given input or output node by name exists.</summary>
        /// <param name="name">The name of the input or output to look for.</param>
        /// <returns>True if the name exists in this node structure.</returns>
        public bool Contains(string name) => this.Find(name) is not null;

        /// <summary>Finds the given named node by name.</summary>
        /// <param name="name">The name of the dot separated named to look for.</param>
        /// <returns>The named node or null if not found.</returns>
        public INamed Find(string name) => this.Find(name.Split('.'));

        /// <summary>Finds the given named node by scoped names.</summary>
        /// <param name="names">The names to look for.</param>
        /// <returns>The named node or null if not found.</returns>
        public INamed Find(IEnumerable<string> names) {
            INamespace scope = this.Nodes;
            INamed cur = null;
            foreach (string name in names) {
                if (scope is null) return null;
                cur = scope.Find(name);
                scope = cur is INamespace curScope ? curScope : null;
            }
            return cur;
        }

        /// <summary>Sets a value for the given named input.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <typeparam name="T">The type of the value to set to the input.</typeparam>
        /// <param name="name">The name of the input node to set.</param>
        /// <param name="value">The value to set to that node.</param>
        /// <returns>True if named input node is found and is the correct type, false otherwise.</returns>
        public bool SetValue<T>(string name, T value) {
            INode node = this.Find(name);
            if (node is IValueInput<T> input) {
                this.SetValue(input, value);
                return true;
            }
            return false;
        }

        /// <summary>Sets the value of the given input node.</summary>
        /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="input">The input node to set the value of.</param>
        /// <param name="value">The value to set to the given input.</param>
        public void SetValue<T>(IValueInput<T> input, T value) {
            bool changed = input.SetValue(value);
            if (changed) this.touched.Add(input);
        }

        /// <summary>Gets the value of from an named output node.</summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="name">The name of the nde to read the value from.</param>
        /// <returns>
        /// The value from the node or the default value if the node
        /// by that name doesn't exists and the found node is the incorrect type.
        /// </returns>
        public T GetValue<T>(string name) {
            INode node = this.Find(name);
            return (node is IValueOutput<T> input) ? input.Value : default;
        }

        /// <summary>This will trigger the node with the given name.</summary>
        /// <param name="name">The name of trigger node to trigger.</param>
        /// <returns>True if a node by that name is found and it was a trigger, false otherwise.</returns>
        public bool Trigger(string name) {
            INode node = this.Find(name);
            if (node is ITriggerInput input) {
                this.Trigger(input);
                return true;
            }
            return false;
        }

        /// <summary>This will trigger the node with the given name.</summary>
        /// <param name="name">The name of trigger node to trigger.</param>
        /// <returns>True if a node by that name is found and it was a trigger, false otherwise.</returns>
        public bool Trigger(IEnumerable<string> name) {
            INode node = this.Find(name);
            if (node is ITriggerInput input) {
                this.Trigger(input);
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
