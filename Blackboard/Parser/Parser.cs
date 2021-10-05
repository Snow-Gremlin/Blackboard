﻿using Blackboard.Core;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Data.Caps;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using PP = PetiteParser;
using S = System;
using Blackboard.Parser.Prepers;
using Blackboard.Parser.Performers;

namespace Blackboard.Parser {

    /// <summary>This will parse the Blackboard language into actions and nodes to apply to the driver.</summary>
    sealed public class Parser {

        /// <summary>The resource file for the Blackboard language definition.</summary>
        private const string resourceName = "Blackboard.Parser.Parser.lang";

        /// <summary>Prepares the parser's static variables before they are used.</summary>
        static Parser() {
            BaseParser = PP.Loader.Loader.LoadParser(
                PP.Scanner.Default.FromResource(Assembly.GetExecutingAssembly(), resourceName));
        }

        /// <summary>The Blackboard language base parser lazy singleton.</summary>
        static private readonly PP.Parser.Parser BaseParser;

        private readonly Driver driver;
        private readonly Formula formula;
        private Dictionary<string, PP.ParseTree.PromptHandle> prompts;

        private readonly LinkedList<object> stash;
        private readonly LinkedList<IPreper> stack;

        /// <summary>Creates a new Blackboard language parser.</summary>
        /// <param name="driver">The driver to modify.</param>
        public Parser(Driver driver) {
            this.driver = driver;
            this.formula = new Formula(driver);
            this.prompts = null;

            this.stash = new LinkedList<object>();
            this.stack = new LinkedList<IPreper>();

            this.initPrompts();
            this.validatePrompts();
        }

        #region Formula Methods...

        /// <summary>Reads the given lines of input Blackline code.</summary>
        /// <remarks>The commands of this input will be added to formula if valid.</remarks>
        /// <param name="input">The input code to parse.</param>
        public void Read(params string[] input) =>
            this.Read(input as IEnumerable<string>);

        /// <summary>Reads the given lines of input Blackline code.</summary>
        /// <remarks>The commands of this input will be added to formula if valid.</remarks>
        /// <param name="input">The input code to parse.</param>
        public void Read(IEnumerable<string> input, string name = "Unnamed") {
            PP.Parser.Result result = BaseParser.Parse(new PP.Scanner.Default(input, name));
            if (result.Errors.Length > 0)
                throw new Exception(string.Join('\n', result.Errors));
            this.read(result.Tree);
        }

        /// <summary>Reads the given parse tree root for an input Blackline code.</summary>
        /// <param name="node">The parsed tree root node to read from.</param>
        private void read(PP.ParseTree.ITreeNode node) =>
            node.Process(this.prompts);

        /// <summary>This will dispose of all pending actions.</summary>
        public void Discard() => this.formula.Reset();

        /// <summary>This will perform and apply all pending action to Blackboard.</summary>
        public void Commit() => this.formula.Perform();

        #endregion
        #region Prompts Setup...

        /// <summary>Initializes the prompts and operators for this parser.</summary>
        private void initPrompts() {
            this.prompts = new Dictionary<string, PP.ParseTree.PromptHandle>() {
                { "clear",         this.handleClear },
                { "pushNamespace", this.handlePushNamespace },
                { "popNamespace",  this.handlePopNamespace },

                { "newTypeInputNoAssign",   this.handleNewTypeInputNoAssign },
                { "newTypeInputWithAssign", this.handleNewTypeInputWithAssign },
                { "newVarInputWithAssign",  this.handleNewVarInputWithAssign },

                //{ "startDefine",            this.handleStartDefine },
                //{ "typeDefine",             this.handleTypeDefine },
                //{ "varDefine",              this.handleVarDefine },
                //{ "pullTrigger",            this.handlePullTrigger },
                //{ "conditionalPullTrigger", this.handleConditionalPullTrigger },

                { "assignment",   this.handleAssignment },
                { "cast",         this.handleCast },
                { "memberAccess", this.handleMemberAccess },
                { "startCall",    this.handleStartCall },
                { "endCall",      this.handleEndCall },
                { "pushId",       this.handlePushId },
                { "pushBool",     this.handlePushBool },
                { "pushBin",      this.handlePushBin },
                { "pushOct",      this.handlePushOct },
                { "pushInt",      this.handlePushInt },
                { "pushHex",      this.handlePushHex },
                { "pushDouble",   this.handlePushDouble },
                { "pushString",   this.handlePushString },
                { "pushType",     this.handlePushType },
            };

            this.addProcess(3, "trinary");
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

        /// <summary>This adds a prompt for an operator handler.</summary>
        /// <param name="count">The number of values to pop off the stack for this function.</param>
        /// <param name="name">The name of the prompt to add to.</param>
        private void addProcess(int count, string name) {
            FuncGroup op = this.driver.Global.Find(Driver.OperatorNamespace, name) as FuncGroup;
            this.prompts[name] = (PP.ParseTree.PromptArgs args) => {
                PP.Scanner.Location loc = args.Tokens[^1].End;
                IPreper[] inputs = this.pop<IPreper>(count);
                this.push(new FunctionPrep(op, name, loc, inputs));
            };
        }
    
        /// <summary>Validates that all prompts in the grammar are handled.</summary>
        private void validatePrompts() {
            // TODO: Move most of this over to PetiteParser.
            HashSet<string> remaining = new(BaseParser.Grammar.Prompts.Select((prompt) => prompt.Name));
            HashSet<string> missing = new();
            foreach (string name in this.prompts.Keys) {
                if (remaining.Contains(name)) remaining.Remove(name);
                else missing.Add(name);
            }
            if (remaining.Count > 0 || missing.Count > 0)
                throw new Exception("Blackboard's parser grammer has prompts which do not match prompt handlers.").
                    With("Not handled", string.Join(", ", remaining)).
                    With("Not in grammer", string.Join(", ", missing));
        }

        #endregion
        #region Stack Helpers...

        /// <summary>Pushes a preper onto the stack.</summary>
        /// <param name="preper">The preper to push.</param>
        private void push(IPreper preper) => this.stack.AddLast(preper);

        /// <summary>Pops off a preper is on the top of the stack.</summary>
        /// <typeparam name="T">The type of the preper to read as.</typeparam>
        /// <returns>The preper which was on top of the stack.</returns>
        private T pop<T>() where T : class, IPreper {
            IPreper item = this.stack.Last.Value;
            this.stack.RemoveLast();
            return item as T;
        }

        /// <summary>Pops one or more preper off the stack.</summary>
        /// <typeparam name="T">The types of the prepers to read.</typeparam>
        /// <param name="count">The number of prepers to pop.</param>
        /// <returns>The popped prepers in the order oldest to newest.</returns>
        private T[] pop<T>(int count) where T: class, IPreper {
            T[] items = new T[count];
            for (int i = 0; i < count; i++)
                items[count-1-i] = this.pop<T>();
            return items;
        }

        /// <summary>Pushes an object onto the stash stack.</summary>
        /// <param name="value">The value to push.</param>
        private void stashPush(object value) => this.stash.AddLast(value);

        /// <summary>Pops off an object is on the top of the stash stack.</summary>
        /// <typeparam name="T">The type of the object to read as.</typeparam>
        /// <returns>The object which was on top of the stash stack.</returns>
        private T stashPop<T>() where T : class {
            object value = this.stash.Last.Value;
            this.stash.RemoveLast();
            return value as T;
        }

        #endregion
        #region Prompt Handlers...

        /// <summary>This is called before each statement to prepare and clean up the parser.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleClear(PP.ParseTree.PromptArgs args) {
            args.Tokens.Clear();
            this.stash.Clear();
            this.stack.Clear();
        }

        /// <summary>This is called when the namespace has openned.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushNamespace(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string name = token.Text;

            IWrappedNode scope = this.formula.CurrentScope;
            IWrappedNode next = scope.ReadField(name);
            if (next is not null) {
                if (next.Type.IsAssignableTo(typeof(Namespace)))
                    throw new Exception("Can not open namespace. Another non-namespace exists by that name.").
                         With("Identifier", name).
                         With("Location", loc);
                scope = next;
            } else {
                VirtualNode nextScope = new(name, typeof(Namespace), scope);
                scope.


                scope = nextScope;
            }

            this.formula.PushScope(scope);
        }

        /// <summary>This is called when the namespace had closed.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePopNamespace(PP.ParseTree.PromptArgs args) {
            this.scopeStack.RemoveFirst();
        }

        /// <summary>This creates a new input node of a specific type without assigning the value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleNewTypeInputNoAssign(PP.ParseTree.PromptArgs args) {
            IAssignable targetActor = this.pop<IAssignable>();
            Stash<Type> typeActor   = this.pop<Stash<Type>>();

            Type t = typeActor.Value;
            INode newNode =
                t == Type.Bool    ? new InputValue<Bool>() :
                t == Type.Int     ? new InputValue<Int>() :
                t == Type.Double  ? new InputValue<Double>() :
                t == Type.String  ? new InputValue<String>() :
                t == Type.Trigger ? new InputTrigger() :
                throw new Exception("Unsupported type for new input").
                    With("Type", t);

            targetActor.Assign(newNode, true);

            // Push the type back onto the stack for the next assignment.
            this.push(typeActor);
        }

        /// <summary>This creates a new input node of a specific type and assigns it with an initial value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleNewTypeInputWithAssign(PP.ParseTree.PromptArgs args) {
            StackItem valueItem = this.pop();
            IAssignable targetActor = this.pop<IAssignable>();
            Stash<Type> typeItem    = this.pop<Stash<Type>>();

            if (idItem.Value is not null)
                throw new Exception("Can not create new input node. Identifier already exists in the receiver.").
                     With("Identifier", idItem);
            if (!idItem.HasId)
                throw new Exception("Can not create new input node with no Identifier.").
                     With("Identifier", idItem);

            Type t = typeItem.ValueAs<Type>();
            INode value = t.Implicit(valueItem.ValueAs<INode>());
            if (value is null)
                throw new Exception("The new input node can not be assigned the value given to it.").
                     With("Value",      valueItem).
                     With("Type",       typeItem).
                     With("Identifier", idItem);

            // Unless the identifier has a receiver, add the new input node to the current namespace scope.
            Namespace scope = this.scopeStack.First.Value;
            scope[idItem.Id] =
                t == Type.Bool    ? new InputValue<Bool>(  (value as IValue<Bool>  ).Value) :
                t == Type.Int     ? new InputValue<Int>(   (value as IValue<Int>   ).Value) :
                t == Type.Double  ? new InputValue<Double>((value as IValue<Double>).Value) :
                t == Type.String  ? new InputValue<String>((value as IValue<String>).Value) :
                t == Type.Trigger ? new InputTrigger(      (value as ITrigger      ).Provoked) :
                throw new Exception("Unsupported type for new input").
                    With("Type", typeItem);

            // Push the type back onto the stack for the next assignment.
            this.push(typeItem);
        }

        /// <summary>This creates a new input node and assigns it with an initial value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleNewVarInputWithAssign(PP.ParseTree.PromptArgs args) {
            StackItem valueItem = this.pop();
            StackItem idItem    = this.pop();

            if (idItem.Value is not null)
                throw new Exception("Can not create new input node. Identifier already exists in the receiver.").
                     With("Identifier", idItem);
            if (!idItem.HasId)
                throw new Exception("Can not create new input node with no Identifier.").
                     With("Identifier", idItem);

            // Pull the type from the value that is going to be assigned.
            INode value = valueItem.ValueAs<INode>();
            Type t = Type.TypeOf(value);

            // Unless the identifier has a receiver, add the new input node to the current namespace scope.
            Namespace scope = this.scopeStack.First.Value;
            scope[idItem.Id] =
                t == Type.Bool    ? new InputValue<Bool>(  (value as IValue<Bool>  ).Value) :
                t == Type.Int     ? new InputValue<Int>(   (value as IValue<Int>   ).Value) :
                t == Type.Double  ? new InputValue<Double>((value as IValue<Double>).Value) :
                t == Type.String  ? new InputValue<String>((value as IValue<String>).Value) :
                t == Type.Trigger ? new InputTrigger(      (value as ITrigger      ).Provoked) :
                throw new Exception("Unsupported type from assignment for new input").
                    With("Type", t);
        }

        /*
        /// <summary>This assigns several existing input nodes with a new value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleAssignExisting(PP.ParseTree.PromptArgs args) {
            INode right = this.popNode().First();
            while (this.stack.Count > 0) {
                Identifier id = this.pop<Identifier>();
                INode left = this.find(id);
                if (left is null) throw new Exception("Unknown input variable " + id + " at " + id.Location + ".");
                else if (left is IValueInput<bool>   leftBool)    leftBool.   SetValue(Cast.AsBoolValue(   right));
                else if (left is IValueInput<int>    leftInt)     leftInt.    SetValue(Cast.AsIntValue(    right));
                else if (left is IValueInput<double> leftDouble)  leftDouble. SetValue(Cast.AsDoubleValue( right));
                else if (left is ITriggerInput       leftTrigger) leftTrigger.Trigger( Cast.AsTriggerValue(right));
                else throw new Exception("Unable to assign to " + Cast.TypeName(left) + " at " + id.Location + ".");
            }
        }
        */

        /*
        /// <summary>This handles defining a new typed output node.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleTypeDefine(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop<Identifier>();
            this.defineValue(loc, this.typeText, id, right);
        }

        /// <summary>This handles defining a new untyped output node.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleVarDefine(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop<Identifier>();
            this.defineValue(loc, Cast.TypeName(right), id, right);
        }

        /// <summary>This handles when a trigger is provoked unconditionally.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePullTrigger(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Identifier id = this.pop<Identifier>();
            this.provokeTrigger(loc, id);
        }

        /// <summary>This handles when a trigger should only be provoked if a condition returns true.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleConditionalPullTrigger(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Identifier id = this.pop<Identifier>();
            INode node = this.popNode().First();
            if (node is not IValue<bool> boolNode)
                throw new Exception("May only conditionally provoke the trigger " + id + " with a bool or trigger at " + loc + ".");
            if (boolNode.Value) this.provokeTrigger(loc, id);
        }
        */

        /*
        /// <summary>This handles setting the value type.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleSetType(PP.ParseTree.PromptArgs args) =>
            this.typeText = args.Tokens[^1].Text;
        */


        /// <summary>This handles assigning the left value to the right value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleAssignment(PP.ParseTree.PromptArgs args) {


            // TODO: IMPLEMENT


        }

        /// <summary>This handles performing a type cast of a node.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleCast(PP.ParseTree.PromptArgs args) {
            //NodeItem right = this.pop<NodeItem>();
            //Identifier left = this.pop<Identifier>();

            // TODO: IMPLEMENT

        }

        /// <summary>This handles accessing an identifier to find the receiver for the next identifier.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleMemberAccess(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string id = token.Text;
            StackItem left = this.pop();

            if (!left.ValueIs<Namespace>())
                throw new Exception("The receiver for the identifier is not a namespace.").
                    With("Stack Item", left).
                    With("Identifier", id).
                    With("Loaction", loc);

            Namespace scope = left.ValueAs<Namespace>();
            object value = scope.ContainsKey(id) ? scope[id] : null;
            this.push(loc, left.Id + "." + id, value);
        }

        /// <summary>This handles preparing for a method call.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleStartCall(PP.ParseTree.PromptArgs args) {
            StackItem item = this.pop();
            if (!item.ValueIs<FuncGroup>())
                throw new Exception("Identifier was not a function.").
                    With("Stack Item", item);
            this.push(item);
        }

        /// <summary>This handles the end of a method call and creates the node for the method.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleEndCall(PP.ParseTree.PromptArgs args) {
            LinkedList<INode> funcArgs = new();
            StackItem callItem;
            while (true) {
                StackItem item = this.pop();
                if (item.ValueIs<FuncGroup>()) {
                    callItem = item;
                    break;
                }
                funcArgs.AddFirst(item.ValueAs<INode>());
            }

            FuncGroup func = callItem.ValueAs<FuncGroup>();
            INode node = func.Build(funcArgs.ToArray());
            if (node is null)
                throw new Exception("Failed to find a function that can accept the given argument types.").
                    With("Function", callItem).
                    With("Args", string.Join(", ", funcArgs.TypeNames()));

            if (funcArgs.IsConstant()) node = node.ToLiteral();
            this.push(callItem.Location, node);
        }

        /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushId(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string text = token.Text;

            object value = null;
            foreach (Namespace scope in this.scopeStack) {
                if (scope.ContainsKey(text)) {
                    value =  scope[text];
                    if (this.reduceNodes && value is INode node)
                        value = node.ToLiteral();
                    break;
                }
            }

            // If the value is not found in the scopes, that is fine, set it as null,
            // it is likely about to be used for creating a new identifier or
            // will be caught when attempted to be used.
            this.push(loc, text, value);
        }

        /// <summary>This handles pushing a bool literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushBool(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string text = token.Text;
            try {
                bool value = bool.Parse(text);
                this.push(loc, Literal.Bool(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a bool.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }
   
        /// <summary>This handles pushing a binary int literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushBin(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string text = token.Text;
            try {
                int value = S.Convert.ToInt32(text, 2);
                this.push(loc, Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a binary int.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing an ocatal int literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushOct(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string text = token.Text;
            try {
                int value = S.Convert.ToInt32(text, 8);
                this.push(loc, Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse an octal int.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a decimal int literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushInt(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string text = token.Text;
            try {
                int value = int.Parse(text);
                this.push(loc, Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a decimal int.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a hexadecimal int literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushHex(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string text = token.Text[2..];
            try {
                int value = int.Parse(text, NumberStyles.HexNumber);
                this.push(loc, Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a hex int.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a double literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushDouble(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string text = token.Text;
            try {
                double value = double.Parse(text);
                this.push(loc, Literal.Double(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a double.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a string literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushString(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string text = token.Text;
            // TODO: Need to handle decoding escaped sequences.
            this.push(loc, Literal.String(text));
        }

        /// <summary>This handles pushing a type onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushType(PP.ParseTree.PromptArgs args) {
            PP.Tokenizer.Token token = args.Tokens[^1];
            PP.Scanner.Location loc = token.End;
            string text = token.Text;
            Type t = Type.FromName(text);
            if (t is null)
                throw new Exception("Unrecognized type name.").
                    With("Text", text).
                    With("Location", loc);
            this.push(loc, t);
        }

        #endregion
    }
}
