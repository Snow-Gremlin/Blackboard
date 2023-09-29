using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for an input which has a value as data.</summary>
internal interface IDataInput: IInput {

    /// <summary>Sets the value of this input via data.</summary>
    /// <remarks>
    /// This is not intended to be called directly, it should be called via the slate or action.
    /// This will throw an exception if the data type is not valid for the given input.
    /// </remarks>
    /// <param name="data">The data value to assign to the input.</param>
    /// <returns>True if there was any change, false otherwise.</returns>
    public bool SetData(IData data);
}
