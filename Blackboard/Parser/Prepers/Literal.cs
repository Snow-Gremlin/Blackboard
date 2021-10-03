using Blackboard.Core;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Parser.Performers;
using PetiteParser.Scanner;
using Caps = Blackboard.Core.Nodes.Caps;

namespace Blackboard.Parser.Prepers {

    /// <summary>This is a preper for creating a new literal node.</summary>
    /// <typeparam name="T">The type of the value in the literal.</typeparam>
    sealed internal class Literal<T>: IPreper
        where T : IComparable<T>, new() {

        /// <summary>Creates a new literal preper for the given value.</summary>
        /// <param name="loc">The location this literal was defined at.</param>
        /// <param name="value">The value to assign to the literal.</param>
        public Literal(Location loc, T value) {
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
            new NodeRef(this.Location, new Caps.Literal<T>(this.Value)) :
            throw new Exception("Invalid option for a literal preper.").
                With("Option", option).
                With("Value", this.Value).
                With("Location", this.Location.ToString());
    }
}
