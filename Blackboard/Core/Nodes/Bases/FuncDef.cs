using Blackboard.Core.Debug;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>
    /// This is the base function node information for all non-group function types.
    /// This represents a single function definition with a specific signature.
    /// </summary>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>
    public abstract class FuncDef<TReturn>: IFuncDef
        where TReturn : INode {

        /// <summary>The minimum required nodes for the new node's arguments.</summary>
        /// <remarks>Must be less than or equal to the maximum arguments and zero or more.</remarks>
        public readonly int MinArgs;

        /// <summary>The maximum allowed nodes for the new node's arguments.</summary>
        /// <remarks>Must be greater than or equal to the minimum arguments.</remarks>
        public readonly int MaxArgs;

        /// <summary>Indicates that at least one argument must not be a cast.</summary>
        /// <remarks>
        /// This protects against situations where all arguments can be cast into two equal matches but we
        /// only want to take one when the programmer uses some type to indicate which one to use.
        /// For example if everything can be cast to a double and to a string during a sum, we don't
        /// want to take the string concatenation unless one of the arguments is a string. The programmer
        /// then indicates if the value is a sum or a concatenation by including at least one string.
        /// </remarks>
        public readonly bool NeedsOneNoCast;

        /// <summary>Indicates if there is only one argument for a new node, return the argument.</summary>
        /// <remarks>
        /// This is for things like sum which is valid with only one so we don't want to set minimum
        /// to 2. However sum of a single value is that value (same with multiply, and, or, etc), so for
        /// functions like that, instead of wrapping it in the function, simply return the argument.
        /// This is only used if the minimum allowed arguments is less than two.
        /// This will not be allowed if the first argument type is not or
        /// can not be implicitly cast to the returned value.
        /// </remarks>
        public readonly bool PassthroughOne;

        /// <summary>All the argument types.</summary>
        private readonly Type[] argTypes;

        /// <summary>Creates a new non-group function base with the specific number of arguments.</summary>
        /// <remarks>By default no group is defined for this function.</remarks>
        /// <param name="needsOneNoCast">
        /// This indicates that at least one argument must not be implicitly cast for this function.
        /// This helps ensure that some functions are only called if there is not cast so that the developer can select the
        /// function they want to use which have similar implicit cast weights by adding a non-cast value or pre-casting a value.
        /// </param>
        /// <param name="passOne">
        /// This indicates that if there is only one input, just return it.
        /// If needed it will still perform an implicit cast to match the first argument type.
        /// This is only valid for this constructor if and only if there is one argument.
        /// </param>
        /// <param name="argTypes">This is the required types for this function.</param>
        protected FuncDef(bool needsOneNoCast, bool passOne, params Type[] argTypes) :
            this(argTypes.Length, argTypes.Length, needsOneNoCast, passOne, argTypes) { }

        /// <summary>Creates a new non-group function base.</summary>
        /// <remarks>By default no group is defined for this function.</remarks>
        /// <param name="min">The minimum allowed number of arguments. Will be clamped to zero.</param>
        /// <param name="max">The maximum allowed number of arguments. Will be clamped to the minimum value.</param>
        /// <param name="needsOneNoCast">
        /// This indicates that at least one argument must not be implicitly cast for this function.
        /// This helps ensure that some functions are only called if there is not cast so that the developer can select the
        /// function they want to use which have similar implicit cast weights by adding a non-cast value or pre-casting a value.
        /// </param>
        /// <param name="passOne">
        /// This indicates that if there is only one input, just return it.
        /// If needed it will still perform an implicit cast to match the first argument type.
        /// </param>
        /// <param name="argTypes">The types for all the arguments. Any nil types are ignored.</param>
        protected FuncDef(int min, int max, bool needsOneNoCast, bool passOne, params Type[] argTypes) {
            argTypes = argTypes.NotNull().ToArray();

            void throwExp(string message) =>
                throw new Exception(message).
                    With("Minimum", min).
                    With("Maximum", max).
                    With("Need one no cast", needsOneNoCast).
                    With("Pass one", passOne).
                    With("Types", argTypes.Strings().Join(", ")).
                    With("Return type", typeof(TReturn));

            if (min < 0)
                throwExp("Must define a minimum required number of arguments that is zero or more.");

            if (min > max)
                throwExp("Must define a maximum allowed number of arguments that is greater than and equal to the minimum required number.");

            if (max < argTypes.Length)
                throwExp("The maximum allowed number of arguments is below the number of given types. Trim the type list.");

            if (max > 0 && argTypes.Length <= 0)
                throwExp("Must have at least one argument type if maximum is greater than zero.");

            if (needsOneNoCast && min < 1)
                throwExp("May not require one no cast and have a minimum required of arguments less than one.");

            if (passOne && max < 1)
                throwExp("May not allow pass one if the maximum allowed value less than one.");

            bool passthroughOne = passOne && Type.Match<TReturn>(argTypes[0]).IsMatch;
            if (passOne && !passthroughOne)
                throwExp("May not pass one when the first argument type is not inherited nor implicit cast to the return type.");

            this.MinArgs = min;
            this.MaxArgs = max;
            this.NeedsOneNoCast = needsOneNoCast;
            this.argTypes = argTypes;
            this.PassthroughOne = passthroughOne;
        }

        /// <summary>This is the type name of the node.</summary>
        public string TypeName => "Function";

        /// <summary>The collection of argument types.</summary>
        /// <remarks>
        /// If the maximum number of arguments is more than the number of
        /// argument types then the last argument is a repeatable variable length argument.
        /// </remarks>
        public IReadOnlyList<Type> ArgumentTypes => this.argTypes;

        /// <summary>
        /// Indicates that the last argument type maybe used zero or more times based on
        /// the maximum and minimum number of arguments.
        /// If false the last argument may only be used zero or one time.
        /// </summary>
        public bool LastArgVariable => this.argTypes.Length < this.MaxArgs;

        /// <summary>Returns the type that would be return if built.</summary>
        /// <returns>The type which would be returned.</returns>
        public S.Type ReturnType => typeof(TReturn);

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The matching results for this function.</returns>
        public virtual FuncMatch Match(Type[] types) =>
            types.Length < this.MinArgs ? FuncMatch.NoMatch :
            types.Length > this.MaxArgs ? FuncMatch.NoMatch :
            FuncMatch.Create(this.NeedsOneNoCast, types.Zip(this.argTypes.RepeatLast(), Type.Match));

        /// <summary>This will implicitly cast the given parameter,</summary>
        /// <param name="node">The node to cast.</param>
        /// <param name="t">The type to get the cast to.</param>
        /// <returns>The resulting parameter in the expected type.</returns>
        static private INode castParam(INode node, Type t) {
            INode cast = t.Implicit(node);
            return cast is not null ? cast :
                throw new Exception("Error implicitly casting parameter").
                    With("Node", node).
                    With("Implicit", t).
                    With("Result", cast);
        }

        /// <summary>Builds and returns the function node.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public virtual INode Build(INode[] nodes) =>
            this.PassthroughOne && nodes.Length == 1 ? castParam(nodes[0], Type.FromType<TReturn>()) :
            this.PostCastBuild(nodes.Zip(this.argTypes.RepeatLast(), castParam).ToArray());

        /// <summary>Builds and return the function node with the given arguments already casted.</summary>
        /// <param name="nodes">These are the nodes casted into the correct type for the build.</param>
        /// <returns>The resulting function node.</returns>
        protected abstract INode PostCastBuild(INode[] nodes);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
