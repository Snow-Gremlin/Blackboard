using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
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

    /// <summary>This will parse the Blackboard language into actions and nodes to apply to the driver.</summary>
    sealed public class Parser {

        /// <summary>The resource file for the Blackboard language definition.</summary>
        private const string resourceName = "Blackboard.Parser.Parser.lang";

        /// <summary>Prepares the parser's static variables before they are used.</summary>
        static Parser() => baseParser = PP.Loader.Loader.LoadParser(
                PP.Scanner.Default.FromResource(Assembly.GetExecutingAssembly(), resourceName));

        /// <summary>The Blackboard language base parser lazy singleton.</summary>
        static private readonly PP.Parser.Parser baseParser;

        private readonly Driver driver;

        private Dictionary<string, PP.ParseTree.PromptHandle> prompts;

        /// <summary>Creates a new Blackboard language parser.</summary>
        /// <param name="driver">The driver to modify.</param>
        public Parser(Driver driver) {
            this.driver = driver;
            this.prompts = null;

            this.initPrompts();
            this.validatePrompts();
        }

        #region Read/Formula Methods...

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
            if (result.Errors.Length > 0) throw new Exception(result.Errors.Join("\n"));

            // process the resulting tree to build the formula.
            return this.read(result.Tree);
        }

        /// <summary>Reads the given parse tree root for an input Blackboard code.</summary>
        /// <param name="node">The parsed tree root node to read from.</param>
        /// <returns>The formula for performing the parsed actions.</returns>
        private IAction read(PP.ParseTree.ITreeNode node) {
            try {
                Builder stacks = new(this.driver);
                node.Process(this.prompts, stacks);
                return stacks.ToAction();
            } catch (S.Exception ex) {
                throw new Exception("Error occurred while parsing input code.", ex);
            }
        }

        #endregion
        #region Prompts Setup...

        /// <summary>Initializes the prompts and operators for this parser.</summary>
        private void initPrompts() {
            this.prompts = new Dictionary<string, PP.ParseTree.PromptHandle>();

            this.addHandler("clear",         handleClear);
            this.addHandler("defineId",      handleDefineId);
            this.addHandler("pushNamespace", handlePushNamespace);
            this.addHandler("popNamespace",  handlePopNamespace);

            this.addHandler("newTypeInputNoAssign",   handleNewTypeInputNoAssign);
            this.addHandler("newTypeInputWithAssign", handleNewTypeInputWithAssign);
            this.addHandler("newVarInputWithAssign",  handleNewVarInputWithAssign);

            this.addHandler("typeDefine", handleTypeDefine);
            this.addHandler("varDefine",  handleVarDefine);

            this.addHandler("provokeTrigger",            handleProvokeTrigger);
            this.addHandler("conditionalProvokeTrigger", handleConditionalProvokeTrigger);

            this.addHandler("typeGet", handleTypeGet);
            this.addHandler("varGet",  handleVarGet);

            this.addHandler("assignment",   handleAssignment);
            this.addHandler("cast",         handleCast);
            this.addHandler("memberAccess", handleMemberAccess);
            this.addHandler("startCall",    handleStartCall);
            this.addHandler("addArg",       handleAddArg);
            this.addHandler("endCall",      handleEndCall);
            this.addHandler("pushId",       handlePushId);
            this.addHandler("pushBool",     handlePushBool);
            this.addHandler("pushBin",      handlePushBin);
            this.addHandler("pushOct",      handlePushOct);
            this.addHandler("pushInt",      handlePushInt);
            this.addHandler("pushHex",      handlePushHex);
            this.addHandler("pushDouble",   handlePushDouble);
            this.addHandler("pushString",   handlePushString);
            this.addHandler("pushType",     handlePushType);

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
            this.addProcess(2, "remainder");
            this.addProcess(2, "power");
            this.addProcess(1, "negate");
            this.addProcess(1, "not");
            this.addProcess(1, "invert");
        }

        /// <summary>This adds a handler for the given name.</summary>
        /// <param name="name">This is the name of the prompt this handler is for.</param>
        /// <param name="hndl">This is the handler to call on this prompt.</param>
        private void addHandler(string name, S.Action<Builder> hndl) =>
            this.prompts[name] = (PP.ParseTree.PromptArgs args) => hndl(args as Builder);

        /// <summary>This adds a prompt for an operator handler.</summary>
        /// <param name="count">The number of values to pop off the stack for this function.</param>
        /// <param name="name">The name of the prompt to add to.</param>
        private void addProcess(int count, string name) {
            IFuncGroup funcGroup = this.driver.Global.Find(Driver.OperatorNamespace, name) as IFuncGroup;
            this.prompts[name] = (PP.ParseTree.PromptArgs args) => {
                Builder builder = args as Builder;
                PP.Scanner.Location loc = args.LastLocation;
                INode[] inputs = builder.Pop(count);
                INode result = funcGroup.Build(inputs);
                if (result is null)
                    throw new Exception("Could not perform the operation with the given input.").
                        With("Operation", name).
                        With("Input", inputs.Types().Strings().Join(", "));
                builder.Push(result);
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

        #endregion
        #region Helper Methods...

        /// <summary>Performs a cast if needed from one value to another by creating a new node.</summary>
        /// <remarks>If a cast is added then the cast will be added to the builder as a new node.</remarks>
        /// <param name="builder">The formula builder being used.</param>
        /// <param name="type">The type to cast the value to.</param>
        /// <param name="value">The value to cast to the given type.</param>
        /// <param name="explicitCasts">
        /// Indicates if explicit casts are allowed to be used when casting.
        /// If false then only inheritance or implicit casts will be used.
        /// </param>
        /// <returns>The cast value or the given value in the given type.</returns>
        static private INode performCast(Builder builder, Type type, INode value, bool explicitCasts = false) {
            Type valueType = Type.TypeOf(value);
            TypeMatch match = type.Match(valueType, explicitCasts);
            if (!match.IsAnyCast)
                return match.IsMatch ? value :
                    throw new Exception("The value type can not be cast to the given type.").
                        With("Location", builder.LastLocation).
                        With("Target", type).
                        With("Type", valueType).
                        With("Value", value);

            Namespace ops = builder.Driver.Global.Find(Driver.OperatorNamespace) as Namespace;
            INode castGroup =
                type == Type.Bool    ? ops.Find("castBool") :
                type == Type.Int     ? ops.Find("castInt") :
                type == Type.Double  ? ops.Find("castDouble") :
                type == Type.String  ? ops.Find("castString") :
                type == Type.Trigger ? ops.Find("castTrigger") :
                throw new Exception("Unsupported type for new definition cast").
                    With("Location", builder.LastLocation).
                    With("Type", type);
            INode castValue = (castGroup as IFuncGroup).Build(value);
            builder.AddNewNode(castValue);
            return castValue;
        }

        /// <summary>Creates an assignment action and adds it to the builder if possible.</summary>
        /// <param name="builder">The formula builder being used.</param>
        /// <param name="target">The node to assign the value to.</param>
        /// <param name="value">The value to assign to the given target node.</param>
        /// <returns>The cast value or given value which was used in the assignment.</returns>
        static private INode addAssignment(Builder builder, INode target, INode value) {
            // Check if the base types match. Don't need to check that the type is
            // a data type or trigger since only those can be reduced to constants.
            PP.Scanner.Location loc = builder.LastLocation;
            Type targetType = Type.TypeOf(target);
            INode castValue = performCast(builder, targetType, value);
            IAction assign =
                targetType == Type.Bool    ? Assign<Bool>.  Create(loc, target, castValue) :
                targetType == Type.Int     ? Assign<Int>.   Create(loc, target, castValue) :
                targetType == Type.Double  ? Assign<Double>.Create(loc, target, castValue) :
                targetType == Type.String  ? Assign<String>.Create(loc, target, castValue) :
                targetType == Type.Trigger ? Provoke.       Create(loc, target, castValue) :
                throw new Exception("Unsupported type for an assignment").
                    With("Location", loc).
                    With("Type", targetType).
                    With("Input", target).
                    With("Value", value);
            builder.AddAction(assign);
            return castValue;
        }

        /// <summary>Creates a new input node with the given name in the local scope.</summary>
        /// <param name="builder">The formula builder being used.</param>
        /// <param name="name">The name to create the input for.</param>
        /// <param name="type">The type of input to create.</param>
        /// <returns>The newly created input.</returns>
        static private INode createInput(Builder builder, string name, Type type) {
            if (builder.CurrentScope.ContainsField(name))
                throw new Exception("A node already exists with the given name.").
                    With("Name", name).
                    With("Type", type);

            INode node =
                type == Type.Bool    ? new InputValue<Bool>() :
                type == Type.Int     ? new InputValue<Int>() :
                type == Type.Double  ? new InputValue<Double>() :
                type == Type.String  ? new InputValue<String>() :
                type == Type.Trigger ? new InputTrigger() :
                throw new Exception("Unsupported type for new typed input").
                    With("Location", builder.LastLocation).
                    With("Type", type);

            builder.AddAction(new Define(builder.CurrentScope.Receiver, name, node, builder.NewNodes));
            builder.CurrentScope.WriteField(name, node);
            builder.ClearNewNodes();
            return node;
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
        static private void handleDefineId(Builder builder) => builder.PushId(builder.LastText);

        /// <summary>This is called when the namespace has opened.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushNamespace(Builder builder) {
            string name = builder.LastText;
            VirtualNode scope = builder.CurrentScope;

            // Check if the virtual namespace already exists.
            INode next = scope.ReadField(name);
            if (next is not null) {
                if (next is not VirtualNode nextspace)
                    throw new Exception("Can not open namespace. Another non-namespace exists by that name.").
                         With("Identifier", name).
                         With("Location", builder.LastLocation);
                builder.PushScope(nextspace);
                return;
            }

            // Create a new virtual namespace and an action to define the new namespace when this formula is run.
            Namespace newspace = new();
            VirtualNode nextScope = new(name, newspace);
            builder.AddAction(new Define(scope, name, newspace));
            builder.PushScope(nextScope);
        }

        /// <summary>This is called when the namespace had closed.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePopNamespace(Builder builder) => builder.PopScope();

        /// <summary>This creates a new input node of a specific type without assigning the value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleNewTypeInputNoAssign(Builder builder) {
            string name = builder.PopId();
            Type type = builder.PeekType();
            createInput(builder, name, type);
        }

        /// <summary>This creates a new input node of a specific type and assigns it with an initial value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleNewTypeInputWithAssign(Builder builder) {
            INode value = builder.Pop();
            Type type = builder.PeekType();
            string name = builder.PopId();
            INode target = createInput(builder, name, type);
            addAssignment(builder, target, value);
        }

        /// <summary>This creates a new input node and assigns it with an initial value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleNewVarInputWithAssign(Builder builder) {
            INode value = builder.Pop();
            string name = builder.PopId();
            Type type = Type.TypeOf(value);
            INode target = createInput(builder, name, type);
            addAssignment(builder, target, value);
        }

        /// <summary>This handles defining a new typed named node.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleTypeDefine(Builder builder) {
            INode value = builder.Pop();
            Type type = builder.PeekType();
            string name = builder.PopId();
            INode castValue = performCast(builder, type, value);
            builder.AddAction(new Define(builder.CurrentScope.Receiver, name, castValue, builder.NewNodes));
            builder.CurrentScope.WriteField(name, castValue);
            builder.ClearNewNodes();
        }

        /// <summary>This handles defining a new untyped named node.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleVarDefine(Builder builder) {
            INode value = builder.Pop();
            string name = builder.PopId();
            builder.AddAction(new Define(builder.CurrentScope.Receiver, name, value, builder.NewNodes));
            builder.CurrentScope.WriteField(name, value);
            builder.ClearNewNodes();
        }

        /// <summary>This handles when a trigger is provoked unconditionally.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleProvokeTrigger(Builder builder) {
            INode target = builder.Pop();
            builder.AddAction(Provoke.Create(builder.LastLocation, target));
        }

        /// <summary>This handles when a trigger should only be provoked if a condition returns true.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleConditionalProvokeTrigger(Builder builder) {
            INode target = builder.Pop();
            INode value  = builder.Pop();
            if (target is not ITriggerInput input)
                throw new Exception("Target node is not an input trigger.").
                    With("Target", target).
                    With("Value", value).
                    With("Location", builder.LastLocation);

            INode castValue = performCast(builder, Type.Trigger, value);
            builder.AddAction(new Provoke(input, castValue as ITrigger));

            // Push the condition onto the stack for any following trigger pulls.
            // See comment in `handleAssignment` about pushing cast value back onto the stack.
            builder.Push(castValue);
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
            INode value  = builder.Pop();
            INode target = builder.Pop();
            INode castValue = addAssignment(builder, target, value);

            // Push the cast value back onto the stack for any following assignments.
            // By using the cast it makes it more like `X=Y=Z` is `Y=Z; X=Y;` but means that if a double and an int are being set by
            // an int the int must be assigned first then it can cast to a double for the double assignment, otherwise it will cast
            // to a double but not be able to implicitly cast that double back to an int. For example: if `int X; double Y;` then
            // `Y=X=3;` works and `X=Y=3` will not. One drawback for this way is that if you assign an int to multiple doubles it will
            // construct multiple cast nodes, but with a little optimization to remove duplicate node paths, this isn't an issue. 
            // Alternatively, if we push he value then `X=Y=Z` will be like `Y=Z; X=Z;`. 
            builder.Push(castValue);
        }

        /// <summary>This handles performing a type cast of a node.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleCast(Builder builder) {
            INode value = builder.Pop();
            Type type   = builder.PopType();
            builder.PushNew(performCast(builder, type, value, true));
        }

        /// <summary>This handles accessing an identifier to find the receiver for the next identifier.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleMemberAccess(Builder builder) {
            string name = builder.LastText;
            INode rNode = builder.Pop();
            if (rNode is not IFieldReader receiver)
                throw new Exception("Unexpected node type for a member access. Expected a field reader.").
                    With("Node", rNode).
                    With("Name", name).
                    With("Location", builder.LastLocation);

            INode node = receiver.ReadField(name);
            if (node is not null) {
                if (node is VirtualNode vNode) node = vNode.Receiver;
                builder.Push(node);
                return;
            }

            throw new Exception("No identifier found in the receiver stack.").
                With("Identifier", name).
                With("Location", builder.LastLocation);
        }

        /// <summary>This handles preparing for a method call.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleStartCall(Builder builder) => builder.StartArgs();

        /// <summary>This handles the end of a method call and creates the node for the method.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleAddArg(Builder builder) => builder.AddArg(builder.Pop());

        /// <summary>This handles finishing a method call and building the node for the method.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleEndCall(Builder builder) {
            INode[] args = builder.EndArgs();
            INode node = builder.Pop();
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

            if (!args.Contains(result)) builder.AddNewNode(result);
            builder.Push(result);
        }

        /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushId(Builder builder) {
            string name = builder.LastText;
            foreach (VirtualNode scope in builder.Scopes) {
                INode node = scope.ReadField(name);
                if (node is not null) {
                    if (node is VirtualNode vNode) node = vNode.Receiver;
                    builder.Push(node);
                    return;
                }
            }

            throw new Exception("No identifier found in the scope stack.").
                With("Identifier", name).
                With("Location", builder.LastLocation);
        }

        /// <summary>This handles pushing a bool literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushBool(Builder builder) {
            string text = builder.LastText;
            try {
                bool value = bool.Parse(text);
                builder.PushNew(Literal.Bool(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a bool.", ex).
                    With("Text", text).
                    With("Location", builder.LastLocation);
            }
        }

        /// <summary>This handles pushing a binary int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushBin(Builder builder) {
            string text = builder.LastText;
            try {
                int value = S.Convert.ToInt32(text, 2);
                builder.PushNew(Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a binary int.", ex).
                    With("Text", text).
                    With("Location", builder.LastLocation);
            }
        }

        /// <summary>This handles pushing an octal int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushOct(Builder builder) {
            string text = builder.LastText;
            try {
                int value = S.Convert.ToInt32(text, 8);
                builder.PushNew(Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse an octal int.", ex).
                    With("Text", text).
                    With("Location", builder.LastLocation);
            }
        }

        /// <summary>This handles pushing a decimal int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushInt(Builder builder) {
            string text = builder.LastText;
            try {
                int value = int.Parse(text);
                builder.PushNew(Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a decimal int.", ex).
                    With("Text", text).
                    With("Location", builder.LastLocation);
            }
        }

        /// <summary>This handles pushing a hexadecimal int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushHex(Builder builder) {
            string text = builder.LastText[2..];
            try {
                int value = int.Parse(text, NumberStyles.HexNumber);
                builder.PushNew(Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a hex int.", ex).
                    With("Text", text).
                    With("Location", builder.LastLocation);
            }
        }

        /// <summary>This handles pushing a double literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushDouble(Builder builder) {
            string text = builder.LastText;
            try {
                double value = double.Parse(text);
                builder.PushNew(Literal.Double(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a double.", ex).
                    With("Text", text).
                    With("Location", builder.LastLocation);
            }
        }

        /// <summary>This handles pushing a string literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushString(Builder builder) {
            string text = builder.LastText;
            try {
                string value = PP.Misc.Text.Unescape(text);
                builder.PushNew(Literal.String(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to decode escaped sequences.", ex).
                    With("Text", text).
                    With("Location", builder.LastLocation);
            }
        }

        /// <summary>This handles pushing a type onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushType(Builder builder) {
            string text = builder.LastText;
            Type t = Type.FromName(text);
            if (t is null)
                throw new Exception("Unrecognized type name.").
                    With("Text", text).
                    With("Location", builder.LastLocation);
            builder.PushType(t);
        }

        #endregion
    }
}
