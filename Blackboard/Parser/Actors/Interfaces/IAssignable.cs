using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Parser.Actors.Interfaces {

    /// <summary>Indicates that this actor is assignable.</summary>
    internal interface IAssignable: IActor {

        /// <summary>This will attempt to write this node to the given receiver or to the top of the stack.</summary>
        /// <param name="node">The node to assign to this receiver as this identifier.</param>
        /// <param name="mustCreate">Indicates that the value must be created and does not exist yet.</param>
        public void Assign(INode node, bool mustCreate);
    }
}
