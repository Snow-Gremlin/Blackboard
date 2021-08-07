using Blackboard.Core;
using PetiteParser.Scanner;

namespace Blackboard.Parser {

    /// <summary>The stack item for the parser.</summary>
    sealed internal class StackItem {

        /// <summary>The location for this stack item.</summary>
        public readonly Location Location;

        /// <summary>The identifier for this stack item.</summary>
        public readonly string Id;

        /// <summary>
        /// The value from the stack item. Or if this has an identifier
        /// then this is the value found at this identifier or null if nothing found.
        /// </summary>
        public readonly object Value;

        /// <summary>Indicates if this stack item has an identifier.</summary>
        public bool HasId => !string.IsNullOrEmpty(this.Id);

        /// <summary>This determines if the value can be cast to the given type.</summary>
        /// <typeparam name="T">The type to try to cast the value to.</typeparam>
        /// <returns>True if the value can be cast, false otherwise.</returns>
        public bool ValueIs<T>() => this.Value is T;

        /// <summary>This gets the value cast into the given type.</summary>
        /// <remarks>This will throw an exception if it can't cast the value.</remarks>
        /// <typeparam name="T">The type to cast the value to.</typeparam>
        /// <returns>The value as the given type.</returns>
        public T ValueAs<T>() => this.Value is T result ? result :
            throw Exception.UnexpectedItemOnTheStack(this.ToString(), typeof(T).FullName);

        /// <summary>Creates a new stack item.</summary>
        /// <param name="loc">The location of the stack item.</param>
        /// <param name="value">The value from the stack,  or null.</param>
        public StackItem(Location loc, object value):
            this(loc, null, value) { }

        /// <summary>Creates a new stack item.</summary>
        /// <param name="loc">The location of the stack item.</param>
        /// <param name="id">The optional identifier for the value or empty.</param>
        /// <param name="value">The value from the stack, the value found at this identifier, or null.</param>
        public StackItem(Location loc, string id, object value) {
            this.Location = loc;
            this.Id = id;
            this.Value = value;
        }

        /// <summary>The string for this stack item.</summary>
        /// <returns>The stack item's string.</returns>
        public override string ToString() {
            string id = this.HasId ? "": this.Id + ", ";
            string value = this.Value?.ToString() ?? "null";
            return "StackItem(" + id + value + ", " + this.Location + ")";
        }
    }
}
