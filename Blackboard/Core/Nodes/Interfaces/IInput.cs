namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for a node used for input.</summary>
    /// <remarks>
    /// All inputs may be used as an output so that a single value can
    /// be written to and read from without having to attach an output to it.
    /// </remarks>
    public interface IInput: IOutput { }
}
