using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Parser.Performers;

namespace Blackboard.Parser.Prepers {

    /// <summary>These are factories for easily making literal prepers.</summary>
    static internal class LiteralPrep {

        /// <summary>Creates a boolean literal preper.</summary>
        /// <param name="value">The boolean value for the literal.</param>
        /// <returns>The newly created literal preper.</returns>
        static public LiteralPrep<Bool> Bool(bool value) => new(new Bool(value));

        /// <summary>Creates a integer literal preper.</summary>
        /// <param name="value">The integer value for the literal.</param>
        /// <returns>The newly created literal preper.</returns>
        static public LiteralPrep<Int> Int(int value) => new(new Int(value));

        /// <summary>Creates a double literal preper.</summary>
        /// <param name="value">The double value for the literal.</param>
        /// <returns>The newly created literal preper.</returns>
        static public LiteralPrep<Double> Double(double value) => new(new Double(value));

        /// <summary>Creates a string literal preper.</summary>
        /// <param name="value">The string value for the literal.</param>
        /// <returns>The newly created literal preper.</returns>
        static public LiteralPrep<String> String(string value) => new(new String(value));
    }

    /// <summary>This is a preper for creating a new literal node.</summary>
    /// <typeparam name="T">The type of the value in the literal.</typeparam>
    sealed internal class LiteralPrep<T>: IPreper
        where T : IComparable<T>, new() {

        /// <summary>Creates a new literal preper for the given value.</summary>
        /// <param name="value">The value to assign to the literal.</param>
        public LiteralPrep(T value) {
            this.Value = value;
        }

        /// <summary>The value to assign to the literal.</summary>
        public T Value;

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">TNot Used</param>
        /// <param name="reduce">Not used since a literal already is a constant.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public IPerformer Prepare(Formula formula, bool reduce = false) => new NodeHold(new Literal<T>(this.Value));
    }
}
