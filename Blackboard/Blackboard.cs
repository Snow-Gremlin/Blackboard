using Blackboard.Core;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Formuila;
using Blackboard.Core.Formuila.Factory;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;

namespace Blackboard;

sealed public class Blackboard {

    private readonly Slate slate;
    private readonly Logger? logger;

    public Blackboard() => this.slate = new Slate();

    public Formula CreateFormula(params string[] input) =>
        new Parser.Parser(this.slate).Read(input);

    public Surface.ITrigger OnProvoke(string name) {
        Factory factory = new(this.slate, this.logger);
        factory.RequestExtern(name, Type.Trigger);
        factory.Build().Perform();

        IOutput output = this.slate.GetOutput(Type.Trigger, name);
        return output as Surface.ITrigger ??
            throw new Message("Failed to create output trigger").
                With("name", name).
                With("existing", output);
    }
    
    public Surface.IValue<T> OnChange<T>(string name)
        where T : struct, IData, IEquatable<T> {
        Type type = Type.FromValueType(typeof(T)) ??
            throw new Message("The given type is unsupported").
                With("Type", typeof(T));

        Factory factory = new(this.slate, this.logger);
        factory.RequestExtern(name, default(T).Type);
        factory.Build().Perform();
        
        IOutput output = this.slate.GetOutput(type, name);
        return output as Surface.IValue<T> ??
            throw new Message("Failed to create value output").
                With("type", type).
                With("name", name).
                With("existing", output);
    }
}
