using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Gets the a formatted string from the parent values.</summary>
    sealed public class Format : ValueNode<String>, IChild {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public IFuncDef Factory =>
            new Function1N<IValueParent<String>, IValueParent<IData>, Format>((fmt, args) => new Format(fmt, args));

        /// <summary>This is the parent to increment the counter.</summary>
        private IValueParent<String> format;

        /// <summary>This is the list of all the parent nodes to read from.</summary>
        private readonly List<IValueParent<IData>> args;

        /// <summary>Creates a string formatter node.</summary>
        public Format() {
            this.format = null;
            this.args = new List<IValueParent<IData>>();
        }

        /// <summary>Creates a string formatter node.</summary>
        /// <param name="fmt">The parent which provides the format string.</param>
        /// <param name="args">The parents which provide all the arguments to fill the format with.</param>
        public Format(IValueParent<String> fmt, params IValueParent<IData>[] args) : base() {
            this.format = fmt;
            this.args = args.ToList();
        }

        /// <summary>Creates a string formatter node.</summary>
        /// <param name="fmt">The parent which provides the format string.</param>
        /// <param name="args">The parents which provide all the arguments to fill the format with.</param>
        public Format(IValueParent<String> fmt, IEnumerable<IValueParent<IData>> args) : base() {
            this.format = fmt;
            this.args = args.ToList();
        }

        /// <summary>The parent node to get the format string from.</summary>
        public IValueParent<String> FormatParent {
            get => this.format;
            set => IChild.SetParent(this, ref this.format, value);
        }

        /// <summary>This adds parents to this node.</summary>
        /// <remarks>The value is updated after these parents are added.</remarks>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(IEnumerable<IValueParent<IData>> parents) =>
            this.args.AddParents(parents);

        /// <summary>This removes the given parents from this node.</summary>
        /// <remarks>The value is updated after these parents are removed.</remarks>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(IEnumerable<IValueParent<IData>> parents) =>
            this.args.RemoveParents(this, parents);

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Format();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Format);

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public ParentCollection Parents => new ParentCollection(this, 1).
            With(() => this.format, parent => this.format = parent).
            With(this.args);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override String CalcuateValue() =>
            this.format is null ? default :
            new(string.Format(this.format.Value.Value,
                this.args.Select(n => n?.Value?.ValueObject ?? null).ToArray()));
    }
}
