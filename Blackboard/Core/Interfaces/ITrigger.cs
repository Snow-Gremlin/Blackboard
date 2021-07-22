namespace Blackboard.Core.Interfaces {

    /// <summary>The interface for a trigger node.</summary>
    public interface ITrigger: INode {

        /// <summary>Indicates this trigger had been provoked during the current evaluation.</summary>
        bool Provoked { get; }

        /// <summary>Resets the trigger after evaluation has finished.</summary>
        void Reset();
    }
}
