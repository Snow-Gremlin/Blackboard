using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an output which has a value.</summary>
    /// <typeparam name="T">The type of the value to output.</typeparam>
    public interface IValueOutput<T>: IValue<T>, IOutput
        where T : IData {

        /// <summary>This event is emitted when the value is changed.</summary>
        event System.EventHandler OnChanged;
    }
}
