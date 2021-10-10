using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using System.Linq;

namespace Blackboard.Parser {

    /// <summary>This will perform the actions that need to be run.</summary>
    /// <remarks>
    /// This should not throw an exception if prepared correctly.
    /// If the performer does throw an exception the prepers should be fixed to prevent this.
    /// </remarks>
    /// <returns>This is the newly created node or null.</returns>
    internal delegate INode Performer();

    /// <summary>This is a collection of performers to build the nodes for the parser.</summary>
    static internal class Performers {

        /// <summary>Creates a new evaluator.</summary>
        /// <param name="value">The performer to get the node to evaluate.</param>
        /// <returns>The performer to evaluate a node.</returns>
        static public Performer Evaluator(Performer value) =>
            () => (value() as IDataNode)?.ToConstant();

        /// <summary>Creates a new function performer.</summary>
        /// <param name="func">The function to performer.</param>
        /// <param name="args">The input arguments for the function to perform.</param>
        /// <returns>The performer to build a function.</returns>
        static public Performer Function(IFuncDef func, params Performer[] args) =>
            () => {
                INode[] nodes = args.Select((arg) => arg()).NotNull().ToArray();
                return func.Build(nodes);
            };

        /// <summary>Creates a new node holder.</summary>
        /// <remarks>This must NOT hold a node which has been wrapped or a node which may contain a wrapped node.</remarks>
        /// <param name="node">The node being held.</param>
        /// <returns>The performer to hold a node.</returns>
        static public Performer NodeHold(INode node) =>
            () => node;

        /// <summary>Creates a new virtual node writer.</summary>
        /// <param name="vitrualNode">The virtual node to write to.</param>
        /// <param name="value">The performer to get the node to write.</param>
        /// <returns>The performer to write to a virtual node.</returns>
        static public Performer VirtualNodeWriter(VirtualNode vitrualNode, Performer value) =>
            () => {
                INode node = value();
                vitrualNode.Node = node;
                return node;
            };

        /// <summary>Creates a new virtual node reference.</summary>
        /// <param name="wrappedNode">The reference to the real or virtual node.</param>
        static public Performer WrappedNodeReader(IWrappedNode wrappedNode) =>
            () => wrappedNode.Node;
    }
}
