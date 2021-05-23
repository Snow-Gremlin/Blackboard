using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Caps {

    /// <summary>The global namespace node for containing other named nodes.</summary>
    sealed public class Global: Node, INamespace {

        /// <summary>Create a new global namespace.</summary>
        public Global() { }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>This determines if the namespace has a child with the given name.</summary>
        /// <param name="name">The name of the child to check for.</param>
        /// <returns>True if the child exists, false otherwise.</returns>
        public bool Exists(string name) => this.Find(name) is not null;

        /// <summary>Finds the child with the given name.</summary>
        /// <param name="name">The name of the child to check for.</param>
        /// <returns>The child with the given name, otherwise null is returned.</returns>
        public INamed Find(string name) {
            foreach (INode node in this.Children) {
                if ((node is INamed named) && (named.Name == name)) return named;
            }
            return null;
        }

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>The namespace shouldn't be evaluated so this will always return nothing.</returns>
        public override IEnumerable<INode> Eval() => Enumerable.Empty<INode>();

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Global";
    }
}
