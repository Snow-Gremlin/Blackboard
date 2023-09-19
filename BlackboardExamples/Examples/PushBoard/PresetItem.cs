namespace BlackboardExamples.Examples.PushBoard;

/// <summary>An item used for defining a preset.</summary>
/// <param name="Name">The display name of the preset.</param>
/// <param name="Custom">The code to set for the preset.</param>
/// <param name="Code">The code to set for the preset.<</param>
public readonly record struct PresetItem(string Name, bool Custom, params string[] Code) {

    /// <summary>Gets the custom item instance.</summary>
    static public readonly PresetItem CustomInstance = new("Custom", true);
    
    /// <summary>Creates a new preset item.</summary>
    /// <param name="name">The display name of the preset.</param>
    /// <param name="code">The code to set for the preset.</param>
    public PresetItem(string name, params string[] code) : this(name, false, code) { }

    /// <summary>The display name of the preset.</summary>
    /// <returns>The name of the preset.</returns>
    public override string ToString() => this.Name;
}
