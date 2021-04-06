using Blackboard.Core.Caps;
using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using Blackboard.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Blackboard.Core {
    public class Driver {

        /// <summary>The input nodes which have been modified.</summary>
        private List<IInput> touched;

        public Driver(TextWriter log = null) {
            this.Log = log;
            this.touched = new List<IInput>();
            this.Nodes = new Global();
        }

        /// <summary>An optional log to keep track of which nodes and what order they are evaluated.</summary>
        public TextWriter Log;

        public Global Nodes { get; }

        public bool HasPending => this.touched.Count > 0;

        public bool Contains(string name) => !(this.Find(name) is null);

        public INode Find(string name) {
            INamespace scope = this.Nodes;
            string[] parts = name.Split('.');
            int max = parts.Length-1;
            for (int i = 0; i <= max; ++i) {
                // TODO: Should sort children in global and namespace to make a faster lookup.
                INode node = scope.Find(parts[i]);
                if (i == max) return node;
                if (!(node is INamespace)) return null;
                scope = node as INamespace;
            }
            return null;
        }

        public bool SetValue<T>(string name, T value) {
            INode node = this.Find(name);
            if (!(node is IValueInput<T>)) return false;
            IValueInput<T> input = node as IValueInput<T>;
            bool changed = input.SetValue(value);
            if (changed) this.touched.Add(input);
            return true;
        }

        public T GetValue<T>(string name) {
            INode node = this.Find(name);
            return (node is IValueOutput<T>) ? (node as IValueOutput<T>).Value : default;
        }

        public bool Trigger(string name) {
            INode node = this.Find(name);
            if (!(node is ITriggerInput)) return false;
            ITriggerInput input = node as ITriggerInput;
            input.Trigger();
            this.touched.Add(input);
            return true;
        }

        /// <summary>Updates and propogates the changes from the given inputs through the blackboard nodes.</summary>
        public void Evalate() {
            LinkedList<INode> pending = new LinkedList<INode>();
            LinkedList<ITrigger> needsReset = new LinkedList<ITrigger>();
            pending.SortInsertUnique(this.touched);
            this.touched.Clear();

            while (pending.Count > 0) {
                INode node = pending.TakeFirst();
                if (!(this.Log is null))
                    this.Log.WriteLine("Eval("+node.Depth+"): "+node);
                pending.SortInsertUnique(node.Eval());
                if (node is ITrigger)
                    needsReset.AddLast(node as ITrigger);
            }

            foreach (ITrigger trigger in needsReset)
                trigger.Reset();
        }
    }
}
