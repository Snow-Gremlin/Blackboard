using System.Collections.Generic;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type can be multiplied with another of the same type.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    /// <see cref="https://en.wikipedia.org/wiki/Multiplication"/>
    public interface IMultiplicative<T>: IData
        where T : IData {

        /// <summary>Gets the product of this value and the other value.</summary>
        /// <remarks>The current value is not used in the product.</remarks>
        /// <param name="other">The value to multiply this value with.</param>
        /// <returns>The product of this value and the other value.</returns>
        T Mul(IEnumerable<T> other);

        /// <summary>
        /// Indicates that for this data type, multiplication is commutable,
        /// meaning that the order of the parents makes no difference to the result.
        /// </summary>
        /// <remarks>This value shall be constant for a data type.</remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
        bool MulCommutable { get; }

        /// <summary>The identity of multiplication for this data type.</summary>
        /// <remarks>Typically this is one.</remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
        T MulIdentityValue { get; }
   }
}
