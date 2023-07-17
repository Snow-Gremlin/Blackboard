using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Outer;

namespace Blackboard.Core.Innate;

/// <summary>Group of all initial constants for Blackboard.</summary>
static public class Constants {

    /// <summary>This adds all the initial constants for Blackboard.</summary>
    /// <param name="global">The global namespace for the slate.</param>
    static internal void Add(Namespace global) {
        global["e"    ] = Literal.Double(System.Math.E);
        global["pi"   ] = Literal.Double(System.Math.PI);
        global["tau"  ] = Literal.Double(System.Math.Tau);
        global["sqrt2"] = Literal.Double(System.Math.Sqrt(2.0));
        global["nan"  ] = Literal.Data(Double.NaN);
        global["inf"  ] = Literal.Data(Double.Infinity);
        global["null" ] = Literal.Data(Object.Null);
    }
}
