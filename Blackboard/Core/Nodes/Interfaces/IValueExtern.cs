using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for an external value.</summary>
/// <remarks>
/// An external node is a placeholder for a node that will be defined later.
/// The external node can carry a value to act as the default value until it is defined for real.
/// Extern are input nodes so that they can be initialized.
/// </remarks>
/// <typeparam name="T">The type of the value to external.</typeparam>
internal interface IValueExtern<T> : IValueInput<T>, IExtern
    where T : IData { }
