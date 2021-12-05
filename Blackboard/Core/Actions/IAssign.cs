using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Actions {

    /// <summary>The interface for all the typed assignment action.</summary>
    public interface IAssign: IAction {

        /// <summary>The target input node to set the value of.</summary>
        public IInput Target { get; }

        /// <summary>The data node to get the data to assign.</summary>
        public IDataNode Value { get; }
    }
}
