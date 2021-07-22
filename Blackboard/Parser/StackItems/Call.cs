using PP = PetiteParser;

namespace Blackboard.Parser.StackItems {

    /// <summary>A stack item for a method call.</summary>
    public class Call {

        /// <summary>The name of the method to call.</summary>
        public readonly Identifier Identifier;

        /// <summary>The location of the method name.</summary>
        public readonly PP.Scanner.Location Location;

        /// <summary>The stack item for a method call.</summary>
        /// <param name="id">The name of the method to call.</param>
        /// <param name="loc">The location of the last part of the.</param>
        public Call(Identifier id, PP.Scanner.Location loc) {
            this.Identifier = id;
            this.Location = loc;
        }
    }
}
