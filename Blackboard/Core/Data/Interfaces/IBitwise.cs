namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type can be bitwise modified.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface IBitwise<T>: IData
        where T : IData {

        /// <summary>This gets the bitwise NOT of this data value.</summary>
        /// <returns>The bitwise NOT of this value.</returns>
        T BitwiseNot();

        /// <summary>This gets the bitwise AND of this value and the other value.</summary>
        /// <param name="other">The other value to bitwise AND with.</param>
        /// <returns>The result of the bitwise AND.</returns>
        T BitwiseAnd(T other);

        /// <summary>This gets the bitwise OR of this value and the other value.</summary>
        /// <param name="other">The other value to bitwise OR with.</param>
        /// <returns>The result of the bitwise OR.</returns>
        T BitwiseOr(T other);

        /// <summary>This gets the bitwise XOR of this value and the other value.</summary>
        /// <param name="other">The other value to bitwise XOR with.</param>
        /// <returns>The result of the bitwise XOR.</returns>
        T BitwiseXor(T other);

        /// <summary>This gets the left shift of this value by the other value.</summary>
        /// <param name="other">The number of bits to shift by.</param>
        /// <returns>The left shifted value.</returns>
        T LeftShift(T other);

        /// <summary>This gets the right shift of this value by the other value.</summary>
        /// <param name="other">The number of bits to shift by.</param>
        /// <returns>The right shifted value.</returns>
        T RightShift(T other);
    }
}
