using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is a trigger node which has several parents.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Arity#n-ary"/>
    public abstract class NaryTrigger: TriggerNode, INaryChild<ITriggerParent> {

        /// <summary>This is a helper for creating unary node factories quickly.</summary>
        /// <param name="handle">The handler for calling the node constructor.</param>
        /// <param name="needsOneNoCast">Indicates that at least one argument must not be a cast.</param>
        /// <param name="passOne">
        /// Indicates if there is only one argument for a new node, return the argument.
        /// By default a Nary function will pass one unless otherwise indicated.
        /// </param>
        /// <param name="min">The minimum number of required nodes.</param>
        /// <param name="max">The maximum allowed number of nodes.</param>
        static public IFuncDef CreateFactory<Tout>(S.Func<IEnumerable<ITriggerParent>, Tout> handle,
            bool needsOneNoCast = false, bool passOne = true, int min = 1, int max = int.MaxValue)
            where Tout : NaryTrigger =>
            new FunctionN<ITriggerParent, Tout>(handle, needsOneNoCast, passOne, min, max);

        /// <summary>This is the list of all the parent nodes to listen to.</summary>
        private List<ITriggerParent> sources;

        /// <summary>Creates a multi-trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public NaryTrigger(params ITriggerParent[] parents) :
            this(parents as IEnumerable<ITriggerParent>) { }

        /// <summary>Creates a multi-trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public NaryTrigger(IEnumerable<ITriggerParent> parents = null) {
            this.sources = new List<ITriggerParent>();
            this.AddParents(parents);
        }

        /// <summary>This adds parents to this node.</summary>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(IEnumerable<ITriggerParent> parents) =>
            this.sources.AddParents(parents);

        /// <summary>This removes the given parents from this node.</summary>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(IEnumerable<ITriggerParent> parents) =>
            this.sources.RemoveParents(this, parents);

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => this.sources;

        /// <summary>
        /// This handles updating this node's value given the
        /// parents' provoked state during evaluation.
        /// </summary>
        /// <remarks>Any null parents are ignored.</remarks>
        /// <param name="provoked">The value from the all the non-null parents.</param>
        /// <returns>The new value for this node.</returns>
        protected abstract bool OnEval(IEnumerable<bool> provoked);

        /// <summary>
        /// This is called when the trigger is evaluated and updated.
        /// It will determine if the trigger should be provoked.
        /// </summary>
        /// <returns>True if this trigger should be provoked, false if not.</returns>
        protected override bool ShouldProvoke() => this.OnEval(this.sources.NotNull().Triggers());
    }
}
