using Blackboard.Parser.Performers;

namespace Blackboard.Parser.Preppers {

    /// <summary>The interface for all preppers.</summary>
    internal interface IPrepper {

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <remarks>
        /// The returned performer for building the nodes must be valid
        /// such that no errors will occur when they perform.
        /// </remarks>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="reduce">
        /// True to reduce the nodes to constants without writting them to Blackboard.
        /// False to create the nodes and look up identifiers when running.
        /// </param>
        /// <returns>The performer which will create the nodes for this prepper.</returns>
        /// <summary>Gets the performers for to build with.</summary>
        public IPerformer Prepare(Formula formula, bool reduce = false);
    }
}
