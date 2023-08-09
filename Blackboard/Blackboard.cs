using Blackboard.Core;
using Blackboard.Core.Formuila.Factory;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;

namespace Blackboard;

sealed public class Blackboard {

    private readonly Slate slate;
    private readonly Logger? logger;

    public Blackboard() => this.slate = new Slate();

    //private Formula read(params string[] input) =>
    //    new Parser.Parser(this.slate).Read(input);

    public ITriggerOutput EmitOn(string name) {
        Factory factory = new(this.slate, this.logger);
        factory.RequestExtern(name, Type.Trigger);
        factory.Build().Perform();
        return this.slate.GetOutputTrigger(name);
    }
}
