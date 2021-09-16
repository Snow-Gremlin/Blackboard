using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Parser.Actors {

    /// <summary>
    /// An actor which contains a value.
    /// This value can not be evaluated nor built,
    /// this is only for storing a value on the stack for the parser.
    /// </summary>
    /// <typeparam name="T">The type of the value to stash.</typeparam>
    sealed internal class Stash<T>: IActor {

        /// <summary>Creates a new value actor.</summary>
        /// <param name="value">The value to store.</param>
        public Stash(T value) {
            this.Value = value;
        }

        /// <summary>The value being stashed onto the stack.</summary>
        public T Value;

        /// <summary>May not evaluate a stashed value.</summary>
        /// <returns>Throws an expection.</returns>
        public INode Evaluate() => throw new Exception("May not evaluate a stashed value action.");

        /// <summary>May not build a stashed value.</summary>
        /// <returns>Throws an expection.</returns>
        public INode BuildNode() => throw new Exception("May not build a node with a stashed value action.");

        /// <summary>Returns null since the stashed value is just for storage.</summary>
        /// <returns>This will always be null.</returns>
        public Type Returns() => null;
    }
}
