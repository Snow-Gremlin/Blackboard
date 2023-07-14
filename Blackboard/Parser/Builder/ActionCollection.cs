﻿using Blackboard.Core.Actions;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Builder;

sealed internal partial class Builder {

    /// <summary>The collection of actions which have been parsed.</summary>
    public class ActionCollection {
        private readonly Builder builder;
        private readonly LinkedList<IAction> actions;

        /// <summary>Creates new a new action collection.</summary>
        /// <param name="builder">The builder this collection belongs to.</param>
        internal ActionCollection(Builder builder) {
            this.builder = builder;
            this.actions = new LinkedList<IAction>();
        }

        /// <summary>Clears the collection of actions.</summary>
        public void Clear() => this.actions.Clear();

        /// <summary>Gets the formula containing all the actions.</summary>
        public Formula Formula =>
            new(this.builder.Slate, this.actions.Append(new Finish()));

        /// <summary>Adds a pending action into this formula.</summary>
        /// <param name="performer">The performer to add.</param>
        public void Add(IAction action) {
            this.builder.Logger.Info("Add Action: {0}", action);
            this.actions.AddLast(action);
        }

        /// <summary>Gets the human readable string of the current actions.</summary>
        /// <returns>The human readable string.</returns>
        public override string ToString() => this.ToString("");

        /// <summary>Gets the human readable string of the current actions.</summary>
        /// <param name="indent">The indent to apply to all but the first line being returned.</param>
        /// <returns>The human readable string.</returns>
        public string ToString(string indent) =>
            this.actions.Count <= 0 ? "[]" :
            "[\n" + indent + this.actions.Strings().Indent(indent).Join(",\n" + indent) + "]";
    }
}
