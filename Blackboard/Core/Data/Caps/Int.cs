using Blackboard.Core.Data.Interfaces;
using S = System;

namespace Blackboard.Core.Data.Caps {
    public struct Int: IArithmetic<Int>, IComparable<Int>, IBitwise<Int>,
        IExplicit<Double, Int> {

        static Int() {
            Default = new(0);
        }

        static public readonly Int Default;

        public readonly int Value;

        public Int(int value = 0) {
            this.Value = value;
        }

        public Int Abs() => new(S.Math.Abs(this.Value));
        public Int Neg() => new(-this.Value);
        public Int Inc() => new(this.Value + 1);
        public Int Div(Int other) => new(this.Value / other.Value);
        public Int Mod(Int other) => new(this.Value % other.Value);
        public Int Rem(Int other) => new(this.Value - this.Value % other.Value);
        public Int Mul(Int other) => new(this.Value * other.Value);
        public Int Sub(Int other) => new(this.Value - other.Value);
        public Int Sum(Int other) => new(this.Value + other.Value);
        public Int Pow(Int other) => new((int)S.Math.Pow(this.Value, other.Value));
        public Int BitwiseNot() => new(~this.Value);
        public Int BitwiseAnd(Int other) => new(this.Value & other.Value);
        public Int BitwiseOr(Int other) => new(this.Value | other.Value);
        public Int BitwiseXor(Int other) => new(this.Value ^ other.Value);
        public Int LeftShift(Int other) => new(this.Value << other.Value);
        public Int RightShift(Int other) => new(this.Value >> other.Value);
        public Int CastFrom(Double value) => new((int)value.Value);
        public int CompareTo(Int other) => this.Value.CompareTo(other.Value);
        public override bool Equals(object obj) => obj is Int other && this.Value == other.Value;
        public override int GetHashCode() => this.Value.GetHashCode();
        public override string ToString() => "int";
        public static bool operator == (Int left, Int right) => left.Equals(right);
        public static bool operator != (Int left, Int right) => !(left == right);
        public static bool operator < (Int left, Int right) => left.CompareTo(right) < 0;
        public static bool operator <= (Int left, Int right) => left.CompareTo(right) <= 0;
        public static bool operator > (Int left, Int right) => left.CompareTo(right) > 0;
        public static bool operator >= (Int left, Int right) => left.CompareTo(right) >= 0;
    }
}
