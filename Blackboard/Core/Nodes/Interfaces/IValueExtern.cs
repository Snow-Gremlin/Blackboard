using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for an external value.</summary>
/// <remarks>
/// An external node is a placeholder for a node that will be defined later.
/// The external node can carry a value to act as the default value until it is defined for real.
/// </remarks>
/// <typeparam name="T">The type of the value to external.</typeparam>
public interface IValueExtern<T> : IValueParent<T>, IExtern
    where T : IData {

    /// <summary>Sets the default value of this external.</summary>
    /// <remarks>
    /// This is not intended to be called directly, it should be called via the slate or action.
    /// This is also intended to only be called once directly after creation and never again.
    /// </remarks>
    /// <param name="value">The value to assign to the extern.</param>
    /// <returns>True if there was any change, false otherwise.</returns>
    public bool SetValue(T value);
}
