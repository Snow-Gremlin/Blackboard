using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Parser.Performer;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Prepers {

    /// <summary>These are factories for easily making literal prepers.</summary>
    static internal class LiteralPrep {

        /// <summary>Creates a boolean literal preper.</summary>
        /// <param name="loc">The location this literal was defined at.</param>
        /// <param name="value">The boolean value for the literal.</param>
        /// <returns>The newly created literal preper.</returns>
        static public LiteralPrep<Bool> Bool(Location loc, bool value) => new(loc, new Bool(value));

        /// <summary>Creates a integer literal preper.</summary>
        /// <param name="loc">The location this literal was defined at.</param>
        /// <param name="value">The integer value for the literal.</param>
        /// <returns>The newly created literal preper.</returns>
        static public LiteralPrep<Int> Int(Location loc, int value) => new(loc, new Int(value));

        /// <summary>Creates a double literal preper.</summary>
        /// <param name="loc">The location this literal was defined at.</param>
        /// <param name="value">The double value for the literal.</param>
        /// <returns>The newly created literal preper.</returns>
        static public LiteralPrep<Double> Double(Location loc, double value) => new(loc, new Double(value));

        /// <summary>Creates a string literal preper.</summary>
        /// <param name="loc">The location this literal was defined at.</param>
        /// <param name="value">The string value for the literal.</param>
        /// <returns>The newly created literal preper.</returns>
        static public LiteralPrep<String> String(Location loc, string value) => new(loc, new String(value));
    }

    /// <summary>This is a preper for creating a new literal node.</summary>
    /// <typeparam name="T">The type of the value in the literal.</typeparam>
    sealed internal class LiteralPrep<T>: IPreper
        where T : IComparable<T>, new() {

        /// <summary>Creates a new literal preper for the given value.</summary>
        /// <param name="loc">The location this literal was defined at.</param>
        /// <param name="value">The value to assign to the literal.</param>
        public LiteralPrep(Location loc, T value) {
            this.Location = loc;
            this.Value = value;
        }

        /// <summary>The value to assign to the literal.</summary>
        public T Value;

        /// <summary>The location this node was defind in the code being parsed.</summary>
        public Location Location { get; private set; }

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="option">The option for preparing this preper.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public IPerformer Prepare(Formula formula, Options option) =>
            option == Options.Create || option == Options.Evaluate ?
            new NodeHold(this.Location, new Literal<T>(this.Value)) :
            throw new Exception("Invalid option for a literal preper.").
                With("Option", option).
                With("Value", this.Value).
                With("Location", this.Location);
    }
}
