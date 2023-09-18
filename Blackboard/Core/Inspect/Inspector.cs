using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Inspect;

// TODO: Comment
sealed public class Inspector {

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

    private Inspector(Slate slate, Logger? logger) {
        this.slate       = slate;
        this.logger      = new(logger);
        this.touched     = new();
        this.stringifier = Stringifier.Shallow();
        this.stringifier.PreLoadNames(this.slate);
    }

    public bool Passed => this.logger.Count(Level.Error) > 0;

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

    public void CheckParents() =>
        this.touched.OfType<IParent>().Foreach(this.checkParent);

    private void checkParent(IParent parent) {
        foreach (IChild child in parent.Children) {
            if (!child.Parents.Contains(parent))
                this.logger.Error(new Message("Child doesn't know it's parent").
                    With("Child",  this.stringifier.Stringify(child)).
                    With("Parent", this.stringifier.Stringify(parent)));
        }
    }

    public void CheckChildren() =>
        this.touched.OfType<IChild>().Foreach(this.checkChild);

    private void checkChild(IChild child) {
        foreach (IParent parent in child.Parents) {
            if (!parent.Children.Contains(child))
                this.logger.Error(new Message("Parent doesn't know it's child").
                    With("Child",  this.stringifier.Stringify(child)).
                    With("Parent", this.stringifier.Stringify(parent)));
        }
    }
}
