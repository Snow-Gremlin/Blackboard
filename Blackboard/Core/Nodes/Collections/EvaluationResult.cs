using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Collections;

// TODO: Comment
sealed public class EvaluationResult {

   public HashSet<ITrigger> Provoked { get; } = new();

   public HashSet<IOutput> Outputs { get; } = new();
}
