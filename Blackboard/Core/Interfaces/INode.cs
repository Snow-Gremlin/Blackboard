using System.Collections.Generic;

namespace Blackboard.Core.Interfaces {

    public interface INode {

        int Depth { get; }

        IEnumerable<INode> Parents { get; }

        IEnumerable<INode> Children { get; }

        void AddChildren(params INode[] children);

        void AddChildren(IEnumerable<INode> children);

        void RemoveChildren(params INode[] children);

        void RemoveChildren(IEnumerable<INode> children);

        IEnumerable<INode> Eval();
    }
}
