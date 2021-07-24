using Blackboard.Core.Data.Interfaces;
using S = System;

namespace Blackboard.Core.Data.Caps {
    public struct Int: IArithmetic<Int>, IComparable<Int>, IBitwise<Int> {

        public readonly int Value;

        public Int(int value = 0) {
            this.Value = value;
        }

        public Int Abs() => new(S.Math.Abs(this.Value));
        public Int Neg() => new(-this.Value);
        public Int Inc() => new(this.Value + 1);
        public Int Div(Int other) => new(this.Value / other.Value);
        public Int Mod(Int other) => new(this.Value % other.Value);
        public Int Mul(Int other) => new(this.Value * other.Value);
        public Int Sub(Int other) => new(this.Value - other.Value);
        public Int Sum(Int other) => new(this.Value + other.Value);

        public Int BitwiseNot() => new(~this.Value);
        public Int BitwiseAnd(Int other) => new(this.Value & other.Value);
        public Int BitwiseOr(Int other) => new(this.Value | other.Value);
        public Int BitwiseXor(Int other) => new(this.Value ^ other.Value);
        public Int LeftShift(Int other) => new(this.Value << other.Value);
        public Int RightShift(Int other) => new(this.Value >> other.Value);

        public int CompareTo(Int other) => this.Value.CompareTo(other.Value);

        public override string ToString() => "int";
    }
}
