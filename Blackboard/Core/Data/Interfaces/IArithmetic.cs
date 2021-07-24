namespace Blackboard.Core.Data.Interfaces {
    public interface IArithmetic<T>: IData
        where T : IData {
        T Abs();
        T Neg();
        T Inc();
        T Sum(T other);
        T Sub(T other);
        T Mul(T other);
        T Div(T other);
        T Mod(T other);
    }
}
