using S = System;

namespace BlackboardTests.Tools;

/// <summary>This is an attribute for tagging test methods with a string.</summary>
[S.AttributeUsage(S.AttributeTargets.Method)]
public class TestTagAttribute : S.Attribute {

    /// <summary>This is the value applied to the test method.</summary>
    public readonly string Value;

    /// <summary>Creates a new test method tag.</summary>
    /// <param name="value">The value of the tag to apply.</param>
    public TestTagAttribute(string value) => this.Value = value;
}
