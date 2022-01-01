using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an input which has a value.</summary>
    /// <typeparam name="T">The type of the value to input.</typeparam>
    public interface IValueInput<T>: IValueParent<T>, IInput
        where T : IData {

        /// <summary>Sets the value of this input.</summary>
        /// <remarks>This is not intended to be called directly, it should be called via the slate or action.</remarks>
        /// <param name="value">The value to assign to the input.</param>
        /// <returns>True if there was any change, false otherwise.</returns>
        public bool SetValue(T value);
    }
}
