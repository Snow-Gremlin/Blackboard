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

        private void addImplicit(Type dest, int match, Caster func) {
            this.implicitCast[dest] = func;
            this.castMatch[dest] = match;
        }

        private void addExplicit(Type dest, Caster func) =>
            this.implicitCast[dest] = func;

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

        static Type() {
            Trigger       = new Type("trigger", typeof(ITrigger));
            Bool          = new Type("bool", typeof(IValue<Bool>));
            Int           = new Type("int", typeof(IValue<Int>));
            Double        = new Type("double", typeof(IValue<Double>));
            String        = new Type("string", typeof(IValue<String>));
            CounterInt    = new Type("counter-int", typeof(Counter<Int>));
            CounterDouble = new Type("counter-double", typeof(Counter<Double>));
            Toggler       = new Type("toggler", typeof(Toggler));
            LatchBool     = new Type("latch-bool", typeof(Latch<Bool>));
            LatchInt      = new Type("latch-int", typeof(Latch<Int>));
            LatchDouble   = new Type("latch-double", typeof(Latch<Double>));
            LatchString   = new Type("latch-string", typeof(Latch<String>));

            Bool.addImplicit(Trigger, 1, (INode input) => input is IValue<Bool> value ? new BoolAsTrigger(value) : null);
            Bool.addImplicit(String,  1, (INode input) => input is IValue<Bool> value ? new Implicit<Bool, String>(value) : null);

            Int.addImplicit(Double, 1, (INode input) => input is IValue<Int> value ? new Implicit<Int, Double>(value) : null);
            Int.addImplicit(String, 1, (INode input) => input is IValue<Int> value ? new Implicit<Int, String>(value) : null);

            Double.addExplicit(Int,       (INode input) => input is IValue<Double> value ? new Explicit<Double, Int>(value) : null);
            Double.addImplicit(String, 1, (INode input) => input is IValue<Double> value ? new Implicit<Double, String>(value) : null);

            CounterInt.addImplicit(Int,    1, (INode input) => input is IValue<Int> value ? value : null);
            CounterInt.addImplicit(Double, 2, (INode input) => input is IValue<Int> value ? new Implicit<Int, Double>(value) : null);
            CounterInt.addImplicit(String, 2, (INode input) => input is IValue<Int> value ? new Implicit<Int, String>(value) : null);

            CounterDouble.addExplicit(Int,       (INode input) => input is IValue<Double> value ? new Explicit<Double, Int>(value) : null);
            CounterDouble.addImplicit(Double, 1, (INode input) => input is IValue<Double> value ? value : null);
            CounterDouble.addImplicit(String, 2, (INode input) => input is IValue<Double> value ? new Implicit<Double, String>(value) : null);

            Toggler.addImplicit(Trigger, 2, (INode input) => input is IValue<Bool> value ? new BoolAsTrigger(value) : null);
            Toggler.addImplicit(Bool,    1, (INode input) => input is IValue<Bool> value ? value : null);
            Toggler.addImplicit(String,  2, (INode input) => input is IValue<Bool> value ? new Implicit<Bool, String>(value) : null);

            LatchBool.addImplicit(Trigger, 2, (INode input) => input is IValue<Bool> value ? new BoolAsTrigger(value) : null);
            LatchBool.addImplicit(Bool,    1, (INode input) => input is IValue<Bool> value ? value : null);
            LatchBool.addImplicit(String,  2, (INode input) => input is IValue<Bool> value ? new Implicit<Bool, String>(value) : null);

            LatchInt.addImplicit(Int,    1, (INode input) => input is IValue<Int> value ? value : null);
            LatchInt.addImplicit(Double, 2, (INode input) => input is IValue<Int> value ? new Implicit<Int, Double>(value) : null);
            LatchInt.addImplicit(String, 2, (INode input) => input is IValue<Int> value ? new Implicit<Int, String>(value) : null);

            LatchDouble.addExplicit(Int,       (INode input) => input is IValue<Double> value ? new Explicit<Double, Int>(value) : null);
            LatchDouble.addImplicit(Double, 1, (INode input) => input is IValue<Double> value ? value : null);
            LatchDouble.addImplicit(String, 2, (INode input) => input is IValue<Double> value ? new Implicit<Double, String>(value) : null);

            LatchString.addImplicit(String, 1, (INode input) => input is IValue<String> value ? value : null);
        }
    }
}
