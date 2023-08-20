using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Collections;

/// <summary>This is a collection of values returned from evaluating nodes.</summary>
sealed public class EvaluationResult {

    /// <summary>This is the set of triggers which were provoked during evaluation.</summary>
    /// <remarks>These triggers are ones which need to be reset.</remarks>
    public HashSet<ITrigger> Provoked { get; } = new();

    /// <summary>This is the set of outputs which have been changed or provoked.</summary>
    /// <remarks>These are outputs which need to be emitted.</remarks>
    public HashSet<IOutput> Outputs { get; } = new();
}
