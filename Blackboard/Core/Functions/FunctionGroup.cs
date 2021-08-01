using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Functions {

    /// <summary>A collection of function signatures. Typically these should all have the same name.</summary>
    public class FunctionGroup: List<IFunction> {

        /// <summary>Creates a new function collection.</summary>
        /// <param name="funcs">The functions to initially add.</param>
        public FunctionGroup(params IFunction[] funcs) : base(funcs) { }

        /// <summary>Creates a new function collection.</summary>
        /// <param name="funcs">The functions to initially add.</param>
        public FunctionGroup(IEnumerable<IFunction> funcs) : base(funcs) { }

        /// <summary>Finds and returns the best matching function in this collection.</summary>
        /// <param name="nodes">The input to match against the function signatures with.</param>
        /// <returns>The best matching function or null if none match.</returns>
        public IFunction Find(params INode[] nodes) {
            FuncMatch minMatch = null;
            IFunction minFunc = null;
            foreach (IFunction func in this) {
                FuncMatch match = func.Match(nodes);
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
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new node from the function or null.</returns>
        public INode Build(params INode[] nodes) => this.Find(nodes)?.Build(nodes);
    }
}
