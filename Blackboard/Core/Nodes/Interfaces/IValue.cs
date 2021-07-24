using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for a node which has a value.</summary>
    /// <typeparam name="T">The type of value for this node.</typeparam>
    public interface IValue<out T>: INode
        where T : IData {

        /// <summary>The value of this node.</summary>
        T Value { get; }
    }
}
