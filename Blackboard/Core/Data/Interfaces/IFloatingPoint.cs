using Blackboard.Core.Data.Caps;
using S = System;

namespace Blackboard.Core.Data.Interfaces {
    public interface IFloatingPoint<T>: IData
        where T : IData {

        T Lerp(T min, T max);
        T Round(Int decimals);
        T Atan2(T x);
        T Log(T newBase);
        T DoubleMath(S.Func<double, double> func);
    }
}
