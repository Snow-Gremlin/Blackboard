using Blackboard.Core.Data.Caps;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type is nullable.</summary>
    public interface INullable {

        /// <summary>Determines if the this value is null.</summary>
        /// <returns>True if null, false otherwise.</returns>
        Bool IsNull();
    }
}
