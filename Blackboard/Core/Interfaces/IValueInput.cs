namespace Blackboard.Core.Interfaces {

    /// <summary>The interface for an input which has a value.</summary>
    /// <typeparam name="T">The type of the value to input.</typeparam>
    public interface IValueInput<T>: IInput, IValue<T> {

        /// <summary>Sets the value of this input.</summary>
        /// <param name="value">The value to input.</param>
        /// <returns>True if the value changed, false otherwise.</returns>
        bool SetValue(T value);
    }
}
