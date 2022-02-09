using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections {
    public class FixedParents: IParentCollection {
        private interface ISingleParam {
            public S.Type Types { get; }
            public IParent Node { get; set; }
        }

        private class SingleParam<T>: ISingleParam
                where T : class, IParent {

            private readonly S.Func<T> getParent;

            private readonly S.Action<T> setParent;

            public SingleParam(S.Func<T> getParent, S.Action<T> setParent) {
                this.getParent = getParent;
                this.setParent = setParent;
            }

            public S.Type Types => typeof(T);

            public IParent Node {
                get => this.getParent();
                set => this.setParent(value as T);
            }

        }

        private List<ISingleParam> parameters;

        public FixedParents(IChild child) {
            this.Child = child;
            this.parameters = new List<ISingleParam>();
        }

        public readonly IChild Child;

        public IEnumerable<S.Type> Types => this.parameters.Select((p) => p.Types);

        public IEnumerable<IParent> Nodes => this.parameters.Select((p) => p.Node).NotNull();

        internal FixedParents With<T>(S.Func<T> getParent, S.Action<T> setParent)
            where T : class, IParent {
            this.parameters.Add(new SingleParam<T>(getParent, setParent));
            return this;
        }

        public bool ReplaceParent(IParent oldParent, IParent newParent) => throw new S.NotImplementedException();

        public bool SetAllParents(List<IParent> newParents) => throw new S.NotImplementedException();

        /* 
        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool ReplaceParent(IParent oldParent, IParent newParent) =>
            IChild.ReplaceParent(this, ref this.source1, oldParent, newParent) |
            IChild.ReplaceParent(this, ref this.source2, oldParent, newParent);

        /// <summary>This will attempt to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAllParents(List<IParent> newParents) {
            IChild.CheckParentsBeingSet(newParents, false, typeof(IValueParent<T1>), typeof(IValueParent<T2>));
            return IChild.SetParent(this, ref this.source1, newParents[0] as IValueParent<T1>) |
                   IChild.SetParent(this, ref this.source2, newParents[1] as IValueParent<T2>);
        }
        */

        /*
        /// <summary>This is a helper method for replacing a parent in the given node.</summary>
        /// <typeparam name="T">The node type for the parent.</typeparam>
        /// <param name="child">The child to replace the parent for.</param>
        /// <param name="node">The parent node variable being replaced.</param>
        /// <param name="oldParent">The old parent to check for.</param>
        /// <param name="newParent">The new parent being set, or null</param>
        /// <returns>True if replaced, false if not.</returns>
        static protected bool ReplaceParent<T>(IChild child, ref T node, IParent oldParent, IParent newParent)
            where T : class, IParent {
            if (!ReferenceEquals(node, oldParent)) return false;
            if (newParent is not null and not T)
                throw new Exception("Unable to replace old parent with new parent.").
                    With("child", child).
                    With("node", node).
                    With("old Parent", oldParent).
                    With("new Parent", newParent);
            bool removed = node?.RemoveChildren(child) ?? false;
            node = newParent as T;
            if (removed) node?.AddChildren(child);
            return true;
        }

        /// <summary>This checks if the set of parents about to be set to this node is valid.</summary>
        /// <param name="parents">The list of parents to check before being set.</param>
        /// <param name="variableLength">
        /// True if any number of parents may be set, false to check the count.
        /// If true then there must be at least one type in the expected types.
        /// </param>
        /// <param name="expectedTypes">The expected types for the parents.</param>
        static internal void CheckParentsBeingSet(List<IParent> parents, bool variableLength, params S.Type[] expectedTypes) {
            if (!variableLength) {
                int count = expectedTypes.Length;
                if (parents.Count != count)
                    throw new Exception("Incorrect number of parents in the list of parents to set to a node.").
                        With("Expected Count", count).
                        With("Given Count", parents.Count);
            }

            int index = 0;
            foreach ((IParent parent, S.Type expectedType) in parents.Zip(expectedTypes)) {
                if (parent.GetType().IsAssignableTo(expectedType))
                    throw new Exception("Incorrect type of a parent in the list of parents to set to a node.").
                        With("Index", index).
                        With("Expected Type", expectedType).
                        With("Gotten Type", parent.GetType());
                ++index;
            }
        }
        */
    }
}
