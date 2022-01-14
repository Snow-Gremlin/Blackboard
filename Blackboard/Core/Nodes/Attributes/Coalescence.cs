using System;

namespace Blackboard.Core.Nodes.Attributes {

    /// <summary>
    /// This attribute is for an optimization rule.
    /// The optimization rule will allow an n-ary node with a parent of the same type
    /// to remove the parent and put all of that parent's parents in order in place of the parent.
    /// </summary>
    /// <example>
    /// Sum(a, b, Sum(c, d, e), f) => Sum(a, b, c, d, e, f);
    /// Sum(Sum(a, b), Sum(c, d)) => Sum(a, b, c, d);
    /// </example>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    sealed public class Coalescence: Attribute { }
}
