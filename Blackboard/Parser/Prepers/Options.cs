namespace Blackboard.Parser.Prepers {

    /// <summary>
    /// These are options used when preparing performers.
    /// These options are provided by a parent preper into the children prepers.
    /// </summary>
    internal enum Options {

        /// <summary>
        /// This is the default option passed into the root of the parse trees.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that an assignable node (e.g. identifier or indexer)
        /// should assign an existing node instead of resolving the node.
        /// </summary>
        Assign,

        /// <summary>
        /// Indicates that an assignable node (e.g. identifier or indexer)
        /// should create a new node instead of resolving the node.
        /// If the node exists then an error should occur.
        /// </summary>
        Define,

        /// <summary>
        /// Indicates that the node should be evaluated and reduced to
        /// a constant value (e.g. a Literal node).
        /// </summary>
        Evaluate,

        /// <summary>
        /// Indicates that the node should be created
        /// and written to Blackboard if it can be. Identifiers and
        /// other references should resolve into an existing node.
        /// </summary>
        Create
    }
}
