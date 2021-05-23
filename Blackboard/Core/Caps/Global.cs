using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Caps {

    /// <summary>The global namespace node for containing other named nodes.</summary>
    sealed public class Global: Namespace {

        /// <summary>Create a new global namespace.</summary>
        public Global() { }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>The namespace shouldn't be evaluated so this will always return nothing.</returns>
        public override IEnumerable<INode> Eval() => Enumerable.Empty<INode>();

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Global";
    }
}
