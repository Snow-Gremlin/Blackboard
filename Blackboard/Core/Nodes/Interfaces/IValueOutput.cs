using Blackboard.Core.Data.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an output which has a value.</summary>
    /// <typeparam name="T">The type of the value to output.</typeparam>
    public interface IValueOutput<T, TEvent>: IValue<T>, IOutput
        where T : IData
        where TEvent : S.EventArgs {

        /// <summary>This event is emitted when the value is changed.</summary>
        event S.EventHandler<TEvent> OnChanged;
    }
}
