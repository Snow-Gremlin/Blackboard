using Blackboard.Core;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Interfaces;
using Caps = Blackboard.Core.Nodes.Caps;

namespace Blackboard.Parser.Actors.Caps {

    /// <summary>Is an actor for creating a new literal node.</summary>
    /// <typeparam name="T">The type of of the value in the actor.</typeparam>
    sealed internal class Literal<T>: IActor
        where T : IComparable<T>, new() {

        /// <summary>Creates a new literal actor for the given value.</summary>
        /// <param name="value">The value to assign to the literal.</param>
        public Literal(T value) {
            this.Value = value;
        }

        /// <summary>The value to assign to the literal.</summary>
        public T Value;

        /// <summary>Reduces this actor to a literal of the value without writing any nodes.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode Evaluate() => this.BuildNode();

        /// <summary>Creates and writes a node to Blackboard.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode BuildNode() => new Caps.Literal<T>(this.Value);

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public Type Returns() => Type.FromType<Caps.Literal<T>>();
    }
}
