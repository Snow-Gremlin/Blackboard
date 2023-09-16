using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Extensions;
using Blackboard.Core.Record;
using System.Collections.Generic;

namespace Blackboard.Core.Inspect;

// TODO: Comment
sealed public class Debugger {

    static public bool Valid(Slate slate, Logger? logger = null) {
        Debugger v = new(slate, logger);
        v.CheckStructure();
        return v.Passed;
    }

    private readonly Slate          slate;
    private readonly Logger?        logger;
    private readonly HashSet<INode> touched;
    private readonly HashSet<INode> prending;

    private bool failed;

    private Debugger(Slate slate, Logger? logger) {
        this.slate    = slate;
        this.logger   = logger;
        this.touched  = new();
        this.prending = new();
        this.failed   = false;
    }

    public bool Passed => !this.failed;

    public void CheckStructure() => this.checkStructure(this.slate.Global);

    private void checkStructure(Namespace space) {
        foreach (KeyValuePair<string, INode> pair in space.Fields) {

        }
    }
}
