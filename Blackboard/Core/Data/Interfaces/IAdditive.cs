namespace Blackboard.Core.Data.Interfaces {
    public interface IAdditive<T>: IData
        where T : IData {
        T Sum(T other);
    }
}
