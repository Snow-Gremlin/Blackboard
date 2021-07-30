using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core {

    using Caster = S.Func<INode, INode>;

    /// <summary>The types implemented for Blackboard.</summary>
    sealed public class Type {

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

        /// <summary>The boolean latch type which is an extension of the boolean value type.</summary>
        static public readonly Type LatchBool;

        /// <summary>The integer latch type which is an extension of the integer value type.</summary>
        static public readonly Type LatchInt;

        /// <summary>The double latch type which is an extension of the double value type.</summary>
        static public readonly Type LatchDouble;

        /// <summary>The string latch type which is an extension of the string value type.</summary>
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
            }
        }

        static public Type FromName(string name) {
            foreach (Type t in AllTypes) {
                if (t.Name == name) return t;
            }
            return null;
        }

        static public Type TypeOf(INode node) => FromType(node.GetType());

        static public Type FromType<T>() => FromType(typeof(T));

        static public Type FromType(S.Type type) {
            foreach (Type t in AllTypes) {
                if (t.BaseType.IsAssignableTo(type)) return t;
            }
            return null;
        }

        public readonly string Name;
        public readonly S.Type BaseType;
        private Dictionary<Type, int> castMatch;
        private Dictionary<Type, Caster> implicitCast;
        private Dictionary<Type, Caster> expliciteCast;

        private Type(string name, S.Type baseType) {
            this.Name = name;
            this.BaseType = baseType;
            this.castMatch = new();
            this.implicitCast = new();
            this.expliciteCast = new();
        }

        public int Match(Type t) => this == t ? 0 : this.castMatch.TryGetValue(t, out int match) ? match : -1;

        public INode Implicit(INode node) => this.cast(this.implicitCast, node);

        public INode Explicit(INode node) => this.cast(this.expliciteCast, node);

        private INode cast(Dictionary<Type, Caster> dict, INode node) {
            Type t = TypeOf(node);
            return this == t ? node :
                dict.TryGetValue(t, out Caster func) ? func(node) : null;
        }

        public override bool Equals(object obj) => obj is Type other && this.Name == other.Name;

        public override int GetHashCode() => this.Name.GetHashCode();

        public override string ToString() => this.Name;

        private void addImplicit<T>(Type dest, int match, S.Func<T, INode> func) where T: INode {
            this.implicitCast[dest] = (INode input) => input is T value ? func(value) : null;
            this.castMatch[dest] = match;
        }

        private void addExplicit<T>(Type dest, S.Func<T, INode> func) where T : INode =>
            this.expliciteCast[dest] = (INode input) => input is T value ? func(value) : null;

        static Type() {
            Trigger       = new Type("trigger",        typeof(ITrigger));
            Bool          = new Type("bool",           typeof(IValue<Bool>));
            Int           = new Type("int",            typeof(IValue<Int>));
            Double        = new Type("double",         typeof(IValue<Double>));
            String        = new Type("string",         typeof(IValue<String>));
            CounterInt    = new Type("counter-int",    typeof(Counter<Int>));
            CounterDouble = new Type("counter-double", typeof(Counter<Double>));
            Toggler       = new Type("toggler",        typeof(Toggler));
            LatchBool     = new Type("latch-bool",     typeof(Latch<Bool>));
            LatchInt      = new Type("latch-int",      typeof(Latch<Int>));
            LatchDouble   = new Type("latch-double",   typeof(Latch<Double>));
            LatchString   = new Type("latch-string",   typeof(Latch<String>));

            Bool.addImplicit<IValue<Bool>>(Trigger, 1, (input) => new BoolAsTrigger(input));
            Bool.addImplicit<IValue<Bool>>(String,  1, (input) => new Implicit<Bool, String>(input));

            Int.addImplicit<IValue<Int>>(Double, 1, (input) => new Implicit<Int, Double>(input));
            Int.addImplicit<IValue<Int>>(String, 1, (input) => new Implicit<Int, String>(input));

            Double.addExplicit<IValue<Double>>(Int,       (input) => new Explicit<Double, Int>(input));
            Double.addImplicit<IValue<Double>>(String, 1, (input) => new Implicit<Double, String>(input));

            CounterInt.addImplicit<IValue<Int>>(Int,    1, (input) => input);
            CounterInt.addImplicit<IValue<Int>>(Double, 2, (input) => new Implicit<Int, Double>(input));
            CounterInt.addImplicit<IValue<Int>>(String, 2, (input) => new Implicit<Int, String>(input));

            CounterDouble.addExplicit<IValue<Double>>(Int,       (input) => new Explicit<Double, Int>(input));
            CounterDouble.addImplicit<IValue<Double>>(Double, 1, (input) => input);
            CounterDouble.addImplicit<IValue<Double>>(String, 2, (input) => new Implicit<Double, String>(input));

            Toggler.addImplicit<IValue<Bool>>(Trigger, 2, (input) => new BoolAsTrigger(input));
            Toggler.addImplicit<IValue<Bool>>(Bool,    1, (input) => input);
            Toggler.addImplicit<IValue<Bool>>(String,  2, (input) => new Implicit<Bool, String>(input));

            LatchBool.addImplicit<IValue<Bool>>(Trigger, 2, (input) => new BoolAsTrigger(input));
            LatchBool.addImplicit<IValue<Bool>>(Bool,    1, (input) => input);
            LatchBool.addImplicit<IValue<Bool>>(String,  2, (input) => new Implicit<Bool, String>(input));

            LatchInt.addImplicit<IValue<Int>>(Int,    1, (input) => input);
            LatchInt.addImplicit<IValue<Int>>(Double, 2, (input) => new Implicit<Int, Double>(input));
            LatchInt.addImplicit<IValue<Int>>(String, 2, (input) => new Implicit<Int, String>(input));

            LatchDouble.addExplicit<IValue<Double>>(Int,       (input) => new Explicit<Double, Int>(input));
            LatchDouble.addImplicit<IValue<Double>>(Double, 1, (input) => input);
            LatchDouble.addImplicit<IValue<Double>>(String, 2, (input) => new Implicit<Double, String>(input));

            LatchString.addImplicit<IValue<String>>(String, 1, (input) => input);
        }
    }
}
