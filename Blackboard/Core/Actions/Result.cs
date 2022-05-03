using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Inspect;
using Blackboard.Core.Types;
using System.Collections.Generic;

namespace Blackboard.Core.Actions {

    /// <summary>The results from a formula being performed.</summary>
    sealed public class Result {
        private readonly Dictionary<string, object> outputData;

        /// <summary>Creates a new results object.</summary>
        internal Result() =>
            this.outputData = new Dictionary<string, object>();

        #region Output Data...
        
        /// <summary>Gets the names for the set output.</summary>
        public IEnumerable<string> OutputNames => this.outputData.Keys;

        /// <summary>Gets the type of the value at the given output.</summary>
        /// <param name="name">The name given to the output data.</param>
        /// <returns>The type of the output or null if doesn't exist or not a node type.</returns>
        public Type OutputType(string name) {
            object obj = this.outputData[name];
            return obj is null ? null : Type.FromType(obj.GetType());
        }

        /// <summary>Gets the value from an named output.</summary>
        /// <param name="name">The name given to the output data.</param>
        /// <returns>The value from the output.</returns>
        public bool GetBool(string name) =>
            this.GetValue<Bool>(name).Value;

        /// <summary>Gets the value from an named output.</summary>
        /// <param name="name">The name given to the output data.</param>
        /// <returns>The value from the output.</returns>
        public int GetInt(string name) =>
            this.GetValue<Int>(name).Value;

        /// <summary>Gets the value from an named output.</summary>
        /// <param name="name">The name given to the output data.</param>
        /// <returns>The value from the output.</returns>
        public double GetDouble(string name) =>
            this.GetValue<Double>(name).Value;

        /// <summary>Gets the value from an named output.</summary>
        /// <param name="name">The name given to the output data.</param>
        /// <returns>The GetValue from the output.</returns>
        public string GetString(string name) =>
            this.GetValue<String>(name).Value;

        /// <summary>Gets the value from an named output.</summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="name">The name given to the output data.</param>
        /// <returns>The value from the output.</returns>
        public T GetValue<T>(string name) where T : IData {
            object obj = this.outputData[name];
            return obj is null ?
                    throw new Message("Unable to get an output value by the given name.").
                        With("Name", name).
                        With("Value Type", typeof(T)) :
                obj is not T data ?
                    throw new Message("The output value found by the given name is not the expected type.").
                        With("Name", name).
                        With("Found Type", obj.GetType()).
                        With("Expected Type", typeof(T)) :
                data;
        }

        /// <summary>Sets the value of an named output.</summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="name">The name given to the output data.</param>
        /// <returns>The value from the output.</returns>
        public void SetValue<T>(string name, T value) where T : IData =>
            this.outputData[name] = value;

        #endregion
    }
}
