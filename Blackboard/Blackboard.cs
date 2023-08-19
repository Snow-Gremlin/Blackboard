using Blackboard.Core;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Formula;
using Blackboard.Core.Formula.Factory;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Record;
using Blackboard.Core.Types;

namespace Blackboard;

/// <summary>
/// Blackboard is a tool for managing complex interactions and states.
/// This is the main object for the provocateur.
/// </summary>
sealed public class Blackboard {
    private readonly Slate slate;
    private readonly Logger? logger;

    /// <summary>Creates a new blackboard.</summary>
    public Blackboard() => this.slate = new Slate();

    /// <summary>Create a collection of commands that can be run and rerun.</summary>
    /// <param name="input">The input blackboard code to create the formula for.</param>
    /// <returns>The formula for the given blackboard code.</returns>
    public Formula CreateFormula(params string[] input) =>
        new Parser.Parser(this.slate).Read(input);

    /// <summary>Reads the given blackboard code and performs the formula.</summary>
    /// <param name="input">The input blackboard code to run.</param>
    /// <returns>The result of the performed blackboard code.</returns>
    public Result Perform(params string[] input) =>
        this.CreateFormula(input).Perform();

    /// <summary>Watches for a trigger to be provoked.</summary>
    /// <param name="name">The name of the trigger to watch.</param>
    /// <returns>The trigger watcher.</returns>
    public ITriggerWatcher OnProvoke(string name) {
        Factory factory = new(this.slate, this.logger);
        factory.RequestExtern(name, Type.Trigger);
        factory.Build().Perform();

        IOutput output = this.slate.GetOutput(Type.Trigger, name);
        return output as ITriggerWatcher ??
            throw new Message("Failed to create output trigger").
                With("name",     name).
                With("existing", output);
    }

    /// <summary>Watches for a change in the value by the given name.</summary>
    /// <typeparam name="T">The C# type of the value to watch.</typeparam>
    /// <param name="name">The name of the value to watch.</param>
    /// <returns>The value watcher.</returns>
    public IValueWatcher<T> OnChange<T>(string name)
        where T : struct, IData, IEquatable<T> {
        Type type = Type.FromValueType(typeof(T)) ??
            throw new Message("The given type is unsupported").
                With("name", name).
                With("Type", typeof(T));

        Factory factory = new(this.slate, this.logger);
        factory.RequestExtern(name, default(T).Type);
        factory.Build().Perform();

        IOutput output = this.slate.GetOutput(type, name);
        return output as IValueWatcher<T> ??
            throw new Message("Failed to create value output").
                With("type",     type).
                With("name",     name).
                With("existing", output);
    }
}
