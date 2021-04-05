namespace Blackboard.Core.Interfaces {

    /// <summary>The interface for a namespace.</summary>
    /// <remarks>The children for a namespace should always be sorted by name.</remarks>
    public interface INamespace: INode {

        /// <summary>This determines if the namespace has a child with the given name.</summary>
        /// <param name="name">The name of the child to check for.</param>
        /// <returns>True if the child exists, false otherwise.</returns>
        bool Exists(string name);

        /// <summary>Finds the child with the given name.</summary>
        /// <param name="name">The name of the child to check for.</param>
        /// <returns>The child with the given name, otherwise null is returned.</returns>
        INamed Find(string name);
    }
}
