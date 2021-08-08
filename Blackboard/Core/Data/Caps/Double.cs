using Blackboard.Core.Data.Interfaces;
using S = System;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for an IEEE 754 double value such that it can be used in generics.</summary>
    public struct Double: IArithmetic<Double>, IComparable<Double>, IFloatingPoint<Double>,
        IImplicit<Int, Double> {

        /// <summary>The double value being stored.</summary>
        public readonly double Value;

        /// <summary>Creates a new double data value.</summary>
        /// <param name="value">The double value to store.</param>
        public Double(double value = 0.0) {
            this.Value = value;
        }

        /// <summary>Gets the absolute value of this data value.</summary>
        /// <returns>The absolute value of this value.</returns>
        public Double Abs() => new(S.Math.Abs(this.Value));

        /// <summary>Gets the absolute value of this data value.</summary>
        /// <returns>The absolute value of this value.</returns>
        public Double Neg() => new(-this.Value);

        /// <summary>Gets this value incremented by one.</summary>
        /// <returns>This data value plus one.</returns>
        public Double Inc() => new(this.Value + 1.0);

        /// <summary>Gets the division of this value and the other value.</summary>
        /// <param name="other">The value to divide this value with.</param>
        /// <returns>This value divided by the other value.</returns>
        public Double Div(Double other) => new(this.Value / other.Value);

        /// <summary>Gets the modulo of this value and the other value.</summary>
        /// <remarks>The result will have the same sign as this value.</remarks>
        /// <param name="other">The value to mod this value with.</param>
        /// <returns>The modulo of this value and the other value.</returns>
        public Double Mod(Double other) //=> new(this.Value % other.Value);
            {
            double val = this.Value % other.Value;
            S.Console.WriteLine(this.Value+" % "+other.Value+" => "+val);
            return new(val);
        }

        /// <summary>Gets the remainder of this value divided by the other value.</summary>
        /// <remarks>This is the IEEE 754 specification of remainder.</remarks>
        /// <param name="other">The value to divide this value with.</param>
        /// <returns>The remainder of this value divided the other value.</returns>
        public Double Rem(Double other) => new(S.Math.IEEERemainder(this.Value, other.Value));

        /// <summary>Gets the product of this value and the other value.</summary>
        /// <param name="other">The value to multiply this value with.</param>
        /// <returns>The product of this value and the other value.</returns>
        public Double Mul(Double other) => new(this.Value * other.Value);

        /// <summary>Gets the difference between this value and the other value.</summary>
        /// <param name="other">The value to subtract from this value.</param>
        /// <returns>The difference between this value and the other value.</returns>
        public Double Sub(Double other) => new(this.Value - other.Value);

        /// <summary>This will add this data to the other data.</summary>
        /// <param name="other">The other data to add to this value.</param>
        /// <returns>The sum of the two data values.</returns>
        public Double Sum(Double other) => new(this.Value + other.Value);

        /// <summary>Gets the power of this value to the other value.</summary>
        /// <param name="other">The value to use as the exponent.</param>
        /// <returns>The power of this value to the other value.</returns>
        public Double Pow(Double other) => new(S.Math.Pow(this.Value, other.Value));

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

        /// <summary>This gets the Atan2 where this value is the Y input.</summary>
        /// <param name="x">This is the X input value.</param>
        /// <returns>The Atan2 of this and the other value.</returns>
        public Double Atan2(Double x) => new(S.Math.Atan2(this.Value, x.Value));

        /// <summary>This gets the logarithm of this value using the other value as the base.</summary>
        /// <param name="newBase">The value to use as the base of the log.</param>
        /// <returns>The result of the logarithm.</returns>
        public Double Log(Double newBase) => new(S.Math.Log(this.Value, newBase.Value));

        /// <summary>This performs the given function on this value.</summary>
        /// <param name="func">The function to run on this value.</param>
        /// <returns>The resulting value from this value being used in the given function.</returns>
        public Double DoubleMath(S.Func<double, double> func) => new(func(this.Value));

        /// <summary>Casts an integer into a double for an implicit cast.</summary>
        /// <param name="value">The integer value to cast.</param>
        /// <returns>The resulting double value.</returns>
        public Double CastFrom(Int value) => new(value.Value);

        /// <summary>Compares two doubles together.</summary>
        /// <param name="other">The other double to compare.</param>
        /// <returns>The comparison result indicating which is greater than or equal.</returns>
        public int CompareTo(Double other) => this.Value.CompareTo(other.Value);
        public static bool operator ==(Double left, Double right) => left.CompareTo(right) == 0;
        public static bool operator !=(Double left, Double right) => left.CompareTo(right) != 0;
        public static bool operator <(Double left, Double right) => left.CompareTo(right) < 0;
        public static bool operator <=(Double left, Double right) => left.CompareTo(right) <= 0;
        public static bool operator >(Double left, Double right) => left.CompareTo(right) > 0;
        public static bool operator >=(Double left, Double right) => left.CompareTo(right) >= 0;

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is Double other && this.Value == other.Value;

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>Gets the name of this data type.</summary>
        /// <returns>The name of the bool type.</returns>
        public override string ToString() => "double";
    }
}
