using System.Collections.Generic;

namespace Blackboard.Core.Data.Interfaces;

/// <summary>This indicates that this Blackboard data type can be added to another of the same type.</summary>
/// <typeparam name="T">The type of the data implementing this interface.</typeparam>
internal interface IAdditive<T> : IData
    where T : IData {

    /// <summary>This will get the summation of the given other data.</summary>
    /// <remarks>The current value is not used in the sum.</remarks>
    /// <param name="other">The other data to sum together.</param>
    /// <returns>The sum of the given data values.</returns>
    T Sum(IEnumerable<T> other);

    /// <summary>
    /// Indicates that for this data type, summation is commutable,
    /// meaning that the order of the parents makes no difference to the result.
    /// </summary>
    /// <remarks>This value shall be constant for a data type.</remarks>
    /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
    bool SumCommutable { get; }

    /// <summary>The identity of summation for this data type.</summary>
    /// <remarks>Typically this is zero or empty.</remarks>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    T SumIdentityValue { get; }
}
