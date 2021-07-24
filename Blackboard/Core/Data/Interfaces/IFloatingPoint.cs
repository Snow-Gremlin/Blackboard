namespace Blackboard.Core.Data.Interfaces {
    public interface IFloatingPoint<T>: IData
        where T : IData {

        T Lerp(T min, T max);
    }
}
