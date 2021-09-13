using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an input which has a value.</summary>
    /// <remarks>All inputs may be used as an output.</remarks>
    /// <typeparam name="T">The type of the value to input.</typeparam>
    public interface IValueInput<T>: IInput, IEvaluatable, IDataNode
        where T : IData {

        /// <summary>Sets the value of this input.</summary>
        /// <remarks>This is not intended to be be called directly, it should be called via the driver.</remarks>
        /// <param name="value">The value to input.</param>
        /// <returns>True if the value changed, false otherwise.</returns>
        bool SetValue(T value);
    }
}
