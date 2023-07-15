using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Data.Caps;

/// <summary>This is the data storage for a 32 bit unsigned integer value such that it can be used in generics.</summary>
public readonly struct Uint :
    IAdditive<Uint>,
    IBitwise<Uint>,
    IComparable<Uint>,
    IData,
    IDivisible<Uint>,
    IEquatable<Uint>,
    IFinite<Uint>,
    IMultiplicative<Uint>,
    ISubtractive<Uint>,
    IExplicit<Int, Uint>,
    IExplicit<Double, Uint>,
    IExplicit<Object, Uint> {

    #region Static...

    /// <summary>Gets this additive identity, zero.</summary>
    static public readonly Uint Zero = new(0);

    /// <summary>Gets this multiplicative identity, one.</summary>
    static public readonly Uint One = new(1);

    /// <summary>Gets the minimum value for this data type.</summary>
    static public readonly Uint Min = new(uint.MinValue);

    /// <summary>Gets the maximum value for this data type.</summary>
    static public readonly Uint Max = new(uint.MaxValue);

    /// <summary>The identity of summation for this data type.</summary>
    static public readonly Uint SumIdentity = Zero;

    /// <summary>The identity of multiplication for this data type.</summary>
    static public readonly Uint MulIdentity = One;

    /// <summary>The identity of AND for this data type.</summary>
    static public readonly Uint AndIdentity = Max;

    /// <summary>The identity of OR for this data type.</summary>
    static public readonly Uint OrIdentity = Zero;

    #endregion

    /// <summary>The unsigned integer value being stored.</summary>
    public readonly uint Value;

    /// <summary>Creates a new unsigned integer data value.</summary>
    /// <param name="value">The unsigned integer value to store.</param>
    public Uint(uint value) => this.Value = value;

    #region Additive...

    /// <summary>Gets the difference between the first given value and the rest of the other values.</summary>
    /// <remarks>The current value is not used in the subtraction.</remarks>
    /// <param name="other">The values to subtract from the first value.</param>
    /// <returns>The difference between the first value and the rest of the values.</returns>s
    public Uint Sum(IEnumerable<Uint> other) => new((uint)other.Sum(t => t.Value));

    /// <summary>
    /// Indicates that for this data type, summation is commutable,
    /// meaning that the order of the parents makes no difference to the result.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
    public bool SumCommutable => true;

    /// <summary>The identity of summation for this data type.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Uint SumIdentityValue => SumIdentity;

    #endregion
    #region Bitwise...

    /// <summary>This gets the bitwise NOT of this data value.</summary>
    /// <returns>The bitwise NOT of this value.</returns>
    public Uint BitwiseNot() => new(~this.Value);

    /// <summary>This gets the bitwise AND of the other values.</summary>
    /// <remarks>This does not use the current value in the AND.</remarks>
    /// <param name="other">The other values to bitwise AND with.</param>
    /// <returns>The result of the bitwise AND.</returns>
    public Uint BitwiseAnd(IEnumerable<Uint> other) => new(other.Aggregate(uint.MaxValue, (t1, t2) => t1 & t2.Value));

    /// <summary>This gets the bitwise OR of the other values.</summary>
    /// <remarks>This does not use the current value in the OR.</remarks>
    /// <param name="other">The other values to bitwise OR with.</param>
    /// <returns>The result of the bitwise OR.</returns>
    public Uint BitwiseOr(IEnumerable<Uint> other) => new(other.Aggregate((uint)0, (t1, t2) => t1 | t2.Value));

    /// <summary>This gets the bitwise XOR of the other values.</summary>
    /// <remarks>This does not use the current value in the XOR.</remarks>
    /// <param name="other">The other values to bitwise XOR with.</param>
    /// <returns>The result of the bitwise XOR.</returns>
    public Uint BitwiseXor(IEnumerable<Uint> other) => new(other.Aggregate((uint)0, (t1, t2) => t1 ^ t2.Value));

    /// <summary>The identity of AND for this data type.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Uint AndIdentityValue => AndIdentity;

    /// <summary>The identity of OR for this data type.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Uint OrIdentityValue => OrIdentity;

    /// <summary>This gets the left shift of this value by the other value.</summary>
    /// <param name="other">The number of bits to shift by.</param>
    /// <returns>The left shifted value.</returns>
    public Uint LeftShift(Int other) => new(this.Value << other.Value);

    /// <summary>This gets the right shift of this value by the other value.</summary>
    /// <param name="other">The number of bits to shift by.</param>
    /// <returns>The right shifted value.</returns>
    public Uint RightShift(Int other) => new(this.Value >> other.Value);

    #endregion
    #region Comparable...

    public static bool operator < (Uint left, Uint right) => left.CompareTo(right) <  0;
    public static bool operator <=(Uint left, Uint right) => left.CompareTo(right) <= 0;
    public static bool operator > (Uint left, Uint right) => left.CompareTo(right) >  0;
    public static bool operator >=(Uint left, Uint right) => left.CompareTo(right) >= 0;

    /// <summary>Compares two unsigned integers together.</summary>
    /// <param name="other">The other unsigned integer to compare.</param>
    /// <returns>The comparison result indicating which is greater than, less than, or equal.</returns>
    public int CompareTo(Uint other) => this.Value.CompareTo(other.Value);

    #endregion
    #region Data...

    /// <summary>Gets the name for the type of data.</summary>
    public string TypeName => Type.Uint.Name;

    /// <summary>Get the value of the data as a string.</summary>
    public string ValueAsString => this.Value.ToString();

    /// <summary>Get the value of the data as an object.</summary>
    public object ValueAsObject => this.Value;

    #endregion
    #region Divisible...

    /// <summary>Gets the division of this value and the other value.</summary>
    /// <param name="other">The value to divide this value with.</param>
    /// <returns>This value divided by the other value.</returns>
    public Uint Div(Uint other) => new(this.Value / other.Value);

    /// <summary>Gets the modulo of this value and the other value.</summary>
    /// <remarks>The result will have the same sign as this value.</remarks>
    /// <param name="other">The value to mod this value with.</param>
    /// <returns>The modulo of this value and the other value.</returns>
    public Uint Mod(Uint other) => new(this.Value % other.Value);

    #endregion
    #region Equatable...

    public static bool operator ==(Uint left, Uint right) =>  left.Equals(right);
    public static bool operator !=(Uint left, Uint right) => !left.Equals(right);

    /// <summary>Checks if the given unsigned integer is equal to this data type.</summary>
    /// <param name="other">This is the integer to test.</param>
    /// <returns>True if they are equal, otherwise false.</returns>
    public bool Equals(Uint other) => this.Value.Equals(other.Value);

    #endregion
    #region Finite...

    /// <summary>Gets this additive identity, zero.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The identity data value.</returns>
    public Uint ZeroValue => Zero;

    /// <summary>Gets this multiplicative identity, one.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The identity data value.</returns>
    public Uint OneValue => One;

    /// <summary>Gets the minimum value for this data type.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The minimum data value.</returns>
    public Uint MinValue => Min;

    /// <summary>Gets the maximum value for this data type.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The maximum data value.</returns>
    public Uint MaxValue => Max;

    #endregion
    #region Multiplicative...

    /// <summary>Gets the product of this value and the other values.</summary>
    /// <remarks>The current value is not used in the product.</remarks>
    /// <param name="other">The values to multiply this value with.</param>
    /// <returns>The product of this value and the other values.</returns>
    public Uint Mul(IEnumerable<Uint> other) => new(other.Aggregate((uint)1, (t1, t2) => t1 * t2.Value));

    /// <summary>
    /// Indicates that for this data type, multiplication is commutable,
    /// meaning that the order of the parents makes no difference to the result.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
    public bool MulCommutable => true;

    /// <summary>The identity of multiplication for this data type.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Uint MulIdentityValue => MulIdentity;

    #endregion
    #region Subtractive...

    /// <summary>Gets the difference between this value and the other value.</summary>
    /// <param name="other">The value to subtract from this value.</param>
    /// <returns>The difference between this value and the other value.</returns>
    public Uint Sub(Uint other) => new(this.Value - other.Value);

    #endregion
    #region Casts...

    /// <summary>Casts a signed integer into an unsigned integer for an explicit cast.</summary>
    /// <param name="value">The integer value to cast.</param>
    /// <returns>The resulting unsigned integer value.</returns>
    public Uint CastFrom(Int value) => new((uint)value.Value);

    /// <summary>Casts a double into an unsigned integer for an explicit cast.</summary>
    /// <param name="value">The double value to cast.</param>
    /// <returns>The resulting unsigned integer value.</returns>
    public Uint CastFrom(Double value) => new((uint)value.Value);

    /// <summary>Casts an object into an unsigned integer for an explicit cast.</summary>
    /// <param name="value">The object value to cast.</param>
    /// <returns>The resulting unsigned integer value.</returns>
    public Uint CastFrom(Object value) => new(value.CastTo<uint>(this.TypeName));

    #endregion

    /// <summary>Gets the hash code of the stored value.</summary>
    /// <returns>The stored value's hash code.</returns>
    public override int GetHashCode() => this.Value.GetHashCode();

    /// <summary>Checks if the given object is equal to this data type.</summary>
    /// <param name="obj">This is the object to test.</param>
    /// <returns>True if they are equal, otherwise false.</returns>
    public override bool Equals(object? obj) => obj is Uint other && this.Equals(other);

    /// <summary>Gets the name of this data type and value.</summary>
    /// <returns>The name of the integer type and value.</returns>
    public override string ToString() => this.TypeName+"("+this.ValueAsString+")";
}
