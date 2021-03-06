using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Actions {

    /// <summary>The interface for all the typed assignment actions.</summary>
    /// <remarks>
    /// This is only implemented by Assign but has no type parameter
    /// so that all typed Assigns can easily be used generically.
    /// </remarks>
    public interface IAssign: IAction {

        /// <summary>The target input node to set the value of.</summary>
        public IInput Target { get; }

        /// <summary>The data node to get the data to assign.</summary>
        public IDataNode Value { get; }

        /// <summary>All the nodes which are new children of the node to write.</summary>
        public IReadOnlyList<IEvaluable> NeedPending { get; }
    }
}
