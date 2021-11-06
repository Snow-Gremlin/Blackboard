using System.Collections.Generic;
using S = System;

namespace Blackboard.Core {

    /// <summary>A collection of predefined exceptions.</summary>
    static internal class Exceptions {
        // TODO: Check for unused exceptions.

        /// <summary>An exception for a function which was defined with an unknown type.</summary>
        /// <param name="typeIndex">The index in the argument types which is unknown.</param>
        /// <returns>The new exception.</returns>
        static public Exception UnknownFunctionParamType(int typeIndex) =>
            new Exception("The type used to define a function is not known by the Blackboard type system.").
                With("Type index", typeIndex);

        /// <summary>An exception for a node or group is being renames and it is invalid.</summary>
        /// <param name="name">The name which is invalid.</param>
        /// <returns>The new exception.</returns>
        static public Exception InvalidName(string name) =>
            new Exception("The given name is not a valid identifier.").
                With("name", name);

        /// <summary>An exception for when a loop is detected adding children to a node.</summary>
        /// <returns>The new exception.</returns>
        static public Exception NodeLoopDetected() =>
            new("May not add children: Loop detected.");

        /// <summary>An exception for when a loop is detected adding a group to a scope.</summary>
        /// <returns>The new exception.</returns>
        static public Exception ScopeLoopDetected() =>
            new("May not add to scope: Loop detected.");

        /// <summary>An exception for when a node type isn't being defined correctly for data and non-data types.</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="realType">The C# type for this type.</param>
        /// <param name="dataType">The IData type underlying this type.</param>
        /// <returns>The new exception.</returns>
        static public Exception TypeDefinitionInvalid(string name, S.Type realType, S.Type dataType) =>
            new Exception("A node type with a non-null data type must have a real type which implements IDataNode and vice versa.").
                With("Name", name).
                With("Real Type", realType).
                With("Data Type", dataType);

        /// <summary>An exception for when a value can not be found in the driver.</summary>
        /// <param name="names">The names of the value to find.</param>
        /// <returns>The new exception.</returns>
        static public Exception NoValueFoundByNames(IEnumerable<string> names) =>
            new Exception("No value found by the given name.").
                With("Names", string.Join(".", names));

        /// <summary>An exception for when a value can not be cast.</summary>
        /// <param name="names">The names of the found value.</param>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="requestedType">The requested type to cast the value into.</param>
        /// <returns>The new exception.</returns>
        static public Exception UnableToCastValueAsRequested(IEnumerable<string> names, S.Type valueType, S.Type requestedType) =>
            new Exception("May not cast the value with the given name into requested type.").
                With("Names", string.Join(".", names)).
                With("Value Type", valueType).
                With("Requested Type", requestedType);
    }
}
