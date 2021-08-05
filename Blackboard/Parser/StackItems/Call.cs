using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;
using System.Collections.Generic;

namespace Blackboard.Parser.StackItems {

    /// <summary>A stack item for a method call.</summary>
    sealed public class Call: StackItem {

        /// <summary>This is the function group to call.</summary>
        public readonly FuncGroup Func;

        /// <summary>The name of the method to call.</summary>
        public readonly string Name;

        /// <summary>The arguments for this method call.</summary>
        public List<INode> Arguments;

        /// <summary>The stack item for a method call.</summary>
        /// <param name="loc">The location of the identifier.</param>
        /// <param name="func">The function group to call.</param>
        /// <param name="name">The name of the method to call.</param>
        public Call(Location loc, FuncGroup func, string name): base(loc) {
            this.Func = func;
            this.Name = name;
            this.Arguments = new List<INode>();
        }

        /// <summary>The string for this stack item.</summary>
        /// <returns>The stack item's string.</returns>
        public override string ToString() =>
            "Call of " + this.Name + " at " + this.Location;
    }
}
