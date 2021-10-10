using System.Collections.Generic;
using Blackboard.Core.Nodes.Interfaces;
using System.Linq;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>A dictionary for containing named objects.</summary>
    sealed public class Namespace: INode, IFieldReader, IFieldWriter {
        private SortedDictionary<string, INode> fields;

        /// <summary>Creates a new namespace.</summary>
        public Namespace() {
            this.fields = new SortedDictionary<string, INode>();
        }

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

        /// <summary>Determines if the given field by name exists.</summary>
        /// <param name="name">The name of the field to look for.</param>
        /// <returns>True if the name exists in this node node.</returns>
        public bool ContainsField(string name) => this.fields.ContainsKey(name);

        /// <summary>Reads the node for the field by the given name.</summary>
        /// <param name="name">The name for the node to look for.</param>
        /// <returns>The node or null if not found.</returns>
        public INode ReadField(string name) => this.fields.ContainsKey(name) ? this.fields[name] : null;

        /// <summary>Writes or overwrites a new field to this node.</summary>
        /// <param name="name">The name of the field to write.</param>
        /// <param name="node">The node to write to the field.</param>
        public void WriteField(string name, INode node) => this.fields[name] = node;

        /// <summary>Remove a field from this node by name if it exists.</summary>
        /// <param name="name">The name of the fields to remove.</param>
        /// <returns>True if the field wwas removed, false otherwise.</returns>
        public bool RemoveField(string name) => this.fields.Remove(name);

        /// <summary>Finds the node at the given path.</summary>
        /// <param name="names">The names to the node to find.</param>
        /// <returns>The node at the end of the path or null.</returns>
        public INode Find(params string[] names) =>
            this.Find(names as IEnumerable<string>);

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
    }
}
