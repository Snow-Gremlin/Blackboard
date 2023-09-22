using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Types;

/// <summary>The set of extensions for working with Blackboard Types.</summary>
static public class Extensions {

    /// <summary>The type nodes for all the given nodes.</summary>
    /// <param name="nodes">The nodes to get the types for.</param>
    /// <returns>The types of the given nodes.</returns>
    static public IEnumerable<Type> Types(this IEnumerable<INode> nodes) =>
        from node in nodes select Type.TypeOf(node);

    /// <summary>The real types for all the given node types.</summary>
    /// <param name="types">The node types to get the real types for.</param>
    /// <returns>The real type for all the given types.</returns>
    static public IEnumerable<S.Type> RealTypes(this IEnumerable<Type> types) =>
        from t in types select t?.RealType ?? null;

    /// <summary>This returns the first type from the given list which the given source is assignable to.</summary>
    /// <param name="types">The types to check if the source assignable to.</param>
    /// <param name="source">The source type find the first assignable to type.</param>
    /// <returns>The first type that the source is assignable to or null if not found.</returns>
    static public Type? FirstAssignable(this IEnumerable<Type> types, S.Type source) =>
        types.FirstOrDefault((t) => source.IsAssignableTo(t.RealType));
}
