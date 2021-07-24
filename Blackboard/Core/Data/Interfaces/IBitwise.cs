namespace Blackboard.Core.Data.Interfaces {
    public interface IBitwise<T>: IData
        where T : IData {
        T BitwiseNot();
        T BitwiseAnd(T other);
        T BitwiseOr(T other);
        T BitwiseXor(T other);
        T LeftShift(T other);
        T RightShift(T other);
    }
}
