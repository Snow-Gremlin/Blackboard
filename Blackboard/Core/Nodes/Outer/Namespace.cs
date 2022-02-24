using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>A dictionary for containing named objects.</summary>
    sealed public class Namespace: IFieldWriter {

        // These are the named children of this namespace.
        private SortedDictionary<string, INode> fields;

        /// <summary>Creates a new namespace.</summary>
        public Namespace() {
            this.fields = new SortedDictionary<string, INode>();
        }

        /// <summary>This is the type name of the node.</summary>
        public string TypeName => "Namespace";

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
        /// <returns>True if the name exists in this node.</returns>
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
        public void WriteField(string name, INode node) {
            if (node is null)
                throw new Message("May not write a null node to a namespace.").
                    With("Name", name).
                    With("Namespace", this);
            if (this.fields.ContainsKey(name))
                throw new Message("A node by the given name already exists in the namespace.").
                    With("Name", name).
                    With("Node", node).
                    With("Namespace", this);
            this.fields[name] = node;
        }

        /// <summary>Removes fields from this node by name if they exist.</summary>
        /// <param name="names">The names of the fields to remove.</param>
        /// <returns>True if the fields were removed, false otherwise.</returns>
        public bool RemoveFields(IEnumerable<string> names) {
            bool removed = true;
            foreach (string name in names) {
                if (!this.fields.Remove(name)) removed = false;
            }
            return removed;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
