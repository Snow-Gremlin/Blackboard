using Blackboard.Core.Nodes.Outer;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>Indicates nodes which can be a constant.</summary>
    public interface IConstantable: INode {

        /// <summary>Converts this node to a constant.</summary>
        /// <returns>A constant of this node or null if not able to be made into a constant.</returns>
        public IConstant ToConstant() =>
            this switch {
                IConstant c => c,
                IDataNode v => Literal.Data(v.Data),
                ITrigger  t => new ConstTrigger(t.Provoked),
                _           => null,
            };
    }
}
