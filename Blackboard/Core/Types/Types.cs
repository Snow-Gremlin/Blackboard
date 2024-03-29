﻿using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Types;

partial class Type {

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

    /// <summary>The unsigned integer value type.</summary>
    static public readonly Type Uint;

    /// <summary>The double value type.</summary>
    static public readonly Type Double;
    
    /// <summary>The float value type.</summary>
    static public readonly Type Float;

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

    #region Lookup

    /// <summary>Gets all the types.</summary>
    /// <remarks>These are ordered by inheriting object before the object that was inherited.</remarks>
    static public IEnumerable<Type> AllTypes => Node.AllInheritors.Append(Node);

    /// <summary>Finds the type given the type name.</summary>
    /// <param name="name">The name of the type to get.</param>
    /// <returns>The type for the given name or null if name isn't found.</returns>
    static public Type? FromName(string name) =>
        AllTypes.Where(t => t.Name == name).FirstOrDefault();

    /// <summary>This gets the type given a node.</summary>
    /// <param name="node">The node to get the type of.</param>
    /// <returns>The type for the given node or null if not found.</returns>
    static internal Type? TypeOf(INode node) => FromType(node.GetType());

    /// <summary>This gets the type from the given node type.</summary>
    /// <typeparam name="T">The generic type to get the type of.</typeparam>
    /// <returns>The type for the given generic or null if not found.</returns>
    static internal Type? FromType<T>() where T : INode => FromType(typeof(T));

    /// <summary>This gets the type from the given node type.</summary>
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
    
    /// <summary>This gets the type given the C# base value type.</summary>
    /// <param name="type">The C# value type, like `int`.</param>
    /// <returns>The type for the given C# value type, like `Int`.</returns>
    static public Type? FromValueType(S.Type type) =>
        AllTypes.Where(t => t.ValueType == type).FirstOrDefault();

    /// <summary>This determines the implicit and inheritance match.</summary>
    /// <param name="input">This is the type to cast from.</param>
    /// <param name="output">This is the type to cast too.</param>
    /// <returns>The result of the match.</returns>
    static public TypeMatch Match(Type input, Type output) =>
        output.Match(input);
    
    /// <summary>This determines the implicit and inheritance match.</summary>
    /// <param name="node">The node to try casting from.</param>
    /// <returns>The result of the match.</returns>
    static internal TypeMatch Match<T>(INode node) where T : INode =>
        Match<T>(TypeOf(node));

    /// <summary>This determines the implicit and inheritance match.</summary>
    /// <param name="t">The type to try casting from.</param>
    /// <returns>The result of the match.</returns>
    static internal TypeMatch Match<T>(Type? t) where T : INode =>
        FromType<T>()?.Match(t) ?? TypeMatch.NoMatch;

    /// <summary>Performs an implicit cast of the given node into this type.</summary>
    /// <param name="node">The node to implicitly cast.</param>
    /// <returns>The node cast into this type or null if the cast is not possible.</returns>
    static internal T? Implicit<T>(INode node) where T : class, INode =>
        FromType<T>()?.Implicit(node) as T;

    /// <summary>Performs an explicit cast of the given node into this type.</summary>
    /// <param name="node">The node to explicitly cast.</param>
    /// <returns>The node cast into this type or null if the cast is not possible.</returns>
    static internal T? Explicit<T>(INode node) where T : class, INode =>
        FromType<T>()?.Explicit(node) as T;

    #endregion
    #region Initialization

    /// <summary>Initializes the types before they are used.</summary>
    static Type() {
        Node          = new Type("node",           typeof(INode));
        Trigger       = new Type("trigger",        typeof(ITrigger),        Node);
        Object        = new Type("object",         typeof(IValue<Object>),  Node,   typeof(Object), typeof(object));
        Bool          = new Type("bool",           typeof(IValue<Bool>),    Node,   typeof(Bool),   typeof(bool));
        Int           = new Type("int",            typeof(IValue<Int>),     Node,   typeof(Int),    typeof(int));
        Uint          = new Type("uint",           typeof(IValue<Uint>),    Node,   typeof(Uint),   typeof(uint));
        Double        = new Type("double",         typeof(IValue<Double>),  Node,   typeof(Double), typeof(double));
        Float         = new Type("float",          typeof(IValue<Float>),   Node,   typeof(Float),  typeof(float));
        String        = new Type("string",         typeof(IValue<String>),  Node,   typeof(String), typeof(string));
        Namespace     = new Type("namespace",      typeof(Namespace),       Node);
        FuncGroup     = new Type("function-group", typeof(FuncGroup),       Node);
        FuncDef       = new Type("function-def",   typeof(IFuncDef),        Node);

        // TODO: Come up with a better way to handle sub-typing than predefining them.
        CounterInt    = new Type("counter-int",    typeof(Counter<Int>),    Int,    typeof(Int));
        CounterDouble = new Type("counter-double", typeof(Counter<Double>), Double, typeof(Double));
        Toggler       = new Type("toggler",        typeof(Toggler),         Bool,   typeof(Bool));
        LatchObject   = new Type("latch-object",   typeof(Latch<Object>),   Object, typeof(Object));
        LatchBool     = new Type("latch-bool",     typeof(Latch<Bool>),     Bool,   typeof(Bool));
        LatchInt      = new Type("latch-int",      typeof(Latch<Int>),      Int,    typeof(Int));
        LatchDouble   = new Type("latch-double",   typeof(Latch<Double>),   Double, typeof(Double));
        LatchString   = new Type("latch-string",   typeof(Latch<String>),   String, typeof(String));

        addCast<IValueParent<Bool>>(Bool.imps, Trigger, (input) => new BoolAsTrigger(input));
        addImplicit<Bool, Object>();
        addImplicit<Bool, String>();

        addImplicit<Int, Object>();
        addExplicit<Int, Uint>();
        addImplicit<Int, Double>();
        addImplicit<Int, Float>();
        addImplicit<Int, String>();

        addImplicit<Uint, Object>();
        addExplicit<Uint, Int>();
        addImplicit<Uint, Double>();
        addImplicit<Uint, Float>();
        addImplicit<Uint, String>();
        
        addImplicit<Double, Object>();
        addExplicit<Double, Int>();
        addExplicit<Double, Uint>();
        addExplicit<Double, Float>();
        addImplicit<Double, String>();

        addImplicit<Float, Object>();
        addExplicit<Float, Int>();
        addExplicit<Float, Uint>();
        addImplicit<Float, Double>();
        addImplicit<Float, String>();

        addExplicit<Object, Bool>();
        addExplicit<Object, Int>();
        addExplicit<Object, Uint>();
        addExplicit<Object, Double>();
        addExplicit<Object, Float>();
        addImplicit<Object, String>();

        addImplicit<String, Object>();

        addCast<IFuncDef>(FuncDef.imps, FuncGroup, (input) => new FuncGroup(input));
    }

    /// <summary>Adds a cast-ability definition to a type.</summary>
    /// <typeparam name="T">The node type for the source type to cast from.</typeparam>
    /// <param name="dict">Either the implicit or explicit dictionary for the type being added to.</param>
    /// <param name="dest">The destination type to cast to.</param>
    /// <param name="func">The function for performing the cast.</param>
    static private void addCast<T>(Dictionary<Type, caster> dict, Type dest, S.Func<T, INode> func)
        where T : INode =>
        dict[dest] = input => input is T value ? func(value) : null;

    /// <summary>Adds an implicit cast-ability definition to a type.</summary>
    /// <typeparam name="Tin">The input data type.</typeparam>
    /// <typeparam name="Tout">The output data type.</typeparam>
    static private void addImplicit<Tin, Tout>()
        where Tin:  struct, IData
        where Tout: struct, IImplicit<Tin, Tout>, IEquatable<Tout> =>
        addCast<IValueParent<Tin>>(default(Tin).Type.imps, default(Tout).Type, input => new Implicit<Tin, Tout>(input));
    
    /// <summary>Adds an explicit cast-ability definition to a type.</summary>
    /// <typeparam name="Tin">The input data type.</typeparam>
    /// <typeparam name="Tout">The output data type.</typeparam>
    static private void addExplicit<Tin, Tout>()
        where Tin:  struct, IData
        where Tout: struct, IExplicit<Tin, Tout>, IEquatable<Tout> =>
        addCast<IValueParent<Tin>>(default(Tin).Type.exps, default(Tout).Type, input => new Explicit<Tin, Tout>(input));

    #endregion
}
