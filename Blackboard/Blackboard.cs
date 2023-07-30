using Blackboard.Core;
using System;

namespace Blackboard;

sealed public class Blackboard {

    private readonly Slate slate;

    public Blackboard() => this.slate = new Slate();

    //private Formula read(params string[] input) =>
    //    new Parser.Parser(this.slate).Read(input);

    public EventHandler EmitOn(string name) =>
        (sender, args) => this.EmitOn<EventArgs>(name)(sender, args);

    public EventHandler<T> EmitOn<T>(string name) where T: EventArgs {
        throw new NotImplementedException();
    }
}
