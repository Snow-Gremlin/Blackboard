using System.Collections.Generic;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type can be multiplied with another of the same type.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface IMultiplicative<T>: IData
        where T : IData {

        /// <summary>Gets the product of this value and the other value.</summary>
        /// <remarks>The current value is not used in the product.</remarks>
        /// <param name="other">The value to multiply this value with.</param>
        /// <returns>The product of this value and the other value.</returns>
        T Mul(IEnumerable<T> other);
    }
}
