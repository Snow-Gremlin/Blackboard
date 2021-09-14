using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using Blackboard.Core.Functions;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>A collection of function signatures. Typically these should all have the same name.</summary>
    public class FuncGroup: List<IFunction>, INode {

        /// <summary>Creates a new function collection.</summary>
        /// <param name="funcs">The functions to initially add.</param>
        public FuncGroup(params IFunction[] funcs) : base(funcs) { }

        /// <summary>Creates a new function collection.</summary>
        /// <param name="funcs">The functions to initially add.</param>
        public FuncGroup(IEnumerable<IFunction> funcs) : base(funcs) { }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>The set of children nodes to this node in the graph.</summary>
        public IEnumerable<INode> Children => Enumerable.Empty<INode>();

        /// <summary>Finds and returns the best matching function in this collection.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The best matching function or null if none match.</returns>
        public IFunction Find(params Type[] types) {
            FuncMatch minMatch = null;
            IFunction minFunc = null;
            foreach (IFunction func in this) {
                FuncMatch match = func.Match(types);
                if (!match.IsMatch) continue;

                if (minMatch is null) {
                    minMatch = match;
                    minFunc = func;
                    continue;
                }

                if (minMatch.CompareTo(match) > 0) {
                    minMatch = match;
                    minFunc = func;
                }
            }
            return minFunc;
        }

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="types">The types of the parameters being passed into the function.</param>
        /// <returns>The new node from the function or null.</returns>
        public Type Returns(params Type[] types) => this.Find(types)?.Returns();

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new node from the function or null.</returns>
        public INode Build(params INode[] nodes) => this.Find(nodes.Select(Type.TypeOf).ToArray())?.Build(nodes);
    }
}
