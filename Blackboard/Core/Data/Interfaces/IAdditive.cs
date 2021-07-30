namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type can be added to another of the same type.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface IAdditive<T>: IData
        where T : IData {

        /// <summary>This will add this data to the other data.</summary>
        /// <param name="other">The other data to add to this value.</param>
        /// <returns>The sum of the two data values.</returns>
        T Sum(T other);
    }
}
