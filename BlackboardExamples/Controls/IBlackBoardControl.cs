namespace BlackboardExamples.Controls;

/// <summary>The base requirements for a blackboard control.</summary>
internal interface IBlackBoardControl {

    /// <summary>
    /// Connects this control to the given blackboard.
    /// Any previously connected blackboard should to be disconnected first.
    /// </summary>
    /// <remarks>
    /// This will write all the input values and triggers, and watch a value and trigger
    /// to blackboard with a given identifier and optional namespace.
    /// </remarks>
    /// <param name="b">The blackboard to connect to.</param>
    public void Connect(Blackboard.Blackboard b);

    /// <summary>Disconnects this control from a blackboard.</summary>
    /// <remarks>This has no effect if not connected.</remarks>
    public void Disconnect();
}
