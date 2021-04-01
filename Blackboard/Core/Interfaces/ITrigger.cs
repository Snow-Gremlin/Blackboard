namespace Blackboard.Core.Interfaces {

    public interface ITrigger: INode {

        bool Triggered { get; }

        void Reset();
    }
}
