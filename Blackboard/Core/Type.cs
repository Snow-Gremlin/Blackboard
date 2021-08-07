using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core {

    using Caster = S.Func<INode, INode>;

    /// <summary>The types implemented for Blackboard.</summary>
    sealed public class Type {

        /// <summary>The base type for all other types.</summary>
        static public readonly Type Node;

        /// <summary>The trigger type.</summary>
        static public readonly Type Trigger;

        /// <summary>The boolean value type.</summary>
        static public readonly Type Bool;

        /// <summary>The integer value type.</summary>
        static public readonly Type Int;

        /// <summary>The double value type.</summary>
        static public readonly Type Double;

        /// <summary>The string value type.</summary>
        static public readonly Type String;

        /// <summary>The integer counter type which is an extension of the integer value type.</summary>
        static public readonly Type CounterInt;

        /// <summary>The double counter type which is an extension of the double value type.</summary>
        static public readonly Type CounterDouble;

        /// <summary>The toggler type which is an extension of the boolean value type.</summary>
        static public readonly Type Toggler;

        /// <summary>The boolean latch which is an extension of the boolean value type.</summary>
        static public readonly Type LatchBool;

        /// <summary>The integer latch which is an extension of the integer value type.</summary>
        static public readonly Type LatchInt;

        /// <summary>The double latch which is an extension of the double value type.</summary>
        static public readonly Type LatchDouble;

        /// <summary>The string latch which is an extension of the string value type.</summary>
        static public readonly Type LatchString;

        /// <summary>Gets all the types.</summary>
        /// <remarks>These must be ordered by inheriting object before the object that was inherited.</remarks>
        static public IEnumerable<Type> AllTypes {
            get {
                yield return LatchString;
                yield return LatchDouble;
                yield return LatchInt;
                yield return LatchBool;
                yield return Toggler;
                yield return CounterDouble;
                yield return CounterInt;
                yield return String;
                yield return Double;
                yield return Int;
                yield return Bool;
                yield return Trigger;
                yield return Node;
            }
        }

        /// <summary>Finds the type given the type name.</summary>
        /// <param name="name">The name of the type to get.</param>
        /// <returns>The type for the given name or null if name isn't found.</returns>
        static public Type FromName(string name) {
            foreach (Type t in AllTypes) {
                if (t.Name == name) return t;
            }
            return null;
        }

        /// <summary>This gets the type given a node.</summary>
        /// <param name="node">The node to get the type of.</param>
        /// <returns>The type for the given node or null if not found.</returns>
        static public Type TypeOf(INode node) => FromType(node.GetType());

        /// <summary>This gets the type from the given generic.</summary>
        /// <typeparam name="T">The generic type to get the type of.</typeparam>
        /// <returns>The type for the given generic or null if not found.</returns>
        static public Type FromType<T>() where T : INode => FromType(typeof(T));

        /// <summary>This get the type from the given C# type.</summary>
        /// <param name="type">The C# type to get this type of.</param>
        /// <returns>The type for the given C# type or null if not found.</returns>
        static public Type FromType(S.Type type) {
            foreach (Type t in AllTypes) {
                if (type.IsAssignableTo(t.RealType)) return t;
            }
            return null;
        } 

        /// <summary>The display name of the type.</summary>
        public readonly string Name;

        /// <summary>The C# type for this type.</summary>
        public readonly S.Type RealType;

        /// <summary>This is the base type this type inherits from.</summary>
        /// <remarks>This is only null for Node type since it doesn't inherit from anything.</remarks>
        public readonly Type BaseType;

        /// <summary>This is a dictionary of other types to an implicit cast.</summary>
        private Dictionary<Type, Caster> imps;

        /// <summary>This is a dictionary of other types to an explicit cast.</summary>
        private Dictionary<Type, Caster> exps;

        /// <summary>This creates a new type.</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="realType">The C# type for this type.</param>
        /// <param name="baseType">The base type this type inherits from.</param>
        private Type(string name, S.Type realType, Type baseType) {
            this.Name = name;
            this.RealType = realType;
            this.BaseType = baseType;
            this.imps = new();
            this.exps = new();
        }

        /// <summary>This determines the implicit and inheritence math.</summary>
        /// <param name="t">The type to try castig to.</param>
        /// <returns>The result of the match.</returns>
        static public TypeMatch Match<T>(INode node) where T : INode =>
            FromType<T>().Match(TypeOf(node));

        /// <summary>This determines the implicit and inheritence match.</summary>
        /// <param name="t">The type to try castig to.</param>
        /// <returns>The result of the match.</returns>
        public TypeMatch Match(Type t) {
            int steps;

            // Check if inheritance can be used.
            // Find the closest inherited type so that the most specific match can be choosen.
            if (t.RealType.IsAssignableTo(this.RealType)) {
                steps = -1;
                do {
                    t = t.BaseType;
                    steps++;
                } while (t is not null && t.RealType.IsAssignableTo(this.RealType));
                return TypeMatch.Inherit(steps);
            }

            // Check if implicit casts exist.
            // Add an initial penalty for using an implicit cast instead of inheritance.
            steps = 0;
            do {
                if (t.imps.ContainsKey(this)) return TypeMatch.Cast(steps);
                t = t.BaseType;
                steps++;
            } while (t is not null);
            return TypeMatch.NoMatch;
        }

        /// <summary>Performs an implicit cast of the given node into this type.</summary>
        /// <param name="node">The node to implicitly cast.</param>
        /// <returns>The node cast into this type or null if the cast is not posible.</returns>
        static public T Implicit<T>(INode node) where T : class, INode =>
            FromType<T>().Implicit(node) as T;

        /// <summary>Performs an implicit cast of the given node into this type.</summary>
        /// <param name="node">The node to implicitly cast.</param>
        /// <returns>The node cast into this type or null if the cast is not posible.</returns>
        public INode Implicit(INode node) =>
            cast(true, node, TypeOf(node), this);

        /// <summary>Performs an explicit cast of the given node into this type.</summary>
        /// <param name="node">The node to explicitly cast.</param>
        /// <returns>The node cast into this type or null if the cast is not posible.</returns>
        static public T Explicit<T>(INode node) where T : class, INode =>
            FromType<T>().Explicit(node) as T;

        /// <summary>Performs an explicit cast of the given node into this type.</summary>
        /// <param name="node">The node to explicitly cast.</param>
        /// <returns>The node cast into this type or null if the cast is not posible.</returns>
        public INode Explicit(INode node) =>
            cast(false, node, TypeOf(node), this);

        /// <summary>Performs a cast of the given node into the destination type.</summary>
        /// <param name="imp">True for implicitly cast, false for explicitly cast.</param>
        /// <param name="node">The node to cast.</param>
        /// <param name="src">The type of the given node.</param>
        /// <param name="dest">The destination type to cast into.</param>
        /// <returns>The node cast into the destination type or null if it can not be cast.</returns>
        static private INode cast(bool imp, INode node, Type src, Type dest) {
            if (src.RealType.IsAssignableTo(dest.RealType)) return node;
            do {
                Dictionary<Type, Caster> dict = imp ? src.imps : src.exps;
                if (dict.TryGetValue(dest, out Caster func)) return func(node);
                src = src.BaseType;
            } while (src is not null);
            return null;
        }

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
        public override bool Equals(object obj) => ReferenceEquals(this, obj);
        
        /// <summary>Gets the hash code for this type.</summary>
        /// <returns>The type's name hash code.</returns>
        public override int GetHashCode() => this.Name.GetHashCode();

        /// <summary>Gets the name of the type.</summary>
        /// <returns>The name of the type.</returns>
        public override string ToString() => this.Name;

        /// <summary>Adds a castability definition to a type.</summary>
        /// <typeparam name="T">The node type for the source type to cast from.</typeparam>
        /// <param name="dict">Either the implicit or explicit dictionar for the type being added to.</param>
        /// <param name="dest">The destination type to cast to.</param>
        /// <param name="func">The function for performing the cast.</param>
        static private void addCast<T>(Dictionary<Type, Caster> dict, Type dest, S.Func<T, INode> func) where T: INode =>
            dict[dest] = (INode input) => input is T value ? func(value) : null;

        /// <summary>Initializes the types before they are used.</summary>
        static Type() {
            Node          = new Type("node",           typeof(INode),           null);
            Trigger       = new Type("trigger",        typeof(ITrigger),        Node);
            Bool          = new Type("bool",           typeof(IValue<Bool>),    Node);
            Int           = new Type("int",            typeof(IValue<Int>),     Node);
            Double        = new Type("double",         typeof(IValue<Double>),  Node);
            String        = new Type("string",         typeof(IValue<String>),  Node);
            CounterInt    = new Type("counter-int",    typeof(Counter<Int>),    Int);
            CounterDouble = new Type("counter-double", typeof(Counter<Double>), Double);
            Toggler       = new Type("toggler",        typeof(Toggler),         Bool);
            LatchBool     = new Type("latch-bool",     typeof(Latch<Bool>),     Bool);
            LatchInt      = new Type("latch-int",      typeof(Latch<Int>),      Int);
            LatchDouble   = new Type("latch-double",   typeof(Latch<Double>),   Double);
            LatchString   = new Type("latch-string",   typeof(Latch<String>),   String);

            addCast<IValue<Bool>>(Bool.imps, Trigger, (input) => new BoolAsTrigger(input));
            addCast<IValue<Bool>>(Bool.imps, String,  (input) => new Implicit<Bool, String>(input));

            addCast<IValue<Int>>(Int.imps, Double, (input) => new Implicit<Int, Double>(input));
            addCast<IValue<Int>>(Int.imps, String, (input) => new Implicit<Int, String>(input));

            addCast<IValue<Double>>(Double.exps, Int,    (input) => new Explicit<Double, Int>(input));
            addCast<IValue<Double>>(Double.imps, String, (input) => new Implicit<Double, String>(input));
        }
    }
}
