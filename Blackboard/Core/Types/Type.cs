// Ignore Spelling: Implicits

using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Types;

/// <summary>The types implemented for Blackboard.</summary>
sealed public class Type {

    /// <summary>This is a delegate for a casting method.</summary>
    /// <param name="node">The node to cast.</param>
    /// <returns>The resulting casted value.</returns>
    private delegate INode? caster(INode node);

    /// <summary>The base type for all other types, not just value types.</summary>
    static public readonly Type Node;

    /// <summary>The trigger type.</summary>
    static public readonly Type Trigger;

    /// <summary>The base of all value types</summary>
    static public readonly Type Object;

    /// <summary>The boolean value type.</summary>
    static public readonly Type Bool;

    /// <summary>The integer value type.</summary>
    static public readonly Type Int;

    /// <summary>The double value type.</summary>
    static public readonly Type Double;

    /// <summary>The string value type.</summary>
    static public readonly Type String;

    /// <summary>The Namespace value type.</summary>
    static public readonly Type Namespace;

    /// <summary>The function group value type.</summary>
    /// <remarks>
    /// A function group contains several function definitions
    /// and can select a definition based on parameter type.
    /// </remarks>
    static public readonly Type FuncGroup;

    /// <summary>The function definition value type.</summary>
    /// <remarks>A function definition is a single implementation with specific parameter types.</remarks>
    static public readonly Type FuncDef;

    /// <summary>The integer counter type which is an extension of the integer value type.</summary>
    static public readonly Type CounterInt;

    /// <summary>The double counter type which is an extension of the double value type.</summary>
    static public readonly Type CounterDouble;

    /// <summary>The Toggler type which is an extension of the boolean value type.</summary>
    static public readonly Type Toggler;

    /// <summary>The object latch which is an extension of the object value type.</summary>
    static public readonly Type LatchObject;

    /// <summary>The boolean latch which is an extension of the boolean value type.</summary>
    static public readonly Type LatchBool;

    /// <summary>The integer latch which is an extension of the integer value type.</summary>
    static public readonly Type LatchInt;

    /// <summary>The double latch which is an extension of the double value type.</summary>
    static public readonly Type LatchDouble;

    /// <summary>The string latch which is an extension of the string value type.</summary>
    static public readonly Type LatchString;

    /// <summary>Gets all the types.</summary>
    /// <remarks>These are ordered by inheriting object before the object that was inherited.</remarks>
    static public IEnumerable<Type> AllTypes => Node.AllInheritors.Append(Node);

    /// <summary>Finds the type given the type name.</summary>
    /// <param name="name">The name of the type to get.</param>
    /// <returns>The type for the given name or null if name isn't found.</returns>
    static public Type? FromName(string name) =>
        AllTypes.Where((t) => t.Name == name).FirstOrDefault();

    /// <summary>This gets the type given a node.</summary>
    /// <param name="node">The node to get the type of.</param>
    /// <returns>The type for the given node or null if not found.</returns>
    static public Type? TypeOf(INode node) => FromType(node.GetType());

    /// <summary>This gets the type from the given generic.</summary>
    /// <typeparam name="T">The generic type to get the type of.</typeparam>
    /// <returns>The type for the given generic or null if not found.</returns>
    static public Type? FromType<T>() where T : INode => FromType(typeof(T));

    /// <summary>This get the type from the given C# type.</summary>
    /// <param name="type">The C# type to get this type of.</param>
    /// <returns>The type for the given C# type or null if not found.</returns>
    static public Type? FromType(S.Type type) {
        if (!type.IsAssignableTo(Node.RealType)) return null;
        Type current = Node;
        while (true) {
            Type? next = current.Inheritors.FirstAssignable(type);
            if (next is null) return current;
            current = next;
        }
    }

    /// <summary>This determines the implicit and inheritance match.</summary>
    /// <param name="input">This is the type to cast from.</param>
    /// <param name="output">This is the type to cast too.</param>
    /// <returns>The result of the match.</returns>
    static public TypeMatch Match(Type input, Type output) => output.Match(input);

    /// <summary>The display name of the type.</summary>
    public readonly string Name;

    /// <summary>The C# type for this type.</summary>
    public readonly S.Type RealType;

    /// <summary>This is the base type this type inherits from.</summary>
    /// <remarks>This is only null for Node type since it doesn't inherit from anything.</remarks>
    public readonly Type? BaseType;

    /// <summary>The underlying IData type for this node, or null if no data.</summary>
    public readonly S.Type? DataType;
        
    /// <summary>The types with base type of this type.</summary>
    private readonly List<Type> inheritors;

    /// <summary>This is a dictionary of other types to an implicit cast.</summary>
    private readonly Dictionary<Type, caster> imps;

    /// <summary>This is a dictionary of other types to an explicit cast.</summary>
    private readonly Dictionary<Type, caster> exps;

    /// <summary>This creates a new type.</summary>
    /// <param name="name">The name of the type.</param>
    /// <param name="realType">The C# type for this type.</param>
    /// <param name="baseType">The base type this type inherits from.</param>
    /// <param name="dataType">The IData type underlying this type.</param>
    private Type(string name, S.Type realType, Type? baseType, S.Type? dataType) {
        this.Name = name;
        this.RealType = realType;
        this.BaseType = baseType;
        this.DataType = dataType;
        this.imps = new();
        this.exps = new();
        this.inheritors = new();

        if ((dataType is null) == realType.IsAssignableTo(typeof(IDataNode)))
            throw new Message("May not have null inherited data type with out a valid read type and visa versa.").
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
    /// <param name="node">The node to try casting from.</param>
    /// <returns>The result of the match.</returns>
    static public TypeMatch Match<T>(INode node) where T : INode =>
        Match<T>(TypeOf(node));

    /// <summary>This determines the implicit and inheritance match.</summary>
    /// <param name="t">The type to try casting from.</param>
    /// <returns>The result of the match.</returns>
    static public TypeMatch Match<T>(Type? t) where T : INode =>
        FromType<T>()?.Match(t) ?? TypeMatch.NoMatch;

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

    /// <summary>The types which this type can directly implicitly cast into.</summary>
    public IReadOnlyCollection<Type> Implicits => this.imps.Keys;

    /// <summary>The types which this type can directly explicitly cast into.</summary>
    public IReadOnlyCollection<Type> Explicits => this.exps.Keys;

    /// <summary>Performs an implicit cast of the given node into this type.</summary>
    /// <param name="node">The node to implicitly cast.</param>
    /// <returns>The node cast into this type or null if the cast is not possible.</returns>
    static public T? Implicit<T>(INode node) where T : class, INode =>
        FromType<T>()?.Implicit(node) as T;

    /// <summary>Performs an implicit cast of the given node into this type.</summary>
    /// <param name="node">The node to implicitly cast.</param>
    /// <returns>The node cast into this type or null if the cast is not possible.</returns>
    public INode? Implicit(INode node) =>
        cast(true, node, TypeOf(node), this);

    /// <summary>Performs an explicit cast of the given node into this type.</summary>
    /// <param name="node">The node to explicitly cast.</param>
    /// <returns>The node cast into this type or null if the cast is not possible.</returns>
    static public T? Explicit<T>(INode node) where T : class, INode =>
        FromType<T>()?.Explicit(node) as T;

    /// <summary>Performs an explicit cast of the given node into this type.</summary>
    /// <param name="node">The node to explicitly cast.</param>
    /// <returns>The node cast into this type or null if the cast is not possible.</returns>
    public INode? Explicit(INode node) =>
        cast(false, node, TypeOf(node), this);

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

    /// <summary>Adds a cast-ability definition to a type.</summary>
    /// <typeparam name="T">The node type for the source type to cast from.</typeparam>
    /// <param name="dict">Either the implicit or explicit dictionary for the type being added to.</param>
    /// <param name="dest">The destination type to cast to.</param>
    /// <param name="func">The function for performing the cast.</param>
    static private void addCast<T>(Dictionary<Type, caster> dict, Type dest, S.Func<T, INode> func) where T : INode =>
        dict[dest] = (INode input) => input is T value ? func(value) : null;

    /// <summary>Initializes the types before they are used.</summary>
    static Type() {
        Node          = new Type("node",           typeof(INode),           null,   null);
        Trigger       = new Type("trigger",        typeof(ITrigger),        Node,   null);
        Object        = new Type("object",         typeof(IValue<Object>),  Node,   typeof(Object));
        Bool          = new Type("bool",           typeof(IValue<Bool>),    Node,   typeof(Bool));
        Int           = new Type("int",            typeof(IValue<Int>),     Node,   typeof(Int));
        Double        = new Type("double",         typeof(IValue<Double>),  Node,   typeof(Double));
        String        = new Type("string",         typeof(IValue<String>),  Node,   typeof(String));
        Namespace     = new Type("namespace",      typeof(Namespace),       Node,   null);
        FuncGroup     = new Type("function-group", typeof(FuncGroup),       Node,   null);
        FuncDef       = new Type("function-def",   typeof(IFuncDef),        Node,   null);
        CounterInt    = new Type("counter-int",    typeof(Counter<Int>),    Int,    typeof(Int));
        CounterDouble = new Type("counter-double", typeof(Counter<Double>), Double, typeof(Double));
        Toggler       = new Type("toggler",        typeof(Toggler),         Bool,   typeof(Bool));
        LatchObject   = new Type("latch-object",   typeof(Latch<Object>),   Object, typeof(Object));
        LatchBool     = new Type("latch-bool",     typeof(Latch<Bool>),     Bool,   typeof(Bool));
        LatchInt      = new Type("latch-int",      typeof(Latch<Int>),      Int,    typeof(Int));
        LatchDouble   = new Type("latch-double",   typeof(Latch<Double>),   Double, typeof(Double));
        LatchString   = new Type("latch-string",   typeof(Latch<String>),   String, typeof(String));

        addCast<IValueParent<Bool>>(Bool.imps, Trigger, (input) => new BoolAsTrigger(input));
        addCast<IValueParent<Bool>>(Bool.imps, Object,  (input) => new Implicit<Bool, Object>(input));
        addCast<IValueParent<Bool>>(Bool.imps, String,  (input) => new Implicit<Bool, String>(input));

        addCast<IValueParent<Int>>(Int.imps, Object, (input) => new Implicit<Int, Object>(input));
        addCast<IValueParent<Int>>(Int.imps, Double, (input) => new Implicit<Int, Double>(input));
        addCast<IValueParent<Int>>(Int.imps, String, (input) => new Implicit<Int, String>(input));

        addCast<IValueParent<Double>>(Double.imps, Object, (input) => new Implicit<Double, Object>(input));
        addCast<IValueParent<Double>>(Double.exps, Int,    (input) => new Explicit<Double, Int>(input));
        addCast<IValueParent<Double>>(Double.imps, String, (input) => new Implicit<Double, String>(input));
            
        addCast<IValueParent<Object>>(Object.exps, Bool,   (input) => new Explicit<Object, Bool>(input));
        addCast<IValueParent<Object>>(Object.exps, Int,    (input) => new Explicit<Object, Int>(input));
        addCast<IValueParent<Object>>(Object.exps, Double, (input) => new Explicit<Object, Double>(input));
        addCast<IValueParent<Object>>(Object.imps, String, (input) => new Implicit<Object, String>(input));

        addCast<IValueParent<String>>(String.imps, Object, (input) => new Implicit<String, Object>(input));

        addCast<IFuncDef>(FuncDef.imps, FuncGroup, (input) => new FuncGroup(input));
    }
}
