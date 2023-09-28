using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Data.Caps;

/// <summary>This is the data storage for an IEEE 754 float value such that it can be used in generics.</summary>
public readonly struct Float :
    IAdditive<Float>,
    IBaseValue<Float, float>,
    IComparable<Float>,
    IData,
    IDivisible<Float>,
    IEquatable<Float>,
    IFinite<Float>,
    IFloatingPoint<Float>,
    IMultiplicative<Float>,
    ISigned<Float>,
    ISubtractive<Float>,
    IImplicit<Int, Float>,
    IImplicit<Uint, Float>,
    IExplicit<Double, Float>,
    IExplicit<Object, Float> {

    #region Static...

    /// <summary>Gets this additive identity, zero.</summary>
    static public readonly Float Zero = new(0.0f);

    /// <summary>Gets this multiplicative identity, one.</summary>
    static public readonly Float One = new(1.0f);

    /// <summary>Gets the minimum value for this data type.</summary>
    static public readonly Float Min = new(float.MinValue);

    /// <summary>Gets the maximum value for this data type.</summary>
    static public readonly Float Max = new(float.MaxValue);

    /// <summary>The identity of summation for this data type.</summary>
    static public readonly Float SumIdentity = Zero;

    /// <summary>The identity of multiplication for this data type.</summary>
    static public readonly Float MulIdentity = One;

    /// <summary>This gets the positive infinity value.</summary>
    static public readonly Float Infinity = new(float.PositiveInfinity);

    /// <summary>Determines if this value is not a number.</summary>
    static public readonly Float NaN = new(float.NaN);

    #endregion

    /// <summary>The float value being stored.</summary>
    public readonly float Value;

    /// <summary>Creates a new float data value.</summary>
    /// <param name="value">The float value to store.</param>
    public Float(float value) => this.Value = value;

    #region Additive...

    /// <summary>Gets the difference between the first given value and the rest of the other values.</summary>
    /// <remarks>The current value is not used in the subtraction.</remarks>
    /// <param name="other">The values to subtract from the first value.</param>
    /// <returns>The difference between the first value and the rest of the values.</returns>s
    public Float Sum(IEnumerable<Float> other) => new(other.Sum(t => t.Value));

    /// <summary>
    /// Indicates that for this data type, summation is commutable,
    /// meaning that the order of the parents makes no difference to the result.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
    public bool SumCommutable => true;

    /// <summary>The identity of summation for this data type.</summary>
    /// <remarks>The current value is not used in the product.</remarks>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Float SumIdentityValue => SumIdentity;

    #endregion
    #region BaseValue...

    /// <summary>Gets the C# base value in the data.</summary>
    public float BaseValue => this.Value;
    
    /// <summary>This creates a new instance of the data with the given value.</summary>
    /// <param name="baseValue">The value to create data for.</param>
    /// <returns>The new data set for the given value.</returns>
    public Float Wrap(float baseValue) => new(baseValue);

    #endregion
    #region Comparable...

    public static bool operator < (Float left, Float right) => left.CompareTo(right) <  0;
    public static bool operator <=(Float left, Float right) => left.CompareTo(right) <= 0;
    public static bool operator > (Float left, Float right) => left.CompareTo(right) >  0;
    public static bool operator >=(Float left, Float right) => left.CompareTo(right) >= 0;

    /// <summary>Compares two floats together.</summary>
    /// <param name="other">The other float to compare.</param>
    /// <returns>The comparison result indicating which is greater than, less than, or equal.</returns>
    public int CompareTo(Float other) => this.Value.CompareTo(other.Value);

    #endregion
    #region Data...

    /// <summary>Gets the type for the type of data.</summary>
    public Type Type => Type.Float;

    /// <summary>Get the value of the data as a string.</summary>
    public string ValueAsString => this.Value.ToString();

    /// <summary>Get the value of the data as an object.</summary>
    public object ValueAsObject => this.Value;

    #endregion
    #region Divisible...

    /// <summary>Gets the division of this value and the other value.</summary>
    /// <param name="other">The value to divide this value with.</param>
    /// <returns>This value divided by the other value.</returns>
    public Float Div(Float other) => new(this.Value / other.Value);

    /// <summary>Gets the modulo of this value and the other value.</summary>
    /// <remarks>The result will have the same sign as this value.</remarks>
    /// <param name="other">The value to mod this value with.</param>
    /// <returns>The modulo of this value and the other value.</returns>
    public Float Mod(Float other) => new(this.Value % other.Value);

    #endregion
    #region Equatable...

    public static bool operator ==(Float left, Float right) =>  left.Equals(right);
    public static bool operator !=(Float left, Float right) => !left.Equals(right);

    /// <summary>Checks if the given float is equal to this data type.</summary>
    /// <param name="other">This is the float to test.</param>
    /// <returns>True if they are equal, otherwise false.</returns>
    public bool Equals(Float other) => this.Value.Equals(other.Value);

    #endregion
    #region Finite...

    /// <summary>Gets this additive identity, zero.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    public Float ZeroValue => Zero;

    /// <summary>Gets this multiplicative identity, one.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    public Float OneValue => One;

    /// <summary>Gets the minimum value for this data type.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    public Float MinValue => Min;

    /// <summary>Gets the maximum value for this data type.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    public Float MaxValue => Max;

    #endregion
    #region Floating Point...

    /// <summary>This gets the linear interpolation between to points using this value as a factor.</summary>
    /// <param name="min">The minimum value for a factor of zero or less.</param>
    /// <param name="max">The maximum value for a factor of one or more.</param>
    /// <returns>The result of the linear interpolation.</returns>
    public Float Lerp(Float min, Float max) {
        float i = this.Value;
        return i <= 0.0f ? min :
               i >= 1.0f ? max :
               new((1.0f - i)*min.Value + i*max.Value);
    }

    /// <summary>This rounds this value to the given decimals.</summary>
    /// <param name="decimals">The integer value to round to.</param>
    /// <returns>This value rounded to the given decimals.</returns>
    public Float Round(Int decimals) => new((float)S.Math.Round(this.Value, decimals.Value));

    /// <summary>This performs the given function on this value.</summary>
    /// <param name="func">The function to run on this value.</param>
    /// <returns>The resulting value from this value being used in the given function.</returns>
    public Float DoubleMath(S.Func<double, double> func) => new((float)func(this.Value));

    /// <summary>This performs the given function on this value.</summary>
    /// <param name="other">The value to use as the second input to the function.</param>
    /// <param name="func">The function to run on this and the given value.</param>
    /// <returns>The resulting value from this and the other value being used in the given function.</returns>
    public Float DoubleMath(Float other, S.Func<double, double, double> func) => new((float)func(this.Value, other.Value));

    /// <summary>Determines if this value is positive or negative infinity.</summary>
    /// <returns>True if the number is either positive or negative infinity, false otherwise.</returns>
    public Bool IsInfinity() => new(float.IsInfinity(this.Value));

    /// <summary>Determines if this value is not a number.</summary>
    /// <returns>True if the number is not a number, false otherwise.</returns>
    public Bool IsNaN() => new(float.IsNaN(this.Value));

    /// <summary>Determines if the value is not infinite.</summary>
    /// <returns>True if the number is finite, false otherwise.</returns>
    public Bool IsFinite() => new(float.IsFinite(this.Value));

    /// <summary>Determines if the value is positive infinity.</summary>
    /// <returns>True if the number is positive infinity, false otherwise.</returns>
    public Bool IsPositiveInfinity() => new(float.IsPositiveInfinity(this.Value));

    /// <summary>Determines if the value is negative infinity.</summary>
    /// <returns>True if the number is negative infinity, false otherwise.</returns>
    public Bool IsNegativeInfinity() => new(float.IsNegativeInfinity(this.Value));

    /// <summary>Determines if the value is in the normal range of floats with full precision.</summary>
    /// <returns>True if the number is normal, false otherwise.</returns>
    /// <see cref="https://en.wikipedia.org/wiki/Subnormal_number"/>
    public Bool IsNormal() => new(float.IsNormal(this.Value));

    /// <summary>Determines if the value is not in the normal range of float so have reduced precision.</summary>
    /// <returns>True if the subnormal is normal, false otherwise.</returns>
    /// <see cref="https://en.wikipedia.org/wiki/Subnormal_number"/>
    public Bool IsSubnormal() => new(float.IsSubnormal(this.Value));

    /// <summary>This gets the positive infinity value.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    public Float InfinityValue => Infinity;

    /// <summary>Determines if this value is not a number.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    public Float NaNValue => NaN;

    #endregion
    #region Multiplicative...

    /// <summary>Gets the product of this value and the other value.</summary>
    /// <remarks>The current value is not used in the product.</remarks>
    /// <param name="other">The values to multiply this value with.</param>
    /// <returns>The product of this value and the other values.</returns>
    public Float Mul(IEnumerable<Float> other) => new(other.Aggregate(1.0f, (t1, t2) => t1 * t2.Value));

    /// <summary>
    /// Indicates that for this data type, multiplication is commutable,
    /// meaning that the order of the parents makes no difference to the result.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
    public bool MulCommutable => true;

    /// <summary>The identity of multiplication for this data type.</summary>
    /// <remarks>The current value is not used in the product.</remarks>
    /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
    public Float MulIdentityValue => MulIdentity;

    #endregion
    #region Signed...

    /// <summary>Gets the absolute value of this data value.</summary>
    /// <returns>The absolute value of this value.</returns>
    public Float Abs() => new(S.Math.Abs(this.Value));

    /// <summary>Gets the absolute value of this data value.</summary>
    /// <returns>The absolute value of this value.</returns>
    public Float Neg() => new(-this.Value);

    /// <summary>Determines if the this value is negative.</summary>
    /// <returns>True if below zero, false if zero or more.</returns>
    public Bool IsNegative() => new(float.IsNegative(this.Value));

    #endregion
    #region Subtractive...

    /// <summary>Gets the difference between this value and the other value.</summary>
    /// <param name="other">The value to subtract from this value.</param>
    /// <returns>The difference between this value and the other value.</returns>
    public Float Sub(Float other) => new(this.Value - other.Value);

    #endregion
    #region Casts...

    /// <summary>Casts an integer into a float for an implicit cast.</summary>
    /// <param name="value">The integer value to cast.</param>
    /// <returns>The resulting float value.</returns>
    public Float CastFrom(Int value) => new(value.Value);

    /// <summary>Casts an unsigned integer into a float for an implicit cast.</summary>
    /// <param name="value">The integer value to cast.</param>
    /// <returns>The resulting float value.</returns>
    public Float CastFrom(Uint value) => new(value.Value);

    /// <summary>Casts a double into a float for an explicit cast.</summary>
    /// <param name="value">The double value to cast.</param>
    /// <returns>The resulting float value.</returns>
    public Float CastFrom(Double value) => new((float)value.Value);

    /// <summary>Casts an object into a float for an explicit cast.</summary>
    /// <param name="value">The object value to cast.</param>
    /// <returns>The resulting float value.</returns>
    public Float CastFrom(Object value) => new(value.CastTo<float>(this.Type.Name));

    #endregion

    /// <summary>Gets the hash code of the stored value.</summary>
    /// <returns>The stored value's hash code.</returns>
    public override int GetHashCode() => this.Value.GetHashCode();

    /// <summary>Checks if the given object is equal to this data type.</summary>
    /// <param name="obj">This is the object to test.</param>
    /// <returns>True if they are equal, otherwise false.</returns>
    public override bool Equals(object? obj) => obj is Float other && this.Equals(other);

    /// <summary>Gets the name of this data type and value.</summary>
    /// <returns>The name of the float type and value.</returns>
    public override string ToString() => this.Type.Name+"("+this.ValueAsString+")";
}
