using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an input which has a value.</summary>
    /// <typeparam name="T">The type of the value to input.</typeparam>
    public interface IValueInput<T>: IValueParent<T>
        where T : IData {

        /// <summary>Sets the value of this input.</summary>
        /// <remarks>This is not intended to be called directly, it should be called via the driver.</remarks>
        /// <param name="value">The value to input.</param>
        /// <returns>
        /// The set of nodes (should be any evaluable children of this node)
        /// that should be updated based on the results of this node's update.
        /// If this evaluation made no change then no node should be returned.
        /// </returns>
        public IEnumerable<Evaluable> SetValue(T value);
    }
}
