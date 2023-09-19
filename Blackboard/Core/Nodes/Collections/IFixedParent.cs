using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Collections;

partial class ParentCollection {

    /// <summary>The definition for a single fixed parent container.</summary>
    private interface IFixedParent {

        /// <summary>Gets the type of the container.</summary>
        public S.Type Type { get; }

        /// <summary>Gets or set the parent.</summary>
        public IParent? Node { get; set; }
    }
}
