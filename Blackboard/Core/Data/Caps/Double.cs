using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for an IEEE 754 double value such that it can be used in generics.</summary>
    public struct Double:
        IAdditive<Double>,
        IComparable<Double>,
        IData,
        IDivisible<Double>,
        IEquatable<Double>,
        IFinite<Double>,
        IFloatingPoint<Double>,
        IMultiplicative<Double>,
        ISigned<Double>,
        ISubtractive<Double>,
        IImplicit<Int, Double> {

        #region Static...

        /// <summary>Gets this additive identity, zero.</summary>
        static public readonly Double Zero = new(0.0);

        /// <summary>Gets this multiplicative identity, one.</summary>
        static public readonly Double One = new(1.0);

        /// <summary>Gets the minimum value, false, for this data type.</summary>
        static public readonly Double Min = new(double.MinValue);

        /// <summary>Gets the maximum value, true, for this data type.</summary>
        static public readonly Double Max = new(double.MaxValue);

        /// <summary>The identity of summation for this data type.</summary>
        static public readonly Double SumIdentity = Zero;

        /// <summary>The identity of multiplication for this data type.</summary>
        static public readonly Double MulIdentity = One;

        /// <summary>This gets the positive infinity value.</summary>
        static public readonly Double Infinity = new(double.PositiveInfinity);

        /// <summary>Determines if this value is not a number.</summary>
        static public readonly Double NaN = new(double.NaN);

        #endregion

        /// <summary>The double value being stored.</summary>
        public readonly double Value;

        /// <summary>Creates a new double data value.</summary>
        /// <param name="value">The double value to store.</param>
        public Double(double value) => this.Value = value;

        #region Additive...

        /// <summary>Gets the difference between the first given value and the rest of the other values.</summary>
        /// <remarks>The current value is not used in the subtraction.</remarks>
        /// <param name="other">The values to subtract from the first value.</param>
        /// <returns>The difference between the first value and the rest of the values.</returns>s
        public Double Sum(IEnumerable<Double> other) => new(other.Sum(t => t.Value));

        /// <summary>
        /// Indicates that for this data type, summation is commutable,
        /// meaning that the order of the parents makes no difference to the result.
        /// </summary>
        /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
        public bool SumCommutable => true;

        /// <summary>The identity of summation for this data type.</summary>
        /// <remarks>The current value is not used in the product.</remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
        public Double SumIdentityValue => SumIdentity;

        #endregion
        #region Comparable...

        public static bool operator < (Double left, Double right) => left.CompareTo(right) <  0;
        public static bool operator <=(Double left, Double right) => left.CompareTo(right) <= 0;
        public static bool operator > (Double left, Double right) => left.CompareTo(right) >  0;
        public static bool operator >=(Double left, Double right) => left.CompareTo(right) >= 0;

        /// <summary>Compares two doubles together.</summary>
        /// <param name="other">The other double to compare.</param>
        /// <returns>The comparison result indicating which is greater than, less than, or equal.</returns>
        public int CompareTo(Double other) => this.Value.CompareTo(other.Value);

        #endregion
        #region Data...

        /// <summary>Gets the name for the type of data.</summary>
        public string TypeName => Type.Double.Name;

        /// <summary>Get the value of the data as a string.</summary>
        public string ValueString => this.Value.ToString();

        #endregion
        #region Divisible...

        /// <summary>Gets the division of this value and the other value.</summary>
        /// <param name="other">The value to divide this value with.</param>
        /// <returns>This value divided by the other value.</returns>
        public Double Div(Double other) => new(this.Value / other.Value);

        /// <summary>Gets the modulo of this value and the other value.</summary>
        /// <remarks>The result will have the same sign as this value.</remarks>
        /// <param name="other">The value to mod this value with.</param>
        /// <returns>The modulo of this value and the other value.</returns>
        public Double Mod(Double other) => new(this.Value % other.Value);

        #endregion
        #region Equatable...

        public static bool operator ==(Double left, Double right) => left.Equals(right);
        public static bool operator !=(Double left, Double right) => !left.Equals(right);

        /// <summary>Checks if the given double is equal to this data type.</summary>
        /// <param name="other">This is the double to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public bool Equals(Double other) => this.Value == other.Value;

        #endregion
        #region Finite...

        /// <summary>Gets this additive identity, zero.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        public Double ZeroValue => Zero;

        /// <summary>Gets this multiplicative identity, one.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        public Double OneValue => One;

        /// <summary>Gets the minimum value for this data type.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        public Double MinValue => Min;

        /// <summary>Gets the maximum value for this data type.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        public Double MaxValue => Max;

        #endregion
        #region Floating Point...

        /// <summary>This gets the linear interpolation between to points using this value as a factor.</summary>
        /// <param name="min">The minimum value for a factor of zero or less.</param>
        /// <param name="max">The maximum value for a factor of one or more.</param>
        /// <returns>The result of the linear interpolation.</returns>
        public Double Lerp(Double min, Double max) {
            double i = this.Value;
            return i <= 0.0 ? min :
                   i >= 1.0 ? max :
                   new((1.0 - i)*min.Value + i*max.Value);
        }

        /// <summary>This rounds this value to the given decimals.</summary>
        /// <param name="decimals">The integer value to round to.</param>
        /// <returns>This value rounded to the given decimals.</returns>
        public Double Round(Int decimals) => new(S.Math.Round(this.Value, decimals.Value));

        /// <summary>This performs the given function on this value.</summary>
        /// <param name="func">The function to run on this value.</param>
        /// <returns>The resulting value from this value being used in the given function.</returns>
        public Double DoubleMath(S.Func<double, double> func) => new(func(this.Value));

        /// <summary>This performs the given function on this value.</summary>
        /// <param name="other">The value to use as the second input to the function.</param>
        /// <param name="func">The function to run on this and the given value.</param>
        /// <returns>The resulting value from this and the other value being used in the given function.</returns>
        public Double DoubleMath(Double other, S.Func<double, double, double> func) => new(func(this.Value, other.Value));

        /// <summary>Determines if this value is positive or negative infinity.</summary>
        /// <returns>True if the number is either positive or negative infinity, false otherwise.</returns>
        public bool IsInfinity() => double.IsInfinity(this.Value);

        /// <summary>Determines if this value is not a number.</summary>
        /// <returns>True if the number is not a number, false otherwise.</returns>
        public bool IsNaN() => double.IsNaN(this.Value);

        /// <summary>This gets the positive infinity value.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        public Double InfinityValue => Infinity;

        /// <summary>Determines if this value is not a number.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        public Double NaNValue => NaN;

        #endregion
        #region Multiplicative...

        /// <summary>Gets the product of this value and the other value.</summary>
        /// <remarks>The current value is not used in the product.</remarks>
        /// <param name="other">The values to multiply this value with.</param>
        /// <returns>The product of this value and the other values.</returns>
        public Double Mul(IEnumerable<Double> other) => new(other.Aggregate(1.0, (t1, t2) => t1 * t2.Value));

        /// <summary>
        /// Indicates that for this data type, multiplication is commutable,
        /// meaning that the order of the parents makes no difference to the result.
        /// </summary>
        /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
        public bool MulCommutable => true;

        /// <summary>The identity of multiplication for this data type.</summary>
        /// <remarks>The current value is not used in the product.</remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
        public Double MulIdentityValue => MulIdentity;

        #endregion
        #region Signed...

        /// <summary>Gets the absolute value of this data value.</summary>
        /// <returns>The absolute value of this value.</returns>
        public Double Abs() => new(S.Math.Abs(this.Value));

        /// <summary>Gets the absolute value of this data value.</summary>
        /// <returns>The absolute value of this value.</returns>
        public Double Neg() => new(-this.Value);

        /// <summary>Determines if the this value is negative.</summary>
        /// <returns>True if below zero, false if zero or more.</returns>
        public bool IsNegative() => double.IsNegative(this.Value);

        #endregion
        #region Subtractive...

        /// <summary>Gets the difference between this value and the other value.</summary>
        /// <param name="other">The value to subtract from this value.</param>
        /// <returns>The difference between this value and the other value.</returns>
        public Double Sub(Double other) => new(this.Value - other.Value);

        #endregion
        #region Casts...

        /// <summary>Casts an integer into a double for an implicit cast.</summary>
        /// <param name="value">The integer value to cast.</param>
        /// <returns>The resulting double value.</returns>
        public Double CastFrom(Int value) => new(value.Value);

        #endregion

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is Double other && this.Equals(other);

        /// <summary>Gets the name of this data type and value.</summary>
        /// <returns>The name of the double type and value.</returns>
        public override string ToString() => this.TypeName+"("+this.ValueString+")";
    }
}
