using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Data.Caps;

/// <summary>This is the data storage for a 32 bit signed integer value such that it can be used in generics.</summary>
public readonly struct Int :
    IAdditive<Int>,
    IBaseValue<int>,
    IBitwise<Int>,
    IComparable<Int>,
    IData,
    IDivisible<Int>,
    IEquatable<Int>,
    IFinite<Int>,
    IMultiplicative<Int>,
    ISigned<Int>,
    ISubtractive<Int>,
    IExplicit<Uint, Int>,
    IExplicit<Float, Int>,
    IExplicit<Double, Int>,
    IExplicit<Object, Int> {

    #region Static...

    /// <summary>Gets this additive identity, zero.</summary>
    static public readonly Int Zero = new(0);

    /// <summary>Gets this multiplicative identity, one.</summary>
    static public readonly Int One = new(1);

    /// <summary>Gets the minimum value for this data type.</summary>
    static public readonly Int Min = new(int.MinValue);

    /// <summary>Gets the maximum value for this data type.</summary>
    static public readonly Int Max = new(int.MaxValue);

    /// <summary>The identity of summation for this data type.</summary>
    static public readonly Int SumIdentity = Zero;

    /// <summary>The identity of multiplication for this data type.</summary>
    static public readonly Int MulIdentity = One;

    /// <summary>The identity of AND for this data type.</summary>
    static public readonly Int AndIdentity = Max;

    /// <summary>The identity of OR for this data type.</summary>
    static public readonly Int OrIdentity = Zero;

    #endregion

    /// <summary>The integer value being stored.</summary>
    public readonly int Value;

    /// <summary>Creates a new integer data value.</summary>
    /// <param name="value">The integer value to store.</param>
    public Int(int value) => this.Value = value;

    #region Additive...

    /// <summary>Gets the difference between the first given value and the rest of the other values.</summary>
    /// <remarks>The current value is not used in the subtraction.</remarks>
    /// <param name="other">The values to subtract from the first value.</param>
    /// <returns>The difference between the first value and the rest of the values.</returns>s
    public Int Sum(IEnumerable<Int> other) => new(other.Sum(t => t.Value));

    /// <summary>
    /// Indicates that for this data type, summation is commutable,
    /// meaning that the order of the parents makes no difference to the result.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
    public bool SumCommutable => true;

    /// <summary>The identity of summation for this data type.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Int SumIdentityValue => SumIdentity;

    #endregion
    #region BaseValue...

    /// <summary>Gets the C# base value in the data.</summary>
    public int BaseValue => this.Value;

    #endregion
    #region Bitwise...

    /// <summary>This gets the bitwise NOT of this data value.</summary>
    /// <returns>The bitwise NOT of this value.</returns>
    public Int BitwiseNot() => new(~this.Value);

    /// <summary>This gets the bitwise AND of the other values.</summary>
    /// <remarks>This does not use the current value in the AND.</remarks>
    /// <param name="other">The other values to bitwise AND with.</param>
    /// <returns>The result of the bitwise AND.</returns>
    public Int BitwiseAnd(IEnumerable<Int> other) => new(other.Aggregate(int.MaxValue, (t1, t2) => t1 & t2.Value));

    /// <summary>This gets the bitwise OR of the other values.</summary>
    /// <remarks>This does not use the current value in the OR.</remarks>
    /// <param name="other">The other values to bitwise OR with.</param>
    /// <returns>The result of the bitwise OR.</returns>
    public Int BitwiseOr(IEnumerable<Int> other) => new(other.Aggregate(0, (t1, t2) => t1 | t2.Value));

    /// <summary>This gets the bitwise XOR of the other values.</summary>
    /// <remarks>This does not use the current value in the XOR.</remarks>
    /// <param name="other">The other values to bitwise XOR with.</param>
    /// <returns>The result of the bitwise XOR.</returns>
    public Int BitwiseXor(IEnumerable<Int> other) => new(other.Aggregate(0, (t1, t2) => t1 ^ t2.Value));

    /// <summary>The identity of AND for this data type.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Int AndIdentityValue => AndIdentity;

    /// <summary>The identity of OR for this data type.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Int OrIdentityValue => OrIdentity;

    /// <summary>This gets the left shift of this value by the other value.</summary>
    /// <param name="other">The number of bits to shift by.</param>
    /// <returns>The left shifted value.</returns>
    public Int LeftShift(Int other) => new(this.Value << other.Value);

    /// <summary>This gets the right shift of this value by the other value.</summary>
    /// <param name="other">The number of bits to shift by.</param>
    /// <returns>The right shifted value.</returns>
    public Int RightShift(Int other) => new(this.Value >> other.Value);

    #endregion
    #region Comparable...

    public static bool operator < (Int left, Int right) => left.CompareTo(right) <  0;
    public static bool operator <=(Int left, Int right) => left.CompareTo(right) <= 0;
    public static bool operator > (Int left, Int right) => left.CompareTo(right) >  0;
    public static bool operator >=(Int left, Int right) => left.CompareTo(right) >= 0;

    /// <summary>Compares two integers together.</summary>
    /// <param name="other">The other integer to compare.</param>
    /// <returns>The comparison result indicating which is greater than, less than, or equal.</returns>
    public int CompareTo(Int other) => this.Value.CompareTo(other.Value);

    #endregion
    #region Data...

    /// <summary>Gets the type for the type of data.</summary>
    public Type Type => Type.Int;

    /// <summary>Get the value of the data as a string.</summary>
    public string ValueAsString => this.Value.ToString();

    /// <summary>Get the value of the data as an object.</summary>
    public object ValueAsObject => this.Value;

    #endregion
    #region Divisible...

    /// <summary>Gets the division of this value and the other value.</summary>
    /// <param name="other">The value to divide this value with.</param>
    /// <returns>This value divided by the other value.</returns>
    public Int Div(Int other) => new(this.Value / other.Value);

    /// <summary>Gets the modulo of this value and the other value.</summary>
    /// <remarks>The result will have the same sign as this value.</remarks>
    /// <param name="other">The value to mod this value with.</param>
    /// <returns>The modulo of this value and the other value.</returns>
    public Int Mod(Int other) => new(this.Value % other.Value);

    #endregion
    #region Equatable...

    public static bool operator ==(Int left, Int right) =>  left.Equals(right);
    public static bool operator !=(Int left, Int right) => !left.Equals(right);

    /// <summary>Checks if the given integer is equal to this data type.</summary>
    /// <param name="other">This is the integer to test.</param>
    /// <returns>True if they are equal, otherwise false.</returns>
    public bool Equals(Int other) => this.Value.Equals(other.Value);

    #endregion
    #region Finite...

    /// <summary>Gets this additive identity, zero.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The identity data value.</returns>
    public Int ZeroValue => Zero;

    /// <summary>Gets this multiplicative identity, one.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The identity data value.</returns>
    public Int OneValue => One;

    /// <summary>Gets the minimum value for this data type.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The minimum data value.</returns>
    public Int MinValue => Min;

    /// <summary>Gets the maximum value for this data type.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The maximum data value.</returns>
    public Int MaxValue => Max;

    #endregion
    #region Multiplicative...

    /// <summary>Gets the product of this value and the other values.</summary>
    /// <remarks>The current value is not used in the product.</remarks>
    /// <param name="other">The values to multiply this value with.</param>
    /// <returns>The product of this value and the other values.</returns>
    public Int Mul(IEnumerable<Int> other) => new(other.Aggregate(1, (t1, t2) => t1 * t2.Value));

    /// <summary>
    /// Indicates that for this data type, multiplication is commutable,
    /// meaning that the order of the parents makes no difference to the result.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
    public bool MulCommutable => true;

    /// <summary>The identity of multiplication for this data type.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Int MulIdentityValue => MulIdentity;

    #endregion
    #region Signed...

    /// <summary>Gets the absolute value of this data value.</summary>
    /// <returns>The absolute value of this value.</returns>
    public Int Abs() => new(S.Math.Abs(this.Value));

    /// <summary>Gets the absolute value of this data value.</summary>
    /// <returns>The absolute value of this value.</returns>
    public Int Neg() => new(-this.Value);

    /// <summary>Determines if the this value is negative.</summary>
    /// <returns>True if below zero, false if zero or more.</returns>
    public Bool IsNegative() => new(this.Value < 0);

    #endregion
    #region Subtractive...

    /// <summary>Gets the difference between this value and the other value.</summary>
    /// <param name="other">The value to subtract from this value.</param>
    /// <returns>The difference between this value and the other value.</returns>
    public Int Sub(Int other) => new(this.Value - other.Value);

    #endregion
    #region Casts...

    /// <summary>Casts an unsigned integer into an int for an explicit cast.</summary>
    /// <param name="value">The unsigned integer value to cast.</param>
    /// <returns>The resulting integer value.</returns>
    public Int CastFrom(Uint value) => new((int)value.Value);

    /// <summary>Casts a float into an int for an explicit cast.</summary>
    /// <param name="value">The float value to cast.</param>
    /// <returns>The resulting integer value.</returns>
    public Int CastFrom(Float value) => new((int)value.Value);

    /// <summary>Casts a double into an int for an explicit cast.</summary>
    /// <param name="value">The double value to cast.</param>
    /// <returns>The resulting integer value.</returns>
    public Int CastFrom(Double value) => new((int)value.Value);

    /// <summary>Casts an object into an int for an explicit cast.</summary>
    /// <param name="value">The object value to cast.</param>
    /// <returns>The resulting integer value.</returns>
    public Int CastFrom(Object value) => new(value.CastTo<int>(this.Type.Name));

    #endregion

    /// <summary>Gets the hash code of the stored value.</summary>
    /// <returns>The stored value's hash code.</returns>
    public override int GetHashCode() => this.Value.GetHashCode();

    /// <summary>Checks if the given object is equal to this data type.</summary>
    /// <param name="obj">This is the object to test.</param>
    /// <returns>True if they are equal, otherwise false.</returns>
    public override bool Equals(object? obj) => obj is Int other && this.Equals(other);

    /// <summary>Gets the name of this data type and value.</summary>
    /// <returns>The name of the integer type and value.</returns>
    public override string ToString() => this.Type.Name+"("+this.ValueAsString+")";
}
