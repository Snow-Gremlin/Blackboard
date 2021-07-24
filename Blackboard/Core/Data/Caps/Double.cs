using Blackboard.Core.Data.Interfaces;
using S = System;

namespace Blackboard.Core.Data.Caps {
    public struct Double: IArithmetic<Double>, IComparable<Double>, IFloatingPoint<Double>, IImplicit<Int, Double> {

        public readonly double Value;

        public Double(double value) {
            this.Value = value;
        }

        public Double Abs() => new(S.Math.Abs(this.Value));
        public Double Neg() => new(-this.Value);
        public Double Inc() => new(this.Value + 1.0);
        public Double Div(Double other) => new(this.Value / other.Value);
        public Double Mod(Double other) => new(this.Value % other.Value);
        public Double Mul(Double other) => new(this.Value * other.Value);
        public Double Sub(Double other) => new(this.Value - other.Value);
        public Double Sum(Double other) => new(this.Value + other.Value);

        public Double Lerp(Double min, Double max) =>
            (this.Value <= 0.0) ? min :
            (this.Value >= 1.0) ? max :
            new((max.Value-min.Value)*this.Value + min.Value);

        public Double CastFrom(Int value) => new(value.Value);

        public int CompareTo(Double other) => this.Value.CompareTo(other.Value);

        public override string ToString() => "double";
    }
}
