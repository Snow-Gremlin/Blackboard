using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates a data type which is comparable.</summary>
    /// <typeparam name="T">The type of data to compare.</typeparam>
    public interface IComparable<T>: S.IComparable<T>, S.IEquatable<T>, IData
        where T : IData {

        /// <summary>Gets the maximum value from the given other values.</summary>
        /// <remarks>The current value is not used in the maximum value.</remarks>
        /// <param name="other">The values to find the maximum from.</param>
        /// <returns>The maximum value from the given vales.</returns>
        T Max(IEnumerable<T> other);

        /// <summary>Gets the minimum value from given other values.</summary>
        /// <remarks>The current value is not used in the minimum value.</remarks>
        /// <param name="other">The values to find the minimum from.</param>
        /// <returns>The minimum value from the given vales.</returns>
        T Min(IEnumerable<T> other);
    }
}
