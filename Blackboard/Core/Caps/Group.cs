using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blackboard.Core.Caps {

    /// <summary>A namespace node for containing other named nodes.</summary>
    sealed public class Group: Namespace, INamed {

        /// <summary>The name for this namespace.</summary>
        private string name;

        /// <summary>The parent scope or null.</summary>
        private INamespace scope;

        /// <summary>Create a new namespace.</summary>
        /// <param name="scope">This is the initial parent scope.</param>
        public Group(INamespace scope = null) {
            this.Scope = scope;
        }

        /// <summary>Create a new namespace.</summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="scope">This is the initial parent scope.</param>
        public Group(string name, INamespace scope = null) {
            this.Name  = name;
            this.Scope = scope;
        }

        /// <summary>Gets or sets the name for the node.</summary>
        public string Name {
            get => this.name;
            set => this.name = SetName(this, value);
        }

        /// <summary>Gets or sets the containing scope for this name or null.</summary>
        public INamespace Scope {
            get => this.scope;
            set {
                CheckScopeChange(this, value);
                this.SetParent(ref this.scope, value);
            }
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.scope is not null) yield return this.scope;
            }
        }

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>The namespace shouldn't be evaluated so this will always return nothing.</returns>
        public override IEnumerable<INode> Eval() => Enumerable.Empty<INode>();

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() =>
            (this.scope is null ? "" : this.Scope.ToString()+".")+this.Name;
    }
}
