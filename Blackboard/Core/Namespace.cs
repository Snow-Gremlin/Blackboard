using System.Collections.Generic;

namespace Blackboard.Core {

    /// <summary>A dictionary for containing named objects.</summary>
    public class Namespace: SortedDictionary<string, object> {

        /// <summary>Creates a new namespace.</summary>
        public Namespace() { }

        /// <summary>Determines if the given item by name exists.</summary>
        /// <param name="name">The name of the item to look for.</param>
        /// <returns>True if the name exists in this node structure.</returns>
        public bool Contains(string name) => this.Find(name) is not null;

        /// <summary>Finds the given item by name.</summary>
        /// <param name="names">The names for the item to look for.</param>
        /// <returns>The item or null if not found.</returns>
        public object Find(params string[] names) => this.Find(names as IEnumerable<string>);

        /// <summary>Finds the given item at the given names.</summary>
        /// <param name="names">The names for the item to look for.</param>
        /// <returns>The item or null if not found.</returns>
        public object Find(IEnumerable<string> names) {
            object cur = this;
            foreach (string name in names) {
                if (cur is Namespace scope) {
                    if (!scope.ContainsKey(name)) return null;
                    cur = scope[name];
                } else return null;
            }
            return cur;
        }
    }
}
