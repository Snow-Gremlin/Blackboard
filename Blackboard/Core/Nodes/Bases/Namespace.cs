using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>A node for containing named nodes sorted by name.</summary>
    public abstract class Namespace: INode, INamespace {

        /// <summary>The regex singleton for validating the name.</summary>
        static private Regex nameRegex = null;

        /// <summary>Get the name validator regex.</summary>
        static private Regex NameValidator =>
            nameRegex ??= new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]*$", RegexOptions.Compiled);

        /// <summary>Checks if the given node is a valid name.</summary>
        /// <param name="name">The name to check.</param>
        /// <returns>True if the name is valis, false otherwise.</returns>
        static public bool ValidName(string name) => NameValidator.IsMatch(name);

        /// <summary>This is used to set the name of the given named while checking the name.</summary>
        /// <param name="named">This is the named node to name.</param>
        /// <param name="name">This is the new name to set.</param>
        static internal string SetName(INamed named, string name) {
            if (named.Name == name) return named.Name;
            if (!ValidName(name))
                throw Exception.InvalidName(name);
            if (named?.Scope?.Exists(name) ?? false)
                throw Exception.RenameDuplicateInScope(name, named.Scope);
            // Name is valid an unique, return it to be set.
            return name;
        }

        /// <summary>This checks that the new scope can take on a node with the given name.</summary>
        /// <param name="named">The node the scope is being added to.</param>
        /// <param name="scope">The new scope being set, may be null.</param>
        static internal void CheckScopeChange(INamed named, INamespace scope) {
            string name = named.Name;
            if (scope?.Exists(name) ?? false)
                throw Exception.RenameDuplicateInScope(name, scope);
        }

        /// <summary>The collection of children nodes to this node.</summary>
        private SortedDictionary<string, INamed> children;

        /// <summary>Creates a new node.</summary>
        protected Namespace() {
            this.children = new SortedDictionary<string, INamed>();
            this.Depth = 0;
        }

        /// <summary>The depth in the graph from the furthest input of this node.</summary>
        public int Depth { get; private set; }

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>
        /// The set of children that should be updated based on the results of this update.
        /// If this evaluation made no change then typically no children will be returned.
        /// Usually the entire set of children are returned on change, but it is not required.
        /// </returns>
        public abstract IEnumerable<INode> Eval();

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public abstract IEnumerable<INode> Parents { get; }

        /// <summary>The set of children nodes to this node in the graph.</summary>
        public IEnumerable<INode> Children => this.children.Values;

        /// <summary>This determines if the namespace has a child with the given name.</summary>
        /// <param name="name">The name of the child to check for.</param>
        /// <returns>True if the child exists, false otherwise.</returns>
        public bool Exists(string name) => this.children.ContainsKey(name);

        /// <summary>Finds the child with the given name.</summary>
        /// <param name="name">The name of the child to check for.</param>
        /// <returns>The child with the given name, otherwise null is returned.</returns>
        public INamed Find(string name) =>
            this.children.TryGetValue(name, out INamed node) ? node : null;

        /// <summary>This is a helper method for setting a parent to the node.</summary>
        /// <typeparam name="T">The node type for the parent.</typeparam>
        /// <param name="parent">The parent variable being set.</param>
        /// <param name="newParent">The new parent being set, or null</param>
        protected void SetParent<T>(ref T parent, T newParent) where T : INode {
            if (ReferenceEquals(parent, newParent)) return;
            parent?.RemoveChildren(this);
            parent = newParent;
            newParent?.AddChildren(this);
        }

        /// <summary>Adds children nodes onto this node.</summary>
        /// <remarks>This will always check for loops.</remarks>
        /// <param name="children">The children to add.</param>
        public void AddChildren(params INode[] children) =>
            this.AddChildren(children as IEnumerable<INode>);

        /// <summary>Adds children nodes onto this node.</summary>
        /// <param name="children">The children to add.</param>
        /// <param name="checkedForLoops">Indicates if loops in the graph should be checked for.</param>
        public void AddChildren(IEnumerable<INode> children, bool checkedForLoops = true) {
            if (checkedForLoops && Node.CanReachAny(this, children))
                throw Exception.NodeLoopDetected();
            LinkedList<INode> needsDepthUpdate = new();
            foreach (Node child in children) {
                if (child is not null && child is INamed named) {
                    if (!this.children.ContainsKey(named.Name)) {
                        this.children[named.Name] = named;
                        needsDepthUpdate.SortInsertUnique(named);
                    }
                }
            }
            Node.UpdateDepths(needsDepthUpdate);
        }

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(params INode[] children) =>
            this.RemoveChildren(children as IEnumerable<INode>);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(IEnumerable<INode> children) {
            LinkedList<INode> needsDepthUpdate = new();
            foreach (Node child in children) {
                if (child is not null && child is INamed named) {
                    if (this.children.Remove(named.Name))
                        needsDepthUpdate.SortInsertUnique(child);
                }
            }
            Node.UpdateDepths(needsDepthUpdate);
        }
    }
}
