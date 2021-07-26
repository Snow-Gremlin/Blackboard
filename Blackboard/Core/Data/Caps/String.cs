using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Data.Caps {
    public struct String: IAdditive<String>, IComparable<String>,
        IImplicit<Bool, String>, IImplicit<Int, String>, IImplicit<Double, String> {

        public readonly string Value;

        public String(string value = "") {
            this.Value = value;
        }

        public String Sum(String other) => new(this.Value + other.Value);
        public int CompareTo(String other) => this.Value.CompareTo(other.Value);
        public override bool Equals(object obj) => obj is String other && this.Value == other.Value;
        public override int GetHashCode() => this.Value.GetHashCode();
        public override string ToString() => "string";
        public String CastFrom(Bool value) => new(value.Value.ToString());
        public String CastFrom(Int value) => new(value.Value.ToString());
        public String CastFrom(Double value) => new(value.Value.ToString());
        public static bool operator == (String left, String right) => left.Equals(right);
        public static bool operator != (String left, String right) => !(left == right);
        public static bool operator < (String left, String right) => left.CompareTo(right) < 0;
        public static bool operator <= (String left, String right) => left.CompareTo(right) <= 0;
        public static bool operator > (String left, String right) => left.CompareTo(right) > 0;
        public static bool operator >= (String left, String right) => left.CompareTo(right) >= 0;
    }
}
