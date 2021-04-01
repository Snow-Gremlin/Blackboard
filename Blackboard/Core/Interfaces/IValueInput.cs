namespace Blackboard.Core.Interfaces {

    public interface IValueInput<T>: IInput {

        T Value { get; }

        bool SetValue(T value);
    }
}
