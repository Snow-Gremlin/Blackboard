namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for a node used for input.</summary>
    /// <remarks>
    /// All inputs may be used as an output so that a single value can
    /// be written to and read from without having to attach an output to it.
    /// Some inputs will have parents, they are simply assignable types, like a latch,
    /// which means they can be modified by input not just the parents.
    /// </remarks>
    public interface IInput: IOutput { }
}
