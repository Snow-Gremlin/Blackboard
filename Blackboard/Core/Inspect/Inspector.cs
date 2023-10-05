using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Inspect;

/// <summary>A tool for inspecting, validating, and debugging the slate.</summary>
sealed internal class Inspector {

    /// <summary>Validates the given slate.</summary>
    /// <param name="slate">The slate to validate.</param>
    /// <param name="logger">The logger to write errors, warnings, and info out to.</param>
    /// <returns>True if the slate passed validation, otherwise false.</returns>
    static public bool Validate(Slate slate, Logger? logger) {
        logger = logger.Group("Validate");
        logger.Info("Validate:");
        Inspector v = new(slate, logger);
        v.CollectNodes();
        v.CheckParents();
        v.CheckChildren();
        return v.Passed;
    }

    private readonly Slate          slate;
    private readonly LogCounter     logger;
    private readonly HashSet<INode> touched;
    private readonly Stringifier    stringifier;

    /// <summary>Creates a new inspector.</summary>
    /// <param name="slate">The slate to validate.</param>
    /// <param name="logger">The logger to write errors, warnings, and info out to.</param>
    private Inspector(Slate slate, Logger? logger) {
        this.slate       = slate;
        this.logger      = new(logger);
        this.touched     = new();
        this.stringifier = Stringifier.Shallow();
        this.stringifier.PreLoadNames(this.slate);
    }

    /// <summary>Indicates if there were zero errors.</summary>
    public bool Passed => this.logger.Count(Level.Error) <= 0;

    /// <summary>Collects all the nodes in the slate reachable from global.</summary>
    public void CollectNodes() {
        HashSet<INode> pending = new();

        void addToPending(INode node) {
            if (this.touched.Contains(node) ||
                pending.Contains(node)) return;
            pending.Add(node);
        }

        addToPending(this.slate.Global);
        while (pending.Count > 0) {
            INode node = pending.First();
            pending.Remove(node);

            if (this.touched.Contains(node)) continue;
            this.touched.Add(node);

            if (node is IParent parent)
                parent.Children.Foreach(addToPending);

            if (node is IChild child)
                child.Parents.Foreach(addToPending);

            if (node is IFieldReader reader)
                reader.Fields.Select(pair => pair.Value).Foreach(addToPending);
        }
    }

    /// <summary>Checks that every parent's child has the parent as a parent.</summary>
    public void CheckParents() =>
        this.touched.OfType<IParent>().Foreach(this.checkParent);

    /// <summary>Checks that the children of the given parent has this parent as a parent.</summary>
    /// <param name="parent">The parent to check.</param>
    private void checkParent(IParent parent) {
        foreach (IChild child in parent.Children) {
            if (!child.Parents.Contains(parent))
                this.logger.Error(new Message("Child doesn't know it's parent").
                    With("Child",  this.stringifier.Stringify(child)).
                    With("Parent", this.stringifier.Stringify(parent)));
        }
    }

    /// <summary>Checks that every child's parent has the child as a child.</summary>
    public void CheckChildren() =>
        this.touched.OfType<IChild>().Foreach(this.checkChild);

    /// <summary>Checks that the parents of the given child has this child as a child.</summary>
    /// <param name="child">The child to check.</param>
    private void checkChild(IChild child) {
        foreach (IParent parent in child.Parents) {
            if (!parent.Children.Contains(child))
                this.logger.Error(new Message("Parent doesn't know it's child").
                    With("Child",  this.stringifier.Stringify(child)).
                    With("Parent", this.stringifier.Stringify(parent)));
        }
    }
}
