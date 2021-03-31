namespace Blackboard.Core.Interfaces {

    public interface IValue<out T>: INode {

        T Value { get; }
    }
}
