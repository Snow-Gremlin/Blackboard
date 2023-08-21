using Blackboard.Core;
using Blackboard.Core.Formula;
using Blackboard.Core.Formula.Factory;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Record;
using Blackboard.Core.Types;
using System.Text.RegularExpressions;
using S = System;

namespace Blackboard;

/// <summary>
/// Blackboard is a tool for managing complex interactions and states.
/// This is the main object for the provocateur.
/// </summary>
sealed public partial class Blackboard {

    /// <summary>A regular expression for matching an identifier with an optional namespace.</summary>
    /// <returns>The regular expression for matching a name.</returns>
    [GeneratedRegex(@"[a-zA-Z_$][0-9a-zA-Z_]*(\.[a-zA-Z_$][0-9a-zA-Z_]*)*", RegexOptions.Compiled)]
    private static partial Regex nameRegex();

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

    /// <summary>Groups several calls together.</summary>
    /// <remarks>
    /// Any formulas run or values set during this group will not reset triggers or
    /// emit any output until the group has finished.
    /// </remarks>
    /// <param name="handle">This action performs all the values to group together.</param>
    public void Group(S.Action handle) {
        if (this.slate.Finalization.Suspend) {
            handle();
            return;
        }

        this.slate.Finalization.Suspend = true;
        handle();
        this.slate.Finalization.Suspend = false;
        this.slate.FinishEvaluation();
    }

    /// <summary>
    /// Checks that the given name is a valid identifier with optional namespace
    /// and splits the name into smaller identifiers.
    /// </summary>
    /// <param name="name">The name to split.</param>
    /// <returns>The namespace identifiers followed by the main identifier.</returns>
    private string[] splitUpName(string name) =>
        nameRegex().IsMatch(name) ? name.Split('.') :
            throw new Message("Invalid identifier with optional namespace.").
                With("name", name);

    /// <summary>Create an extension with the given identifier and optional namespace.</summary>
    /// <param name="type">The type of the external to create.</param>
    /// <param name="name">The identifier and optional namespace of the extern.</param>
    private void createExtern(Type type, params string[] name) {
        int max = name.Length-1;
        Factory factory = new(this.slate, this.logger);
        for (int i = 0; i < max; ++i)
            factory.PushNamespace(name[i]);
        factory.RequestExtern(name[max], type);
        factory.Build().Perform();
    }

    /// <summary>Watches for a trigger to be provoked.</summary>
    /// <param name="name">The name of the trigger to watch.</param>
    /// <returns>The trigger watcher.</returns>
    public ITriggerWatcher OnProvoke(string name) {
        string[] parts = this.splitUpName(name);
        this.createExtern(Type.Trigger, parts);

        IOutput output = this.slate.GetOutput(Type.Trigger, parts);
        return output as ITriggerWatcher ??
            throw new Message("Failed to create output trigger").
                With("name",     name).
                With("existing", output);
    }

    /// <summary>Watches for a change in the value by the given name.</summary>
    /// <typeparam name="T">The C# type of the value to watch.</typeparam>
    /// <param name="name">The name of the value to watch.</param>
    /// <returns>The value watcher.</returns>
    public IValueWatcher<T> OnChange<T>(string name) {  
        Type type = Type.FromValueType(typeof(T)) ??
            throw new Message("The given type is unsupported").
                With("name", name).
                With("Type", typeof(T));
        
        string[] parts = this.splitUpName(name);
        this.createExtern(type, parts);

        IOutput output = this.slate.GetOutput(type, parts);
        return output as IValueWatcher<T> ??
            throw new Message("Failed to create value output").
                With("type",     type).
                With("name",     name).
                With("existing", output);
    }

    // TODO: Add a way to get input values.
    // TODO: Add a way to set input values.
}
