﻿using Blackboard.Core.Record;

namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for an output trigger.</summary>
internal interface ITriggerOutput : ITriggerWatcher, IOutput, ITrigger, IChild {}
