namespace Blackboard.Core.Interfaces {

    /// <summary>The interface for an input which has a value.</summary>
    /// <remarks>All inputs may be used as an output.</remarks>
    /// <typeparam name="T">The type of the value to input.</typeparam>
    public interface IValueInput<T>: IValueOutput<T>, IInput {

        /// <summary>Sets the value of this input.</summary>
        /// <param name="value">The value to input.</param>
        /// <returns>True if the value changed, false otherwise.</returns>
        bool SetValue(T value);
    }
}
