using Blackboard.Core.Data.Interfaces;
using S = System;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a 32 bit signed integer value such that it can be used in generics.</summary>
    public struct Int: IArithmetic<Int>, IComparable<Int>, IBitwise<Int>,
        IExplicit<Double, Int> {

        /// <summary>The integer value being stored.</summary>
        public readonly int Value;

        /// <summary>Creates a new integer data value.</summary>
        /// <param name="value">The integer value to store.</param>
        public Int(int value = 0) {
            this.Value = value;
        }

        /// <summary>Gets the absolute value of this data value.</summary>
        /// <returns>The absolute value of this value.</returns>
        public Int Abs() => new(S.Math.Abs(this.Value));

        /// <summary>Gets the absolute value of this data value.</summary>
        /// <returns>The absolute value of this value.</returns>
        public Int Neg() => new(-this.Value);

        /// <summary>Gets this value incremented by one.</summary>
        /// <returns>This data value plus one.</returns>
        public Int Inc() => new(this.Value + 1);

        /// <summary>Gets the division of this value and the other value.</summary>
        /// <param name="other">The value to divide this value with.</param>
        /// <returns>This value divided by the other value.</returns>
        public Int Div(Int other) => new(this.Value / other.Value);

        /// <summary>Gets the modulo of this value and the other value.</summary>
        /// <remarks>The result will have the same sign as this value.</remarks>
        /// <param name="other">The value to mod this value with.</param>
        /// <returns>The modulo of this value and the other value.</returns>
        public Int Mod(Int other) => new(this.Value % other.Value);

        /// <summary>Gets the remainder of this value divided by the other value.</summary>
        /// <remarks>This is identical to modulo for integers.</remarks>
        /// <param name="other">The value to divide this value with.</param>
        /// <returns>The remainder of this value divided the other value.</returns>
        public Int Rem(Int other) => new(this.Value % other.Value);

        /// <summary>Gets the product of this value and the other value.</summary>
        /// <param name="other">The value to multiply this value with.</param>
        /// <returns>The product of this value and the other value.</returns>
        public Int Mul(Int other) => new(this.Value * other.Value);

        /// <summary>Gets the difference between this value and the other value.</summary>
        /// <param name="other">The value to subtract from this value.</param>
        /// <returns>The difference between this value and the other value.</returns>
        public Int Sub(Int other) => new(this.Value - other.Value);

        /// <summary>This will add this data to the other data.</summary>
        /// <param name="other">The other data to add to this value.</param>
        /// <returns>The sum of the two data values.</returns>
        public Int Sum(Int other) => new(this.Value + other.Value);

        /// <summary>Gets the power of this value to the other value.</summary>
        /// <param name="other">The value to use as the exponent.</param>
        /// <returns>The power of this value to the other value.</returns>
        public Int Pow(Int other) => new((int)S.Math.Pow(this.Value, other.Value));

        /// <summary>This gets the bitwise NOT of this data value.</summary>
        /// <returns>The bitwise NOT of this value.</returns>
        public Int BitwiseNot() => new(~this.Value);

        /// <summary>This gets the bitwise AND of this value and the other value.</summary>
        /// <param name="other">The other value to bitwise AND with.</param>
        /// <returns>The result of the bitwise AND.</returns>
        public Int BitwiseAnd(Int other) => new(this.Value & other.Value);

        /// <summary>This gets the bitwise OR of this value and the other value.</summary>
        /// <param name="other">The other value to bitwise OR with.</param>
        /// <returns>The result of the bitwise OR.</returns>
        public Int BitwiseOr(Int other) => new(this.Value | other.Value);

        /// <summary>This gets the bitwise XOR of this value and the other value.</summary>
        /// <param name="other">The other value to bitwise XOR with.</param>
        /// <returns>The result of the bitwise XOR.</returns>
        public Int BitwiseXor(Int other) => new(this.Value ^ other.Value);

        /// <summary>This gets the left shift of this value by the other value.</summary>
        /// <param name="other">The number of bits to shift by.</param>
        /// <returns>The left shifted value.</returns>
        public Int LeftShift(Int other) => new(this.Value << other.Value);

        /// <summary>This gets the right shift of this value by the other value.</summary>
        /// <param name="other">The number of bits to shift by.</param>
        /// <returns>The right shifted value.</returns>
        public Int RightShift(Int other) => new(this.Value >> other.Value);

        /// <summary>Casts a double into an int for an explicit cast.</summary>
        /// <param name="value">The double value to cast.</param>
        /// <returns>The resulting integer value.</returns>
        public Int CastFrom(Double value) => new((int)value.Value);

        /// <summary>Compares two integers together.</summary>
        /// <param name="other">The other integer to compare.</param>
        /// <returns>The comparison result indicating which is greater than or equal.</returns>
        public int CompareTo(Int other) => this.Value.CompareTo(other.Value);
        public static bool operator ==(Int left, Int right) => left.CompareTo(right) == 0;
        public static bool operator !=(Int left, Int right) => left.CompareTo(right) != 0;
        public static bool operator <(Int left, Int right) => left.CompareTo(right) < 0;
        public static bool operator <=(Int left, Int right) => left.CompareTo(right) <= 0;
        public static bool operator >(Int left, Int right) => left.CompareTo(right) > 0;
        public static bool operator >=(Int left, Int right) => left.CompareTo(right) >= 0;

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is Int other && this.Value == other.Value;

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>Gets the name of this data type.</summary>
        /// <returns>The name of the bool type.</returns>
        public override string ToString() => "int";
    }
}
