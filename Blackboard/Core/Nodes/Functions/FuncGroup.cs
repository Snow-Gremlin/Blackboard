using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Functions;

/// <summary>A collection of function signatures.</summary>
sealed public class FuncGroup : IFuncGroup {

    /// <summary>
    /// The children functions of this group.
    /// These should be function definitions.
    /// </summary>
    private readonly List<IFuncDef> definitions;

    /// <summary>Creates a new function collection.</summary>
    public FuncGroup() =>
        this.definitions = new List<IFuncDef>();

    /// <summary>Creates a new function collection.</summary>
    /// <param name="definitions">The function definitions to initially add.</param>
    public FuncGroup(params IFuncDef[] definitions) =>
        this.definitions = new List<IFuncDef>(definitions);

    /// <summary>Creates a new function collection.</summary>
    /// <param name="definitions">The function definitions to initially add.</param>
    public FuncGroup(IEnumerable<IFuncDef> definitions) =>
        this.definitions = new List<IFuncDef>(definitions);

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public INode NewInstance() => new FuncGroup();

    /// <summary>This is the type name of the node.</summary>
    public string TypeName => nameof(FuncGroup);

    /// <summary>The set of function definitions for this group.</summary>
    public IEnumerable<IFuncDef> Definitions => this.definitions;

    /// <summary>Adds function definitions onto this group.</summary>
    /// <param name="definitions">The function definitions to add.</param>
    public void Add(params IFuncDef[] definitions) =>
        this.Add(definitions as IEnumerable<IFuncDef>);

    /// <summary>Adds function definitions onto this group.</summary>
    /// <param name="definitions">The function definitions to add.</param>
    public void Add(IEnumerable<IFuncDef> definitions) =>
        this.definitions.AddRange(definitions.NotNull().WhereNot(this.definitions.Contains));

    /// <summary>Removes all the given function definitions from this node if they exist.</summary>
    /// <param name="definitions">The function definitions to remove.</param>
    public void Remove(params IFuncDef[] definitions) =>
        this.Remove(definitions as IEnumerable<IFuncDef>);

    /// <summary>Removes all the given function definitions from this node if they exist.</summary>
    /// <param name="defs">The function definitions to remove.</param>
    public void Remove(IEnumerable<IFuncDef> definitions) {
        foreach (IFuncDef definition in definitions.NotNull()) {
            int index = this.definitions.IndexOf(definition);
            if (index >= 0) this.definitions.RemoveAt(index);
        }
    }

    /// <summary>Finds and returns the best matching function in this collection.</summary>
    /// <param name="types">The input types to match against the function signatures with.</param>
    /// <returns>The best matching function or null if none match.</returns>
    public IFuncDef? Find(params Type[] types) {
        FuncMatch? minMatch = null;
        IFuncDef? minFunc = null;
        foreach (IFuncDef func in this.definitions) {
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
    public System.Type? Returns(params Type[] types) => this.Find(types)?.ReturnType;

    /// <summary>Builds and returns the function object.</summary>
    /// <param name="nodes">The nodes as parameters to the function.</param>
    /// <returns>The new node from the function or null.</returns>
    public INode? Build(params INode[] nodes) => this.Find(nodes.Select(Type.TypeOf).NotNull().ToArray())?.Build(nodes);

    /// <summary>Gets the string for this node.</summary>
    /// <returns>The debug string for this node.</returns>
    public override string ToString() => Stringifier.Simple(this);
}
