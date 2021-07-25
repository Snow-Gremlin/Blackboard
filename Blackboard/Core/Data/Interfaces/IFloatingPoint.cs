using Blackboard.Core.Data.Caps;

namespace Blackboard.Core.Data.Interfaces {
    public interface IFloatingPoint<T>: IData
        where T : IData {

        T Lerp(T min, T max);
        T Round(Int decimals);
        T Ceiling();
        T Floor();
        T Truncate();
    }
}
