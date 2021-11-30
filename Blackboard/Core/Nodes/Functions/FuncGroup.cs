using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Functions {

    /// <summary>A collection of function signatures.</summary>
    sealed public class FuncGroup: IFuncGroup {

        /// <summary>
        /// The children functions of this group.
        /// These should be function definitions.
        /// </summary>
        private List<IFuncDef> defs;

        /// <summary>Creates a new function collection.</summary>
        /// <param name="defs">The function definitions to initially add.</param>
        public FuncGroup(params IFuncDef[] defs) =>
            this.defs = new List<IFuncDef>(defs);

        /// <summary>Creates a new function collection.</summary>
        /// <param name="defs">The function definitions to initially add.</param>
        public FuncGroup(IEnumerable<IFuncDef> defs) =>
            this.defs = new List<IFuncDef>(defs);

        /// <summary>This is the type name of the node.</summary>
        public string TypeName => "FuncGroup";

        /// <summary>The set of function definitions for this group.</summary>
        public IEnumerable<IFuncDef> Definitions => this.defs;

        /// <summary>Adds function definitions onto this group.</summary>
        /// <param name="defs">The function definitions to add.</param>
        public void AddDefs(params IFuncDef[] defs) =>
            this.AddDefs(defs as IEnumerable<IFuncDef>);

        /// <summary>Adds function definitions onto this group.</summary>
        /// <param name="defs">The function definitions to add.</param>
        public void AddDefs(IEnumerable<IFuncDef> defs) =>
            this.defs.AddRange(defs.NotNull().WhereNot(this.defs.Contains));

        /// <summary>Removes all the given function definitions from this node if they exist.</summary>
        /// <param name="defs">The function definitions to remove.</param>
        public void RemoveDefs(params IFuncDef[] defs) =>
            this.RemoveDefs(defs as IEnumerable<IFuncDef>);

        /// <summary>Removes all the given function definitions from this node if they exist.</summary>
        /// <param name="defs">The function definitions to remove.</param>
        public void RemoveDefs(IEnumerable<IFuncDef> defs) {
            foreach (IFuncDef def in defs.NotNull()) {
                int index = this.defs.IndexOf(def);
                if (index >= 0) this.defs.RemoveAt(index);
            }
        }

        /// <summary>Finds and returns the best matching function in this collection.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The best matching function or null if none match.</returns>
        public IFuncDef Find(params Type[] types) {
            FuncMatch minMatch = null;
            IFuncDef minFunc = null;
            foreach (IFuncDef func in this.defs) {
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

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
