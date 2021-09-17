using PP = PetiteParser;

namespace Blackboard.Parser.Actors.Interfaces {

    /// <summary>The interface for all actors.</summary>
    internal interface IActor {

        /// <summary>The location this actor was defind in the code being parsed.</summary>
        public PP.Scanner.Location Location { get; }

        /// <summary>Prepare will check, optimize, and simplify the actor as much as possible.</summary>
        /// <returns>
        /// This is the actor to replace this one with,
        /// if this actor is returned then it should not be replaced.
        /// if null then this actor should be removed.
        /// </returns>
        public IActor Prepare();
    }
}
