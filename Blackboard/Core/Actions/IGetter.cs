using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Actions {

    /// <summary>The interface for all the typed getter actions.</summary>
    public interface IGetter: IAction {

        /// <summary>The name to write the value to.</summary>
        public string Name { get; }

        /// <summary>The data node to get the data to get.</summary>
        public IDataNode Value { get; }

        /// <summary>All the nodes which are new children of the node to write.</summary>
        public IReadOnlyList<IEvaluable> NeedPending { get; }
    }
}
