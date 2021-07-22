using PP = PetiteParser;
using System.Collections.Generic;

namespace Blackboard.Parser.StackItems {

    /// <summary>The stack item for an identifier.</summary>
    public class Identifier: List<string> {

        /// <summary>The location of the first part of this id.</summary>
        public readonly PP.Scanner.Location Location;

        /// <summary>Creates a new identifier.</summary>
        /// <param name="loc">The location of the first id part.</param>
        /// <param name="ids">Adds the initial id parts for this id.</param>
        public Identifier(PP.Scanner.Location loc, params string[] ids) {
            this.Location = loc;
            this.AddRange(ids);
        }

        /// <summary>This gets the string for this Identifier.</summary>
        /// <returns>The identifier string.</returns>
        public override string ToString() => string.Join(".", this);
    }
}
