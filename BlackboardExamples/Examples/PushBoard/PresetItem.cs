namespace BlackboardExamples.Examples.PushBoard;

/// <summary>An item used for defining a preset.</summary>
sealed public class PresetItem {

    /// <summary>The display name of the preset.</summary>
    public readonly string Name;

    /// <summary>The description to display for this preset.</summary>
    public readonly string Description;

    /// <summary>The code to set for the preset.</summary>
    public readonly string[] Code;

    /// <summary>Indicates if this preset is the custom item.</summary>
    public readonly bool Custom;

    /// <summary>Gets the custom item instance.</summary>
    static public readonly PresetItem CustomInstance = new("Custom", true, "");

    /// <summary>Creates a new preset item.</summary>
    /// <param name="name">The display name of the preset.</param>
    /// <param name="description">The description to display for this preset.</param>
    /// <param name="code">The code to set for the preset.</param>
    /// <param name="custom">Indicates if this preset is the custom item.</param>
    private PresetItem(string name, bool custom, string description, params string[] code) {
        this.Name        = name;
        this.Description = description;
        this.Code        = code;
        this.Custom      = custom;
    }
    
    /// <summary>Creates a new preset item.</summary>
    /// <param name="name">The display name of the preset.</param>
    /// <param name="description">The description to display for this preset.</param>
    /// <param name="code">The code to set for the preset.</param>
    public PresetItem(string name, string description, params string[] code) : this(name, false, description, code) { }

    /// <summary>The display name of the preset.</summary>
    /// <returns>The name of the preset.</returns>
    public override string ToString() => this.Name;
}
