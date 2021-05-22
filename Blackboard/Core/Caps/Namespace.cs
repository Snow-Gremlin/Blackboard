﻿using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blackboard.Core.Caps {

    /// <summary>A namespace node for containing other named nodes.</summary>
    sealed public class Namespace: Node, INamespace, INamed {

        /// <summary>The regex singleton for validating the name.</summary>
        static private Regex nameRegex = null;

        /// <summary>Get the name validator regex.</summary>
        static private Regex NameValidator {
            get {
                if (nameRegex is null)
                    nameRegex = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]*$", RegexOptions.Compiled);
                return nameRegex;
            }
        }

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

        /// <summary>
        /// This finds the name or location a name should
        /// be in the children of the given namespace.
        /// </summary>
        /// <param name="nodes">The sorted set of nodes to search within.</param>
        /// <param name="name">The name to find the location for.</param>
        /// <param name="found">The node found or null if not found.</param>
        /// <returns>The index the name should go or was found.</returns>
        static private int findPossibleIndex(List<INode> nodes, string name, out INode found) {
            int low = 0;
            int high = nodes.Count - 1;
            while (low <= high) {
                int mid = (low + high) / 2;
                INamed node = nodes[mid] as INamed;
                int comp = node.Name.CompareTo(name);

                if (comp == 0) {
                    found = node;
                    return mid;
                }  else if (comp > 0) low = mid + 1;
                else high = mid - 1;
            }
            found = null;
            return low;
        }

        /// <summary>The name for this namespace.</summary>
        private string name;

        /// <summary>The parent scope or null.</summary>
        private INamespace scope;

        /// <summary>Create a new namespace.</summary>
        /// <param name="scope">This is the initial parent scope.</param>
        public Namespace(INamespace scope = null) {
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

        /// <summary>This determines if the namespace has a child with the given name.</summary>
        /// <param name="name">The name of the child to check for.</param>
        /// <returns>True if the child exists, false otherwise.</returns>
        public bool Exists(string name) => !(this.Find(name) is null);

        /// <summary>Finds the child with the given name.</summary>
        /// <param name="name">The name of the child to check for.</param>
        /// <returns>The child with the given name, otherwise null is returned.</returns>
        public INamed Find(string name) {
            foreach (INode node in this.Children) {
                if (node is INamed) {
                    INamed named = node as INamed;
                    if (named.Name == name) return named;
                }
            }
            return null;
        }

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>The namespace shouldn't be evaluated so this will always return nothing.</returns>
        public override IEnumerable<INode> Eval() => Enumerable.Empty<INode>();
    }
}
