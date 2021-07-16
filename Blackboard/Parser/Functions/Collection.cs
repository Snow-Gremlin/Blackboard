using Blackboard.Core;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Functions {

    /// <summary>The collection of functions for the parser.</summary>
    public class Collection: Dictionary<string, List<IFunction>> {

        /// <summary>Creates a new collection of functions.</summary>
        public Collection() { }

        /// <summary>Finds the function which best matches the given input nodes.</summary>
        /// <param name="name">The name of the functions to look through.</param>
        /// <param name="nodes">The nodes which will be used for arguments.</param>
        /// <returns>The nearest function or null if none found.</returns>
        public IFunction Find(string name, params INode[] nodes) =>
            this.Find(name, nodes);

        /// <summary>Finds the function which best matches the given input nodes.</summary>
        /// <param name="name">The name of the functions to look through.</param>
        /// <param name="nodes">The nodes which will be used for arguments.</param>
        /// <returns>The nearest function or null if none found.</returns>
        public IFunction Find(string name, IEnumerable<INode> nodes) {
            int minVal = -1;
            IFunction minFunc = null;
            INode[] inputs = nodes.ToArray();
            foreach (IFunction func in this[name]) {
                int value = func.Match(inputs);
                if (value < 0) continue;
                if (minVal >= 0 && minVal < value) continue;
                minVal = value;
                minFunc = func;
            }
            return minFunc;
        }

        /// <summary>Builds the function with the given name and arguments.</summary>
        /// <param name="name">The name of the functions to look through.</param>
        /// <param name="nodes">The nodes which will be used for arguments.</param>
        /// <returns>The built function or null if nofunction was found.</returns>
        public INode Build(string name, params INode[] nodes) =>
            this.Build(name, nodes);

        /// <summary>Builds the function with the given name and arguments.</summary>
        /// <param name="name">The name of the functions to look through.</param>
        /// <param name="nodes">The nodes which will be used for arguments.</param>
        /// <returns>The built function or null if nofunction was found.</returns>
        public INode Build(string name, IEnumerable<INode> nodes) {
            IFunction func = this.Find(name, nodes);
            INode[] inputs = nodes.ToArray();
            if (func is null) {
                // TODO: Improve this message since it is can be hit by users of Blackboard when they input bad code.
                string input = string.Join(", ", nodes.Select((node) => IFunction.PrettyName(node)));
                throw new Exception("No known procedure to handle "+name+"(" + input + ").");
            }
            return func.Build(inputs);
        }

        /// <summary>Adds new function to this collection.</summary>
        /// <param name="name">The name of the functions to add.</param>
        /// <param name="funcs">The function factories to add.</param>
        /// <returns>The collection so that adds can be chained together.</returns>
        public Collection Add(string name, params IFunction[] funcs) =>
            this.Add(name, funcs);

        /// <summary>Adds new function to this collection.</summary>
        /// <param name="name">The name of the functions to add.</param>
        /// <param name="funcs">The function factories to add.</param>
        /// <returns>The collection so that adds can be chained together.</returns>
        public Collection Add(string name, IEnumerable<IFunction> funcs) {
            if (!this.TryGetValue(name, out List<IFunction> funcs2)) {
                funcs2 = new List<IFunction>();
                this[name] = funcs2;
            }
            funcs2.AddRange(funcs);
            return this;
        }
    }
}
