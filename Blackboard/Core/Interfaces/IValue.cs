namespace Blackboard.Core.Interfaces {

    /// <summary>The interface for a node which has a value.</summary>
    /// <typeparam name="T">The type of value for this node.</typeparam>
    public interface IValue<out T>: INode {

        /// <summary>The value of this node.</summary>
        T Value { get; }
    }
}
