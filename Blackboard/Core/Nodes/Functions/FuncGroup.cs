﻿using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Functions {

    /// <summary>A collection of function signatures.</summary>
    public class FuncGroup: IFuncGroup, IAdopter {

        /// <summary>
        /// The children functions of this group.
        /// These should be function definitions.
        /// </summary>
        private List<IFuncDef> children;

        /// <summary>Creates a new function collection.</summary>
        /// <param name="defs">The function definitions to initially add.</param>
        public FuncGroup(params IFuncDef[] defs) {
            this.children = new List<IFuncDef>(defs);
        }

        /// <summary>Creates a new function collection.</summary>
        /// <param name="defs">The function definitions to initially add.</param>
        public FuncGroup(IEnumerable<IFuncDef> defs) {
            this.children = new List<IFuncDef>(defs);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>The set of children nodes to this node in the graph.</summary>
        public IEnumerable<INode> Children => this.children;

        /// <summary>Adds children nodes onto this node.</summary>
        /// <remarks>This will always check for loops.</remarks>
        /// <param name="children">The children to add.</param>
        public void AddChildren(params INode[] children) =>
            this.AddChildren(children as IEnumerable<INode>);

        /// <summary>Adds children nodes onto this node.</summary>
        /// <param name="children">The children to add.</param>
        /// <param name="checkedForLoops">Indicates if loops in the graph should be checked for.</param>
        public void AddChildren(IEnumerable<INode> children, bool checkedForLoops = true) {
            if (checkedForLoops && INode.CanReachAny(this, children))
                throw Exception.NodeLoopDetected();
            foreach (IFuncDef child in children) {
                if (child is not null && !this.children.Contains(child))
                    this.children.Add(child);
            }
        }

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(params INode[] children) =>
            this.RemoveChildren(children as IEnumerable<INode>);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(IEnumerable<INode> children) {
            foreach (IFuncDef child in children) {
                if (child is not null) {
                    int index = this.children.IndexOf(child);
                    if (index >= 0) this.children.RemoveAt(index);
                }
            }
        }

        /// <summary>Finds and returns the best matching function in this collection.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The best matching function or null if none match.</returns>
        public IFuncDef Find(params Type[] types) {
            FuncMatch minMatch = null;
            IFuncDef minFunc = null;
            foreach (IFuncDef func in this.children) {
                FuncMatch match = func.Match(types);
                if (!match.IsMatch) continue;

                if (minMatch is null || minMatch.CompareTo(match) > 0) {
                    minMatch = match;
                    minFunc = func;
                }
            }
            return minFunc;
        }

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="types">The types of the parameters being passed into the function.</param>
        /// <returns>The new node from the function or null.</returns>
        public System.Type Returns(params Type[] types) => this.Find(types)?.ReturnType;

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new node from the function or null.</returns>
        public INode Build(params INode[] nodes) => this.Find(nodes.Select(Type.TypeOf).ToArray())?.Build(nodes);
    }
}