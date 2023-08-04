namespace Blackboard.Core.Optimization;

/// <summary>The interface for an optimization rule.</summary>
internal interface IRule {

    /// <summary>Performs optimization on the given nodes and surrounding nodes.</summary>
    /// <param name="args">The arguments for the optimization rules.</param>
    public void Perform(RuleArgs args);
}
