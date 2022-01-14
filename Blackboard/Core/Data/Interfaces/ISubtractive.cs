namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type can be subtracted to another of the same type.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface ISubtractive<T>: IData
        where T : IData {

        /// <summary>Gets the difference between this value and the other value.</summary>
        /// <param name="other">The value to subtract from this value.</param>
        /// <returns>The difference between this value and the other value.</returns>
        T Sub(T other);
    }
}
