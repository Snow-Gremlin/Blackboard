using Blackboard.Core;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Functions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using PP = PetiteParser;
using S = System;
using Blackboard.Parser.StackItems;
using Blackboard.Core.Data.Caps;

namespace Blackboard.Parser {

    /// <summary>This will parse the Blackboard language into actions and nodes to apply to the driver.</summary>
    public class Parser {

        /// <summary>The resource file for the Blackboard language definition.</summary>
        private const string resourceName = "Blackboard.Parser.Parser.lang";

        /// <summary>The Blackboard language base parser lazy singleton.</summary>
        static private PP.Parser.Parser BaseParser => ParserSingleton ??= PP.Loader.Loader.LoadParser(
            PP.Scanner.Default.FromResource(Assembly.GetExecutingAssembly(), resourceName));
        static private PP.Parser.Parser ParserSingleton;

        private Driver driver;
        private LinkedList<object> stack;
        private Dictionary<string, PP.ParseTree.PromptHandle> prompts;
        private Stack<Namespace> namescope;

        /// <summary>Creates a new Blackboard language parser.</summary>
        /// <param name="driver">The driver to modify.</param>
        public Parser(Driver driver) {
            this.driver  = driver;
            this.stack   = new LinkedList<object>();
            this.prompts = null;
            this.namescope = new Stack<Namespace>();
            this.namescope.Push(this.driver.Global);

            this.initPrompts();
        }

        /// <summary>Reads the given lines of input Blackline code.</summary>
        /// <param name="input">The input code to parse.</param>
        public void Read(params string[] input) =>
            this.Read(input as IEnumerable<string>);

        /// <summary>Reads the given lines of input Blackline code.</summary>
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

        #region Prompts Setup...

        /// <summary>Initializes the prompts and operators for this parser.</summary>
        private void initPrompts() {
            this.prompts = new Dictionary<string, PP.ParseTree.PromptHandle>() {
                { "Clean", this.handleClean },

                { "NewTypeInputNoAssign",   this.handleNewTypeInputNoAssign },
                { "NewTypeInputWithAssign", this.handleNewTypeInputWithAssign },
                { "NewVarInputWithAssign",  this.handleNewVarInputWithAssign },
                { "AssignExisting",         this.handleAssignExisting },

                { "StartDefine",            this.handleStartDefine },
                { "TypeDefine",             this.handleTypeDefine },
                { "VarDefine",              this.handleVarDefine },
                { "PullTrigger",            this.handlePullTrigger },
                { "ConditionalPullTrigger", this.handleConditionalPullTrigger },

                { "SetType",    this.handleSetType },
                { "StartCall",  this.handleStartCall },
                { "Call",       this.handleEndCall },
                { "PushId",     this.handlePushId },
                { "PushBool",   this.handlePushBool },
                { "PushInt",    this.handlePushInt },
                { "PushHex",    this.handlePushHex },
                { "PushDouble", this.handlePushDouble },
                { "StartId",    this.handleStartId },
                { "AddId",      this.handleAddId },
            };

            this.addProcess("Trinary",       3, "trinary");
            this.addProcess("Logical-Or",    2, "logicalOr");
            this.addProcess("Logical-Xor",   2, "logicalXor");
            this.addProcess("Logical-And",   2, "logicalAnd");
            this.addProcess("Or",            2, "or");
            this.addProcess("Xor",           2, "xor");
            this.addProcess("And",           2, "and");
            this.addProcess("Equal",         2, "equal");
            this.addProcess("Not-Equal",     2, "notEqual");
            this.addProcess("Greater",       2, "greater");
            this.addProcess("Less",          2, "less");
            this.addProcess("Greater-Equal", 2, "greaterEqual");
            this.addProcess("Less-Equal",    2, "lessEqual");
            this.addProcess("Shift-Right",   2, "shiftRight");
            this.addProcess("Shift-Left",    2, "shiftLeft");
            this.addProcess("Sum",           2, "sum");
            this.addProcess("Subtract",      2, "subtract");
            this.addProcess("Multiply",      2, "multiply");
            this.addProcess("Divide",        2, "divide");
            this.addProcess("Modulo",        2, "modulo");
            this.addProcess("Remainder",     2, "remainder");
            this.addProcess("Power",         2, "power");
            this.addProcess("Negate",        1, "negate");
            this.addProcess("Not",           1, "not");
            this.addProcess("Invert",        1, "invert");
        }

        /// <summary>This adds a prompt for an operator handler.</summary>
        /// <param name="name">The name of the prompt to add to.</param>
        /// <param name="count">The number of values to pop off the stack for this function.</param>
        /// <param name="opName">The name of the operator function in the "operators" namespace.</param>
        private void addProcess(string name, int count, string opName) {
            FuncGroup op = this.driver.Global.Find("operators", opName) as FuncGroup;
            this.prompts[name] = (PP.ParseTree.PromptArgs args) => {
                INode[] inputs = this.popNode(count);
                INode node = op.Build(inputs);
                
                if (node is null) {
                    PP.Scanner.Location loc = args.Tokens[^1].End;
                    throw new Exception("The operator can not be called with given input.").
                        With("Operation", opName).
                        With("Inputs", string.Join(", ", inputs.TypeNames())).
                        With("Location", loc.ToString());
                }

                this.push(inputs.IsConstant() ? node.ToLiteral() : node);
            };
        }

        #endregion
        #region Helpers...

        /// <summary>Pushes a new node or stack item onto the stack.</summary>
        /// <param name="value">The value to push.</param>
        private void push(object value) => this.stack.AddLast(value);

        /// <summary>Pops one or more values off the stack of nodes.</summary>
        /// <param name="count">The number of nodes to pop.</param>
        /// <returns>The popped nodes in the order oldest to newest.</returns>
        private INode[] popNode(int count = 1) {
            INode[] nodes = new INode[count];
            for (int i = 0; i < count; i++) {
                nodes[count-1-i] = this.stack.Last.Value as INode;
                this.stack.RemoveLast();
            }
            return nodes;
        }

        /// <summary>Pops off whatever is on the top of the stack.</summary>
        /// <returns>The value which was on top of the stack.</returns>
        private object pop() {
            object value = this.stack.Last.Value;
            this.stack.RemoveLast();
            return value;
        }

        /// <summary>Gets the scope upto the last name in the given identifier.</summary>
        /// <param name="id">The identifier to get the scope from.</param>
        /// <param name="createMissing">Indicates that if a group doesn't exist, it should be added.</param>
        /// <returns>The scope or null if that pass doesn't exist or exists not as a namespace group.</returns>
        private INamespace scope(Identifier id, bool createMissing) {
            INamespace scope = this.driver.Nodes;
            for (int i = 0; i < id.Count - 1; i++) {
                string name = id[i];
                INamed node = scope.Find(name);
                if (node is null) {
                    if (createMissing) scope = new Group(name, scope);
                    else return null;
                } else if (node is INamespace subscope) scope = subscope;
                else return null;
            }
            return scope;
        }

        /// <summary>Finds the node at the given id.</summary>
        /// <param name="id">The identifier to find the node for.</param>
        /// <returns>The found node or null if not found.</returns>
        private INode find(Identifier id) => this.scope(id, false)?.Find(id[^1]);
        
        /// <summary>Creates a new input value node or trigger.</summary>
        /// <param name="loc">The location this assignment was written at.</param>
        /// <param name="typeText">The type of the input node to create.</param>
        /// <param name="id">The identifier of the input node to create.</param>
        /// <param name="node">This is the value to initialize the input node with, may be null to use a default value.</param>
        private void createInputValue(PP.Scanner.Location loc, string typeText, Identifier id, INode node) {
            INamespace scope = this.scope(id, true);
            if (scope is null) throw new Exception("The group in " + id + " is not an identifier group "+
                    "so it can not be used as a scope for an assigned input variable at " + loc + ".");

            string name = id[^1];
            if (scope.Exists(name)) throw new Exception("Can not assign a new " + typeText+ " input. "+
                    "An identifier already exists by the name " + name + " in " + scope + " at " + loc + ".");
            
            INode _ =
                typeText == "bool" ?   new InputValue<bool>(  name, scope, Cast.AsBoolValue(   node)) :
                typeText == "int"?     new InputValue<int>(   name, scope, Cast.AsIntValue(    node)) :
                typeText == "double"?  new InputValue<double>(name, scope, Cast.AsDoubleValue( node)) :
                typeText == "trigger"? new InputTrigger(      name, scope, Cast.AsTriggerValue(node)) :
                throw new Exception("Unknown type: " + typeText);
        }

        /// <summary>Defines an output value node or a constant value.</summary>
        /// <param name="loc">The location this defition was written at.</param>
        /// <param name="typeText">The type of the value to define.</param>
        /// <param name="id">The identifier of the value to create.</param>
        /// <param name="node">The node to define the value with.</param>
        private void defineValue(PP.Scanner.Location loc, string typeText, Identifier id, INode node) {
            INamespace scope = this.scope(id, true);
            if (scope is null) throw new Exception("The top group in " + id + " is not an identifier group "+
                    "so it can not be used as a scope for a typed definition at " + loc + ".");

            string name = id[^1];
            if (scope.Exists(name)) throw new Exception("Can not define a new " + typeText+ " node. "+
                    "An identifier already exists by the name " + name + " in " + scope + " at " + loc + ".");

            // If right is constant or literal create a const node instead of an output node.
            if (Cast.IsConstant(node)) {
                INode _ =
                    typeText == "bool"   ? new Const<bool>(  name, scope, Cast.AsBoolValue(  node)) :
                    typeText == "int"    ? new Const<int>(   name, scope, Cast.AsIntValue(   node)) :
                    typeText == "double" ? new Const<double>(name, scope, Cast.AsDoubleValue(node)) :
                    throw new Exception("Unable to define " + id + " of constant type " + typeText +
                        " with " + Cast.TypeName(node) + " at " + id.Location + ".");
            } else {
                INode _ =
                    typeText == "bool"    ? new OutputValue<bool>(  Cast.As<IValue<bool>>(  node), name, scope) :
                    typeText == "int"     ? new OutputValue<int>(   Cast.As<IValue<int>>(   node), name, scope) :
                    typeText == "double"  ? new OutputValue<double>(Cast.As<IValue<double>>(node), name, scope) :
                    typeText == "trigger" ? new OutputTrigger(      Cast.As<ITrigger>(      node), name, scope) :
                    throw new Exception("Unable to define " + id + " of type " + typeText +
                        " with " + Cast.TypeName(node) + " at " + id.Location + ".");
            }
        }

        /// <summary>This will try to provoke a trigger.</summary>
        /// <param name="loc">The location this provoke was written at.</param>
        /// <param name="id">The identifier of the value to provoke.</param>
        private void provokeTrigger(PP.Scanner.Location loc, Identifier id) {
            INode node = this.find(id);
            if (node is null)
                throw new Exception("No trigger by the name " + id + " was found at " + loc + ".");
            if (node is not ITriggerInput trigger)
                throw new Exception("May only provoke an input trigger. " + id + " is not an input trigger at " + loc + ".");
            trigger.Trigger();
        }

        #endregion
        #region Handlers...

        /// <summary>This is called before each statement to prepare and clean up the parser.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleClean(PP.ParseTree.PromptArgs args) {
            args.Tokens.Clear();
            this.stack.Clear(); // Should be already clear.

            this.isDefine = false;
            this.typeText = undefinedType;
        }

        /// <summary>This creates a new input node of a specific type without assigning the value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleNewTypeInputNoAssign(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Identifier id = this.pop() as Identifier;
            this.createInputValue(loc, this.typeText, id, null);
        }

        /// <summary>This creates a new input node of a specific type and assigns it with an initial value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleNewTypeInputWithAssign(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop() as Identifier;
            this.createInputValue(loc, this.typeText, id, right);
        }

        /// <summary>This creates a new input node and assigns it with an initial value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleNewVarInputWithAssign(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop() as Identifier;
            this.createInputValue(loc, Cast.TypeName(right), id, right);
        }

        /// <summary>This assigns several existing input nodes with a new value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleAssignExisting(PP.ParseTree.PromptArgs args) {
            INode right = this.popNode().First();
            while (this.stack.Count > 0) {
                Identifier id = this.pop() as Identifier;
                INode left = this.find(id);
                if (left is null) throw new Exception("Unknown input variable " + id + " at " + id.Location + ".");
                else if (left is IValueInput<bool>   leftBool)    leftBool.   SetValue(Cast.AsBoolValue(   right));
                else if (left is IValueInput<int>    leftInt)     leftInt.    SetValue(Cast.AsIntValue(    right));
                else if (left is IValueInput<double> leftDouble)  leftDouble. SetValue(Cast.AsDoubleValue( right));
                else if (left is ITriggerInput       leftTrigger) leftTrigger.Trigger( Cast.AsTriggerValue(right));
                else throw new Exception("Unable to assign to " + Cast.TypeName(left) + " at " + id.Location + ".");
            }
        }

        private void handleStartDefine(PP.ParseTree.PromptArgs args) {
            this.isDefine = true;
        }

        /// <summary>This handles defining a new typed output node.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleTypeDefine(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop() as Identifier;
            this.defineValue(loc, this.typeText, id, right);
        }

        /// <summary>This handles defining a new untyped output node.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleVarDefine(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop() as Identifier;
            this.defineValue(loc, Cast.TypeName(right), id, right);
        }

        /// <summary>This handles when a trigger is provoked unconditionally.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePullTrigger(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Identifier id = this.pop() as Identifier;
            this.provokeTrigger(loc, id);
        }

        /// <summary>This handles when a trigger should only be provoked if a condition returns true.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleConditionalPullTrigger(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Identifier id = this.pop() as Identifier;
            INode node = this.popNode().First();
            if (node is not IValue<bool> boolNode)
                throw new Exception("May only conditionally provoke the trigger " + id + " with a bool or trigger at " + loc + ".");
            if (boolNode.Value) this.provokeTrigger(loc, id);
        }

        /// <summary>This handles setting the value type.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleSetType(PP.ParseTree.PromptArgs args) =>
            this.typeText = args.Tokens[^1].Text;

        /// <summary>This handles preparing for a method call.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleStartCall(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Identifier id = this.pop() as Identifier;
            this.stack.AddLast(new Call(id, loc));
        }

        /// <summary>This handles the end of a method call and creates the node for the method.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleEndCall(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Call call;
            LinkedList<INode> inputs = new();
            while (true) {
                object obj = this.stack.Last.Value;
                this.stack.RemoveLast();
                if (obj is INode node) inputs.AddFirst(node);
                else if (obj is Call callObj) {
                    call = callObj;
                    break;
                } else throw new Exception("Expected a node or a call but got " + obj + " at " + loc+ ".");
            }

            // NOTE: Currently functions can't be namespaced, part of types, and there are no classes yet,
            //       so simply do a lookup of the function in the dictionary of functions.
            string name = call.Identifier.ToString();
            INode funcNode = this.funcs.Build(name, inputs);
            this.push(Cast.IsConstant(inputs) ? Cast.ToLiteral(funcNode) : funcNode);
        }

        /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushId(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Identifier id = this.pop() as Identifier;
            INode node = this.driver.Find(id);
            if (node is null)
                throw new Exception("Identifier " + id + " at " + loc + " is unknown.");
            if (this.isDefine) this.push(node);
            else {
                INode literal = Cast.ToLiteral(node);
                if (literal is not null) this.push(literal);
                else throw new Exception("The identifier " + id + " can not be used in assignment at " + loc + ".");
            }
        }

        /// <summary>This handles pushing a bool literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushBool(PP.ParseTree.PromptArgs args) {
            string text = args.Tokens[^1].Text;
            try {
                bool value = bool.Parse(text);
                this.push(Literal.Bool(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \"" + text + "\" as a bool.", ex);
            }
        }

        /// <summary>This handles pushing an int literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushInt(PP.ParseTree.PromptArgs args) {
            string text = args.Tokens[^1].Text;
            try {
                int value = int.Parse(text);
                this.push(Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \""+text+"\" as a int.", ex);
            }
        }

        /// <summary>This handles pushing a hexadecimal int literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushHex(PP.ParseTree.PromptArgs args) {
            string text = args.Tokens[^1].Text[2..];
            try {
                int value = int.Parse(text, NumberStyles.HexNumber);
                this.push(Literal.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \""+text+"\" as a hex int.", ex);
            }
        }

        /// <summary>This handles pushing a double literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushDouble(PP.ParseTree.PromptArgs args) {
            string text = args.Tokens[^1].Text;
            try {
                double value = double.Parse(text);
                this.push(Literal.Double(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \""+text+"\" as a double.", ex);
            }
        }

        /// <summary>This handles an identifier item being added.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleStartId(PP.ParseTree.PromptArgs args) =>
            this.stack.AddLast(new Identifier(args.Tokens[^1].Start, args.Tokens[^1].Text ));

        /// <summary>This handles an identifier being added to the existing identifier.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleAddId(PP.ParseTree.PromptArgs args) =>
            (this.stack.Last.Value as Identifier).Add(args.Tokens[^1].Text);

        #endregion
    }
}
