using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Collections;

partial class ParentCollection {

    /// <summary>This is a single fixed parent container.</summary>
    /// <typeparam name="T">The type of the parent that is being set.</typeparam>
    private class FixedParent<T> : IFixedParent
            where T : class, IParent {
        private IParent? cachedParent;
        private readonly S.Func<T?> getParent;
        private readonly S.Action<T?> setParent;

        /// <summary>Creates a new fixed parent container using the given getter or setter.</summary>
        /// <param name="getParent">The getter to get the parent value from the child.</param>
        /// <param name="setParent">The setter to set a parent value to the child.</param>
        public FixedParent(S.Func<T?> getParent, S.Action<T?> setParent) {
            this.cachedParent = null;
            this.getParent = getParent;
            this.setParent = setParent;
        }

        /// <summary>Gets the type of the parent.</summary>
        public S.Type Type => typeof(T);

        /// <summary>Gets or sets the parent.</summary>
        /// <remarks>
        /// This parent must be able to case to the container type.
        /// It is known that if the parent is null, then the cache can not provide improvement.
        /// </remarks>
        public IParent? Node {
            get => this.cachedParent ??= this.getParent();
            set {
                this.cachedParent = value;
                this.setParent(value as T);
            }
        }
    }
}
