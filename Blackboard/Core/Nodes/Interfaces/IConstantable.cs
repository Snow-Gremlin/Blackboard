namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>Indicates nodes which can be a constant.</summary>
    public interface IConstantable: INode {

        /// <summary>Converts this node to a constant.</summary>
        /// <returns>A constant of this node.</returns>
        public IConstant ToConstant();
    }
}
