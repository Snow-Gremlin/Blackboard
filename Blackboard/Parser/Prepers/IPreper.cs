using PetiteParser.Scanner;
using Blackboard.Parser.Performers;

namespace Blackboard.Parser.Prepers {

    /// <summary>The interface for all prepers.</summary>
    internal interface IPreper {

        /// <summary>The location this node was defind in the code being parsed.</summary>
        public Location Location { get; }

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <remarks>
        /// At the end of prepare the performs for building the nodes should be valid
        /// such that no errors will occur when they perform.
        /// </remarks>
        /// <param name="virtualNodes">This is the list to add virtual nodes to.</param>
        /// <param name="option">The option for preparing this preper.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public IPerformer Prepare(VirtualNodeSet virtualNodes, Options option);
    }
}
