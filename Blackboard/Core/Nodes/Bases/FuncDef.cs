using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>
    /// This is the base function node information for all non-group funtion types.
    /// This represents a single function definition with a specific signature.
    /// </summary>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>
    public abstract class FuncDef<TReturn>: IFuncDef
        where TReturn : class, INode {

        /// <summary>The group this function belongs to.</summary>
        private FuncGroup group;

        /// <summary>Creates a new non-group function base.</summary>
        /// <remarks>By default no group is defined for this function.</remarks>
        protected FuncDef() {
            this.group = null;
        }

        /// <summary>Gets or sets the group this function belongs to.</summary>
        public FuncGroup Group {
            get => this.group;
            set {
                if (ReferenceEquals(this.group, value)) return;
                this.group?.RemoveChildren(this);
                this.group = value;
                this.group?.AddChildren(this);
            }
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<INode> Parents {
            get {
                if (this.Group is not null) yield return this.Group;
            }
        }

        /// <summary>The set of children nodes to this node in the graph.</summary>
        public IEnumerable<INode> Children => Enumerable.Empty<INode>();

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The matching results for this function.</returns>
        public abstract FuncMatch Match(Type[] types);

        /// <summary>Returns the type that would be return if built.</summary>
        /// <returns>The type which would be returned.</returns>
        public System.Type ReturnType => typeof(TReturn);

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public abstract INode Build(INode[] nodes);
    }
}
