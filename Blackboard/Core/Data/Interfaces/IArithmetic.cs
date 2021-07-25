namespace Blackboard.Core.Data.Interfaces {
    public interface IArithmetic<T>: IAdditive<T>, IData
        where T : IData {
        T Abs();
        T Neg();
        T Inc();
        T Sub(T other);
        T Mul(T other);
        T Div(T other);
        T Mod(T other);
        T Rem(T other);
        T Pow(T other);
    }
}
