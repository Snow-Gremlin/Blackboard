using Blackboard.Core.Inspect;

namespace Blackboard.Core.Formula.Actions;

/// <summary>This is an action for performing any remaining evaluation and resets any provoked triggers.</summary>
sealed internal class Finish : IAction {

    /// <summary>Creates a new finish action.</summary>
    public Finish() { }

    /// <summary>This will perform the action.</summary>
    /// <param name="slate">The slate for this action.</param>
    /// <param name="result">The result being created and added to.</param>
    /// <param name="logger">The optional logger to debug with.</param>
    public void Perform(Slate slate, Record.Result result, Logger? logger = null) {
        logger.Info("Finish: {0}", this);
        slate.PerformEvaluation(logger);
        slate.FinishEvaluation();
    }

    /// <summary>Gets a human readable string for this reset.</summary>
    /// <returns>The human readable string for debugging.</returns>
    public override string ToString() => Stringifier.Simple(this);
}
