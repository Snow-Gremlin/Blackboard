using PetiteParser.Scanner;

namespace Blackboard.Parser.Prepers {

    /// <summary>The interface for all prepers.</summary>
    internal interface IPreper {

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <remarks>
        /// The returned performer for building the nodes should be valid
        /// such that no errors will occur when they perform.
        /// </remarks>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="option">The option for preparing this preper.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public Performers.Performer Prepare(Formula formula, Options option);
    }
}
