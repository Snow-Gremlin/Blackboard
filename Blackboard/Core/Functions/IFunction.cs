using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Functions {

    /// <summary>The interface for parsable function factory.</summary>
    public interface IFunction {

        /// <summary>The is a joiner for matching values.</summary>
        /// <param name="matches">The matching values.</param>
        /// <returns>The sum of the match or -1 if not a match.</returns>
        static internal int Join(params int[] matches) =>
            Join(matches as IEnumerable<int>);

        /// <summary>The is a joiner for matching values.</summary>
        /// <param name="matches">The matching values.</param>
        /// <returns>The sum of the match or -1 if not a match.</returns>
        static internal int Join(IEnumerable<int> matches) {
            int sum = 0;
            foreach (int match in matches) {
                if (match < 0) return -1;
                sum += match;
            }
            return sum;
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        int Match(INode[] nodes);

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been positive.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        INode Build(INode[] nodes);
    }
}
