using Blackboard.Parser.Actors.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Actors.Caps {

    /// <summary>
    /// An actor which contains a value.
    /// This value can not be evaluated nor built,
    /// this is only for storing a value on the stack for the parser.
    /// </summary>
    /// <typeparam name="T">The type of the value to stash.</typeparam>
    sealed internal class Stash<T>: IActor {

        /// <summary>Creates a new value actor.</summary>
        /// <param name="value">The value to store.</param>
        /// <param name="loc">The location this value was defined in code.</param>
        public Stash(T value, Location loc) {
            this.Location = loc;
            this.Value = value;
        }

        /// <summary>The value being stashed onto the stack.</summary>
        public T Value;

        /// <summary>The location this actor was defind in the code being parsed.</summary>
        public Location Location { get; private set; }

        /// <summary>Prepare will check, optimize, and simplify the actor as much as possible.</summary>
        /// <returns>This always returns this actor since there is nothing to prepare in this actor.</returns>
        public IActor Prepare() => this;

        /// <summary>Gets a human readable string for this action.</summary>
        /// <returns>The action's string.</returns>
        public override string ToString() =>
            "Stash<"+typeof(T).Name+">("+this.Value+", "+this.Location+")";
    }
}
