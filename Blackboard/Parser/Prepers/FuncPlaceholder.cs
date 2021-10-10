using Blackboard.Core;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Prepers {

    /// <summary>This is a function placeholder which should not be prepared but replaced by a FunctionPrep.</summary>
    sealed internal class FuncPlaceholder: IPreper {

        /// <summary>Creates a new function placeholder.</summary>
        /// <param name="loc">The location this function was defind in the code.</param>
        /// <param name="source">The source of the function to call.</param>
        public FuncPlaceholder(Location loc, IPreper source) {
            this.Location = loc;
            this.Source = source;
        }

        /// <summary>The location this function was called in the code.</summary>
        public Location Location;

        /// <summary>The source of the function to call.</summary>
        public IPreper Source;

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="option">The option for preparing this preper.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public Performers.Performer Prepare(Formula formula, Options option) =>
            throw new Exception("Function placeholder should never be prepared.").
                With("Source", this.Source).
                With("Location", this.Location);
    }
}
