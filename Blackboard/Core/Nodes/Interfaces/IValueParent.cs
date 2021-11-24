using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for a node which has a value and can adopt children.</summary>
    /// <typeparam name="T">The type of value for this node.</typeparam>
    public interface IValueParent<T>: IValue<T>, IParent
        where T: IData {
        // Empty
    }
}
