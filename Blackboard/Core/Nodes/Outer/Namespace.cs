using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>A dictionary for containing named objects.</summary>
    sealed public class Namespace: INode, IFieldReader, IFieldWriter {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<Namespace>(() => new Namespace());

        // These are the named children of this namesapce.
        private SortedDictionary<string, INode> fields;

        /// <summary>Creates a new namespace.</summary>
        public Namespace() {
            this.fields = new SortedDictionary<string, INode>();
        }

        /// <summary>This is the type name of the node.</summary>
        public string TypeName => "Namespace";

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>The set of children nodes to this node in the graph.</summary>
        public IEnumerable<INode> Children => this.fields.Values;

        /// <summary>Gets or sets the field in this namespace.</summary>
        /// <param name="name">The name of the field.</param>
        /// <returns>The node to get or set to this field.</returns>
        public INode this[string name] {
            get => this.ReadField(name);
            set => this.WriteField(name, value);
        }

        /// <summary>Gets the name for the given field.</summary>
        /// <param name="node">The node in the field to look up the name for.</param>
        /// <returns>The name for the given field or null if not found.</returns>
        public string NameOfField(INode node) =>
            this.fields.FirstOrDefault((pair) => ReferenceEquals(pair.Value, node)).Key;

        /// <summary>Determines if the given field by name exists.</summary>
        /// <param name="name">The name of the field to look for.</param>
        /// <returns>True if the name exists in this node node.</returns>
        public bool ContainsField(string name) => this.fields.ContainsKey(name);

        /// <summary>Reads the node for the field by the given name.</summary>
        /// <param name="name">The name for the node to look for.</param>
        /// <returns>The node or null if not found.</returns>
        public INode ReadField(string name) => this.fields.ContainsKey(name) ? this.fields[name] : null;

        /// <summary>Gets all the fields in this namespace with the name for each field.</summary>
        public IEnumerable<KeyValuePair<string, INode>> Fields => this.fields;

        /// <summary>Writes or overwrites a new field to this node.</summary>
        /// <param name="name">The name of the field to write.</param>
        /// <param name="node">The node to write to the field.</param>
        /// <param name="checkedForLoops">Indicates if loops in the graph should be checked for.</param>
        public void WriteField(string name, INode node, bool checkedForLoops = true) {
            if (node is null)
                throw new Exception("May not write a null node to a namespace.").
                    With("Name", name).
                    With("Namespace", this);
            if (this.fields.ContainsKey(name))
                throw new Exception("A node by the given name already exists in the namespace.").
                    With("Name", name).
                    With("Node", node).
                    With("Namespace", this);
            if (checkedForLoops && this.CanReachAny(node))
                throw Exceptions.NodeLoopDetected();
            this.fields[name] = node;
        }

        /// <summary>Remove a field from this node by name if it exists.</summary>
        /// <param name="name">The name of the fields to remove.</param>
        /// <returns>True if the field wwas removed, false otherwise.</returns>
        public bool RemoveField(string name) => this.fields.Remove(name);

        /// <summary>Finds the node at the given path.</summary>
        /// <param name="names">The names to the node to find.</param>
        /// <returns>The node at the end of the path or null.</returns>
        public INode Find(params string[] names) => this.Find(names as IEnumerable<string>);

        /// <summary>Finds the node at the given path.</summary>
        /// <param name="names">The names to the node to find.</param>
        /// <returns>The node at the end of the path or null.</returns>
        public INode Find(IEnumerable<string> names) {
            INode cur = this;
            foreach (string name in names) {
                if (cur is Namespace scope) {
                    if (!scope.ContainsField(name)) return null;
                    cur = scope[name];
                } else return null;
            }
            return cur;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
