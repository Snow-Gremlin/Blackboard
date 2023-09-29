namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for an extern trigger.</summary>
/// <remarks>
/// An external node is a placeholder for a node that will be defined later.
/// An external trigger may not be initialized and will always be unprovoked until replaced.
/// </remarks>
internal interface ITriggerExtern : ITrigger, IExtern { }
