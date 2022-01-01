using System.Collections.Generic;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type can be added to another of the same type.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface IAdditive<T>: IData
        where T : IData {

        /// <summary>This will get the summation of the given other data.</summary>
        /// <remarks>The current value is not used in the sum.</remarks>
        /// <param name="other">The other data to sum together.</param>
        /// <returns>The sum of the given data values.</returns>
        T Sum(IEnumerable<T> other);
    }
}
