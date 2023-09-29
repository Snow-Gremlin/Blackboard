using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for an output which has a value.</summary>
/// <typeparam name="T">The type of the value to output.</typeparam>
internal interface IValueOutput<T> : IValue<T>, IOutput, IChild
    where T : IData { }
