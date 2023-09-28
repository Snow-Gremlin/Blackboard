// Ignore Spelling: Implicits

using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Types;

/// <summary>The types implemented for Blackboard.</summary>
sealed internal partial class Type {

    /// <summary>This is a delegate for a casting method.</summary>
    /// <param name="node">The node to cast.</param>
    /// <returns>The resulting casted value.</returns>
    private delegate INode? caster(INode node);

    /// <summary>The display name of the type.</summary>
    public readonly string Name;

    /// <summary>The C# type for this node type.</summary>
    /// <remarks>This is type for nodes like `IValue~Int~`.</remarks>
    public readonly S.Type RealType;

    /// <summary>This is the base type this type inherits from.</summary>
    /// <remarks>This is only null for Node type since it doesn't inherit from anything.</remarks>
    public readonly Type? BaseType;

    /// <summary>The underlying IData type for this node, or null if no data.</summary>
    /// <remarks>This is types like an `Int` IData inside of an `IValue~Int~` node.</remarks>
    public readonly S.Type? DataType;
    
    /// <summary>The underlying C# data type for the IData of the node, or null if no data value.</summary>
    /// <remarks>This is types like C# `int` for an `Int` IData inside of an `IValue~Int~` node.</remarks>
    public readonly S.Type? ValueType;

    /// <summary>The types with base type of this type.</summary>
    private readonly List<Type> inheritors;

    /// <summary>This is a dictionary of other types to an implicit cast.</summary>
    private readonly Dictionary<Type, caster> imps;

    /// <summary>This is a dictionary of other types to an explicit cast.</summary>
    private readonly Dictionary<Type, caster> exps;

    /// <summary>This creates a new type.</summary>
    /// <param name="name">The name of the type.</param>
    /// <param name="realType">The C# type for this node type.</param>
    /// <param name="baseType">The base type this type inherits from.</param>
    /// <param name="dataType">The IData type underlying this type.</param>
    /// <param name="valueType">The C# data type for the IData of the node.</param>
    private Type(string name, S.Type realType, Type? baseType = null, S.Type? dataType = null, S.Type? valueType = null) {
        this.Name       = name;
        this.RealType   = realType;
        this.BaseType   = baseType;
        this.DataType   = dataType;
        this.ValueType  = valueType;
        this.imps       = new();
        this.exps       = new();
        this.inheritors = new();

        if ((dataType is null) == realType.IsAssignableTo(typeof(IDataNode)))
            throw new BlackboardException("May not have null inherited data type with out a valid read type and visa versa.").
                With("Name",     name).
                With("RealType", realType).
                With("DataType", dataType);
        baseType?.inheritors.Add(this);
    }

    /// <summary>
    /// Checks if inheritance can be used for this given type.
    /// Finds the closest inherited type so that the most specific match can be chosen.
    /// </summary>
    /// <param name="t">The type to check inheritance against.</param>
    /// <returns>The inheritance match steps or -1 if no inheritance match.</returns>
    private int inheritMatchSteps(Type t) {
        int steps = -1;
        if (t.RealType.IsAssignableTo(this.RealType)) {
            Type? nt = t;
            do {
                nt = nt.BaseType;
                steps++;
            } while (nt is not null && nt.RealType.IsAssignableTo(this.RealType));
        }
        return steps;
    }

    /// <summary> 
    /// Checks if an implicit or explicit cast for the given type.
    /// Adds an initial penalty for using an implicit cast instead of inheritance.
    /// </summary>
    /// <param name="exp">Indicates to check explicit, otherwise implicit.</param>
    /// <param name="t">The type to check cast against.</param>
    /// <returns>The implicit or explicit cast steps or -1 if no cast match.</returns>
    private int castMatchSteps(bool exp, Type t) {
        int steps = 0;
        Type? nt = t;
        do {
            Dictionary<Type, caster> dict = exp ? nt.exps : nt.imps;
            if (dict.ContainsKey(this)) return steps;
            nt = nt.BaseType;
            steps++;
        } while (nt is not null);
        return -1;
    }

    /// <summary>This determines the implicit and inheritance match.</summary>
    /// <param name="t">The type to try casting from.</param>
    /// <param name="explicitCasts">
    /// True to also check if it can be cast explicitly, false to ignore explicit casts.
    /// </param>
    /// <returns>The result of the match.</returns>
    public TypeMatch Match(Type? t, bool explicitCasts = false) {
        if (t is null) return TypeMatch.NoMatch;

        int steps = this.inheritMatchSteps(t);
        if (steps >= 0) return TypeMatch.Inherit(steps);

        steps = this.castMatchSteps(false, t);
        if (steps >= 0) return TypeMatch.Implicit(steps);

        if (explicitCasts) {
            steps = this.castMatchSteps(true, t);
            if (steps >= 0) return TypeMatch.Explicit();
        }

        return TypeMatch.NoMatch;
    }

    /// <summary>The types with base type of this type.</summary>
    public IReadOnlyCollection<Type> Inheritors => this.inheritors.AsReadOnly();

    /// <summary>Gets the depth first collection of all types which inherit from this type.</summary>
    public IEnumerable<Type> AllInheritors {
        get {
            foreach (Type inheritor in this.inheritors) {
                foreach (Type decendent in inheritor.AllInheritors)
                    yield return decendent;
                yield return inheritor;
            }
        }
    }

    /// <summary>Determines if this type is the given type or an inheritor of the given type.</summary>
    /// <remarks>This returns true for things like an int counter being checked against an int.</remarks>
    /// <param name="other">The other type to check if inherited.</param>
    /// <returns>True if this is an inheritor of the given type.</returns>
    public bool IsInheritorOf(Type other) =>
        this == other || other.AllInheritors.Contains(this);

    /// <summary>The types which this type can directly implicitly cast into.</summary>
    public IReadOnlyCollection<Type> Implicits => this.imps.Keys;

    /// <summary>The types which this type can directly explicitly cast into.</summary>
    public IReadOnlyCollection<Type> Explicits => this.exps.Keys;

    /// <summary>Performs an implicit cast of the given node into this type.</summary>
    /// <param name="node">The node to implicitly cast.</param>
    /// <returns>The node cast into this type or null if the cast is not possible.</returns>
    public INode? Implicit(INode node) => cast(true, node, TypeOf(node), this);

    /// <summary>Performs an explicit cast of the given node into this type.</summary>
    /// <param name="node">The node to explicitly cast.</param>
    /// <returns>The node cast into this type or null if the cast is not possible.</returns>
    public INode? Explicit(INode node) => cast(false, node, TypeOf(node), this);

    /// <summary>Performs a cast of the given node into the destination type.</summary>
    /// <param name="imp">True for implicitly cast, false for explicitly cast.</param>
    /// <param name="node">The node to cast.</param>
    /// <param name="src">The type of the given node.</param>
    /// <param name="dest">The destination type to cast into.</param>
    /// <returns>The node cast into the destination type or null if it can not be cast.</returns>
    static private INode? cast(bool imp, INode node, Type? src, Type dest) {
        if (src is null) return null;
        if (src.RealType.IsAssignableTo(dest.RealType)) return node;
        do {
            Dictionary<Type, caster> dict = imp ? src.imps : src.exps;
            if (dict.TryGetValue(dest, out caster? func)) return func(node);
            src = src.BaseType;
        } while (src is not null);
        return null;
    }

    /// <summary>Indicates if this type has a data type underlying it.</summary>
    /// <remarks>
    /// Any node which has a data type should be a literal or able to be converted
    /// into a constant value, and extend IDataNode.
    /// </remarks>
    public bool HasData => this.DataType is not null;

    /// <summary>Checks the equality of these two types.</summary>
    /// <param name="left">The left type in the equality.</param>
    /// <param name="right">The right type in the equality.</param>
    /// <returns>True if they are equal, false otherwise.</returns>
    public static bool operator ==(Type left, Type right) => ReferenceEquals(left, right);

    /// <summary>Checks if of these two types are not equal.</summary>
    /// <param name="left">The left type in the equality.</param>
    /// <param name="right">The right type in the equality.</param>
    /// <returns>True if they are not equal, false otherwise.</returns>
    public static bool operator !=(Type left, Type right) => !ReferenceEquals(left, right);

    /// <summary>Checks if the given object is equal to this type.</summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if they are equal, false otherwise.</returns>
    public override bool Equals(object? obj) => ReferenceEquals(this, obj);

    /// <summary>Gets the hash code for this type.</summary>
    /// <returns>The type's name hash code.</returns>
    public override int GetHashCode() => this.Name.GetHashCode();

    /// <summary>Gets the name of the type.</summary>
    /// <returns>The name of the type.</returns>
    public override string ToString() => this.Name;
}
