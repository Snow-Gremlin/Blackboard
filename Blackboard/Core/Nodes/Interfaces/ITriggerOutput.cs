using S = System;

namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for an output trigger.</summary>
public interface ITriggerOutput : Surface.ITrigger, IOutput, ITrigger, IChild {}
