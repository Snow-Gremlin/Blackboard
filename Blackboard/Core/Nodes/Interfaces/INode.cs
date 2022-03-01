namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for all nodes in the blackboard.</summary>
    public interface INode {

        /// <summary>This is the type name of the node without any type parameters.</summary>
        /// <remarks>
        /// This should be filled out at the lowest level in the sealed class to express
        /// the point of the class. Typically this should be the same name as the class
        /// and should be a regular identifier (no spaces, periods, hyphens, etc).
        /// </remarks>
        public string TypeName { get; }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public INode NewInstance();
    }
}
