using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using PP = PetiteParser;
using S = System;

namespace Blackboard.Parser {

    /// <summary>This will parse the Blackboard language into actions and nodes to apply to the slate.</summary>
    sealed public class Parser {

        /// <summary>The resource file for the Blackboard language definition.</summary>
        private const string resourceName = "Blackboard.Parser.Parser.lang";

        /// <summary>Prepares the parser's static variables before they are used.</summary>
        static Parser() => baseParser = PP.Loader.Loader.LoadParser(
                PP.Scanner.Default.FromResource(Assembly.GetExecutingAssembly(), resourceName));

        /// <summary>The Blackboard language base parser lazy singleton.</summary>
        static private readonly PP.Parser.Parser baseParser;

        /// <summary>The slate that this Blackboard is to create the actions for.</summary>
        private readonly Slate slate;

        /// <summary>The list of prompt handlers for running the Blackboard grammar with.</summary>
        private Dictionary<string, PP.ParseTree.PromptHandle> prompts;

        /// <summary>Optional logger to debugging and inspecting the parser.</summary>
        private ILogger logger;

        /// <summary>Creates a new Blackboard language parser.</summary>
        /// <param name="slate">The slate to modify.</param>
        /// <param name="logger">An optional logger for debugging and inspecting the parser.</param>
        public Parser(Slate slate, ILogger logger = null) {
            this.slate   = slate;
            this.prompts = null;
            this.logger  = logger;

            this.initPrompts();
            this.validatePrompts();
        }

        #region Read Methods...

        /// <summary>Reads the given lines of input Blackboard code.</summary>
        /// <remarks>The commands of this input will be added to formula if valid.</remarks>
        /// <param name="input">The input code to parse.</param>
        /// <returns>The formula for performing the parsed actions.</returns>
        public IAction Read(params string[] input) =>
            this.Read(input as IEnumerable<string>);

        /// <summary>Reads the given lines of input Blackboard code.</summary>
        /// <remarks>The commands of this input will be added to formula if valid.</remarks>
        /// <param name="input">The input code to parse.</param>
        /// <returns>The formula for performing the parsed actions.</returns>
        public IAction Read(IEnumerable<string> input, string name = "Unnamed") {
            PP.Parser.Result result = baseParser.Parse(new PP.Scanner.Default(input, name));

            // Check for parser errors.
            if (result.Errors.Length > 0)
                throw new Exception(result.Errors.Join("\n"));

            // process the resulting tree to build the formula.
            return this.read(result.Tree);
        }

        /// <summary>Reads the given parse tree root for an input Blackboard code.</summary>
        /// <param name="node">The parsed tree root node to read from.</param>
        /// <returns>The formula for performing the parsed actions.</returns>
        private IAction read(PP.ParseTree.ITreeNode node) {
            try {
                this.logger?.Log("Parser Read");
                Builder builder = new(this.slate, this.logger?.Sub);
                node.Process(this.prompts, builder);
                this.logger?.Log("Parser Finished");
                return builder.Actions.Current;
            } catch (S.Exception ex) {
                throw new Exception("Error occurred while parsing input code.", ex);
            }
        }

        #endregion
        #region Prompts Setup...

        /// <summary>Initializes the prompts and operators for this parser.</summary>
        private void initPrompts() {
            this.prompts = new Dictionary<string, PP.ParseTree.PromptHandle>();

            this.addHandler("clear", handleClear);
            this.addHandler("defineId", handleDefineId);
            this.addHandler("pushNamespace", handlePushNamespace);
            this.addHandler("popNamespace", handlePopNamespace);

            this.addHandler("newTypeInputNoAssign", handleNewTypeInputNoAssign);
            this.addHandler("newTypeInputWithAssign", handleNewTypeInputWithAssign);
            this.addHandler("newVarInputWithAssign", handleNewVarInputWithAssign);

            this.addHandler("typeDefine", handleTypeDefine);
            this.addHandler("varDefine", handleVarDefine);

            this.addHandler("provokeTrigger", handleProvokeTrigger);
            this.addHandler("conditionalProvokeTrigger", handleConditionalProvokeTrigger);

            this.addHandler("typeGet", handleTypeGet);
            this.addHandler("varGet", handleVarGet);

            this.addHandler("assignment", handleAssignment);
            this.addHandler("cast", handleCast);
            this.addHandler("memberAccess", handleMemberAccess);
            this.addHandler("startCall", handleStartCall);
            this.addHandler("addArg", handleAddArg);
            this.addHandler("endCall", handleEndCall);
            this.addHandler("pushId", handlePushId);
            this.addHandler("pushBool", handlePushBool);
            this.addHandler("pushBin", handlePushBin);
            this.addHandler("pushOct", handlePushOct);
            this.addHandler("pushInt", handlePushInt);
            this.addHandler("pushHex", handlePushHex);
            this.addHandler("pushDouble", handlePushDouble);
            this.addHandler("pushString", handlePushString);
            this.addHandler("pushType", handlePushType);

            this.addProcess(3, "ternary");
            this.addProcess(2, "logicalOr");
            this.addProcess(2, "logicalXor");
            this.addProcess(2, "logicalAnd");
            this.addProcess(2, "or");
            this.addProcess(2, "xor");
            this.addProcess(2, "and");
            this.addProcess(2, "equal");
            this.addProcess(2, "notEqual");
            this.addProcess(2, "greater");
            this.addProcess(2, "less");
            this.addProcess(2, "greaterEqual");
            this.addProcess(2, "lessEqual");
            this.addProcess(2, "shiftRight");
            this.addProcess(2, "shiftLeft");
            this.addProcess(2, "sum");
            this.addProcess(2, "subtract");
            this.addProcess(2, "multiply");
            this.addProcess(2, "divide");
            this.addProcess(2, "modulo");
            this.addProcess(2, "power");
            this.addProcess(1, "negate");
            this.addProcess(1, "not");
            this.addProcess(1, "invert");
        }

        /// <summary>This adds a handler for the given name.</summary>
        /// <param name="name">This is the name of the prompt this handler is for.</param>
        /// <param name="hndl">This is the handler to call on this prompt.</param>
        private void addHandler(string name, S.Action<Builder> hndl) =>
            this.prompts[name] = (PP.ParseTree.PromptArgs args) => {
                this.logger?.Log("Handle {0} [{1}]", name, args.LastLocation);
                hndl(args as Builder);
            };

        /// <summary>This adds a prompt for an operator handler.</summary>
        /// <param name="count">The number of values to pop off the stack for this function.</param>
        /// <param name="name">The name of the prompt to add to.</param>
        private void addProcess(int count, string name) {
            IFuncGroup funcGroup = this.slate.Global.Find(Slate.OperatorNamespace, name) as IFuncGroup;
            if (funcGroup is null)
                throw new Exception("Could not find the operation by the given name.").
                    With("Name", name);

            this.prompts[name] = (PP.ParseTree.PromptArgs args) => {
                Builder builder = args as Builder;
                PP.Scanner.Location loc = args.LastLocation;
                builder.Logger?.Log("Process {0}({1}) [{2}]", name, count, loc);

                INode[] inputs = builder.Nodes.Pop(count).Actualize().ToArray();
                INode result = funcGroup.Build(inputs);
                if (result is null)
                    throw new Exception("Could not perform the operation with the given input.").
                        With("Operation", name).
                        With("Input", inputs.Types().Strings().Join(", "));

                builder.Nodes.Push(result);
            };
        }

        /// <summary>Validates that all prompts in the grammar are handled.</summary>
        private void validatePrompts() {
            string[] unneeded = baseParser.UnneededPrompts(this.prompts);
            string[] missing  = baseParser.MissingPrompts(this.prompts);
            if (unneeded.Length > 0 || missing.Length > 0)
                throw new Exception("Blackboard's parser grammar has prompts which do not match prompt handlers.").
                    With("Not handled", unneeded.Join(", ")).
                    With("Not in grammar", missing.Join(", "));
        }

        /// <summary>A helper handler for parsing literals.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        /// <param name="usage">The string describing what this parse is doing.</param>
        /// <param name="parseMethod">The method to convert a string into a literal node.</param>
        static private void parseLiteral(Builder builder, string usage, S.Func<string, INode> parseMethod) {
            string text = builder.LastText;
            try {
                builder.Nodes.Push(parseMethod(text));
            } catch (S.Exception ex) {
                throw new Exception("Failed to " + usage + ".", ex).
                    With("Text", text).
                    With("Location", builder.LastLocation);
            }
        }

        #endregion
        #region Prompt Handlers...

        /// <summary>This is called before each statement to prepare and clean up the parser.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleClear(Builder builder) {
            builder.Tokens.Clear();
            builder.Clear();
        }

        /// <summary></summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleDefineId(Builder builder) =>
            builder.Identifiers.Push(builder.LastText);

        /// <summary>This is called when the namespace has opened.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushNamespace(Builder builder) {
            string name = builder.LastText;
            VirtualNode scope = builder.Scope.Current;

            // Check if the virtual namespace already exists.
            INode next = scope.ReadField(name);
            if (next is not null) {
                if (next is not VirtualNode nextspace)
                    throw new Exception("Can not open namespace. Another non-namespace exists by that name.").
                         With("Identifier", name).
                         With("Location", builder.LastLocation);
                builder.Scope.Push(nextspace);
                return;
            }

            // Create a new virtual namespace and an action to define the new namespace when this formula is run.
            Namespace newspace = new();
            builder.Actions.Add(new Define(scope.Receiver, name, newspace));
            VirtualNode nextScope = new(name, newspace);
            builder.Scope.Push(nextScope);
            scope.WriteField(name, nextScope);
        }

        /// <summary>This is called when the namespace had closed.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePopNamespace(Builder builder) => builder.Scope.Pop();

        /// <summary>This creates a new input node of a specific type without assigning the value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleNewTypeInputNoAssign(Builder builder) {
            string name = builder.Identifiers.Pop();
            Type type = builder.Types.Peek();
            builder.CreateInput(name, type);
        }

        /// <summary>This creates a new input node of a specific type and assigns it with an initial value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleNewTypeInputWithAssign(Builder builder) {
            INode value = builder.Nodes.Pop();
            Type type = builder.Types.Peek();
            string name = builder.Identifiers.Pop();
            INode target = builder.CreateInput(name, type);
            builder.AddAssignment(target, value);
        }

        /// <summary>This creates a new input node and assigns it with an initial value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleNewVarInputWithAssign(Builder builder) {
            INode value = builder.Nodes.Pop();
            string name = builder.Identifiers.Pop();
            Type type = Type.TypeOf(value);
            INode target = builder.CreateInput(name, type);
            builder.AddAssignment(target, value);
        }

        /// <summary>This handles defining a new typed named node.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleTypeDefine(Builder builder) {
            INode value = builder.Nodes.Pop();
            Type type = builder.Types.Peek();
            string name = builder.Identifiers.Pop();
            INode castValue = builder.PerformCast(type, value);

            VirtualNode curScope = builder.Scope.Current;
            IEnumerable<INode> allNewNodes = builder.PrepareTree(castValue);
            builder.Actions.Add(new Define(curScope.Receiver, name, castValue, allNewNodes));
            curScope.WriteField(name, castValue);
        }

        /// <summary>This handles defining a new untyped named node.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleVarDefine(Builder builder) {
            INode value = builder.Nodes.Pop();
            string name = builder.Identifiers.Pop();

            IEnumerable<INode> allNewNodes = builder.PrepareTree(value);
            builder.Actions.Add(new Define(builder.Scope.Current.Receiver, name, value, allNewNodes));
            builder.Scope.Current.WriteField(name, value);
        }

        /// <summary>This handles when a trigger is provoked unconditionally.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleProvokeTrigger(Builder builder) {
            INode target = builder.Nodes.Pop();
            builder.Actions.Add(Provoke.Create(builder.LastLocation, target));
        }

        /// <summary>This handles when a trigger should only be provoked if a condition returns true.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleConditionalProvokeTrigger(Builder builder) {
            INode target = builder.Nodes.Pop();
            INode value  = builder.Nodes.Pop();
            if (target is not ITriggerInput input)
                throw new Exception("Target node is not an input trigger.").
                    With("Target", target).
                    With("Value", value).
                    With("Location", builder.LastLocation);

            INode castValue = builder.PerformCast(Type.Trigger, value);
            builder.Actions.Add(new Provoke(input, castValue as ITrigger));

            // Push the condition onto the stack for any following trigger pulls.
            // See comment in `handleAssignment` about pushing cast value back onto the stack.
            builder.Nodes.Push(castValue);
        }

        static private void handleTypeGet(Builder builder) {
            // TODO: Implement, need to add a result argument to actions
            //       and a new action to write the gotten value to that result.
        }

        static private void handleVarGet(Builder builder) {
            // TODO: Implement
        }

        /// <summary>This handles assigning the left value to the right value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleAssignment(Builder builder) {
            INode value  = builder.Nodes.Pop();
            INode target = builder.Nodes.Pop();
            INode castValue = builder.AddAssignment(target, value);

            // Push the cast value back onto the stack for any following assignments.
            // By using the cast it makes it more like `X=Y=Z` is `Y=Z; X=Y;` but means that if a double and an int are being set by
            // an int the int must be assigned first then it can cast to a double for the double assignment, otherwise it will cast
            // to a double but not be able to implicitly cast that double back to an int. For example: if `int X; double Y;` then
            // `Y=X=3;` works and `X=Y=3` will not. One drawback for this way is that if you assign an int to multiple doubles it will
            // construct multiple cast nodes, but with a little optimization to remove duplicate node paths, this isn't an issue. 
            // Alternatively, if we push he value then `X=Y=Z` will be like `Y=Z; X=Z;`. 
            builder.Nodes.Push(castValue);
        }

        /// <summary>This handles performing a type cast of a node.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleCast(Builder builder) {
            INode value = builder.Nodes.Pop();
            Type type   = builder.Types.Pop();
            builder.Nodes.Push(builder.PerformCast(type, value, true));
        }

        /// <summary>This handles accessing an identifier to find the receiver for the next identifier.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleMemberAccess(Builder builder) {
            string name = builder.LastText;
            INode rNode = builder.Nodes.Pop();
            if (rNode is not IFieldReader receiver)
                throw new Exception("Unexpected node type for a member access. Expected a field reader.").
                    With("Node", rNode).
                    With("Name", name).
                    With("Location", builder.LastLocation);

            INode node = receiver.ReadField(name);
            if (node is not null) {
                builder.Nodes.Push(node);
                return;
            }

            throw new Exception("No identifier found in the receiver stack.").
                With("Identifier", name).
                With("Location", builder.LastLocation);
        }

        /// <summary>This handles preparing for a method call.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleStartCall(Builder builder) => builder.Arguments.Start();

        /// <summary>This handles the end of a method call and creates the node for the method.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleAddArg(Builder builder) => builder.Arguments.Add(builder.Nodes.Pop());

        /// <summary>This handles finishing a method call and building the node for the method.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleEndCall(Builder builder) {
            INode[] args = builder.Arguments.End();
            INode node = builder.Nodes.Pop();
            if (node is not IFuncGroup group)
                throw new Exception("Unexpected node type for a method call. Expected a function group.").
                    With("Node", node).
                    With("Input", args.Types().Strings().Join(", ")).
                    With("Location", builder.LastLocation);

            INode result = group.Build(args);
            if (result is null)
                throw new Exception("Could not perform the function with the given input.").
                    With("Function", group).
                    With("Input", args.Types().Strings().Join(", ")).
                    With("Location", builder.LastLocation);

            builder.Nodes.Push(result);
        }

        /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushId(Builder builder) {
            string name = builder.LastText;
            foreach (VirtualNode scope in builder.Scope.Scopes) {
                INode node = scope.ReadField(name);
                if (node is not null) {
                    builder.Nodes.Push(node);
                    builder.Existing.Add(node);
                    return;
                }
            }

            S.Console.WriteLine(builder.Scope.Global);

            throw new Exception("No identifier found in the scope stack.").
                With("Identifier", name).
                With("Location", builder.LastLocation);
        }

        /// <summary>This handles pushing a bool literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushBool(Builder builder) =>
            parseLiteral(builder, "parse a bool", (string text) => Literal.Bool(bool.Parse(text)));

        /// <summary>This handles pushing a binary int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushBin(Builder builder) =>
            parseLiteral(builder, "parse a binary int", (string text) => Literal.Int(S.Convert.ToInt32(text, 2)));

        /// <summary>This handles pushing an octal int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushOct(Builder builder) =>
            parseLiteral(builder, "parse a octal int", (string text) => Literal.Int(S.Convert.ToInt32(text, 8)));

        /// <summary>This handles pushing a decimal int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushInt(Builder builder) =>
            parseLiteral(builder, "parse a decimal int", (string text) => Literal.Int(int.Parse(text)));

        /// <summary>This handles pushing a hexadecimal int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushHex(Builder builder) =>
            parseLiteral(builder, "parse a hex int", (string text) => Literal.Int(int.Parse(text, NumberStyles.HexNumber)));

        /// <summary>This handles pushing a double literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushDouble(Builder builder) =>
            parseLiteral(builder, "parse a double", (string text) => Literal.Double(double.Parse(text)));

        /// <summary>This handles pushing a string literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushString(Builder builder) =>
            parseLiteral(builder, "decode escaped sequences", (string text) => Literal.String(PP.Misc.Text.Unescape(text)));

        /// <summary>This handles pushing a type onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushType(Builder builder) {
            string text = builder.LastText;
            Type t = Type.FromName(text);
            if (t is null)
                throw new Exception("Unrecognized type name.").
                    With("Text", text).
                    With("Location", builder.LastLocation);
            builder.Types.Push(t);
        }

        #endregion
    }
}
