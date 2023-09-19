namespace BlackboardExamples.Controls;

/// <summary>The base requirements for a blackboard control component.</summary>
internal interface IBlackBoardComponent {

    /// <summary>
    /// Connects this control component to the given blackboard.
    /// Any previously connected blackboard should to be disconnected first.
    /// </summary>
    /// <remarks>
    /// This will write an input value or trigger, and or watch a value or trigger
    /// in the blackboard with a given identifier and optional namespace.
    /// </remarks>
    /// <param name="id">The given identifier and optional namespace.</param>
    /// <param name="b">The blackboard to connect to.</param>
    public void Connect(Blackboard.Blackboard b, string id);

    /// <summary>Disconnects this control component from a blackboard.</summary>
    /// <remarks>This has no effect if not connected.</remarks>
    public void Disconnect();
}
