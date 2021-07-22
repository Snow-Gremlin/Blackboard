using Blackboard.Core;
using Blackboard.Core.Caps;
using Blackboard.Core.Interfaces;
using Blackboard.Parser.Functions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using PP = PetiteParser;
using S = System;
using Blackboard.Parser.StackItems;

namespace Blackboard.Parser {

    /// <summary>This will parse the Blackboard language into actions and nodes to apply to the driver.</summary>
    public class Parser {

        private const string undefinedType = "undefined";

        /// <summary>The resource file for the Blackboard language definition.</summary>
        private const string resourceName = "Blackboard.Parser.Parser.lang";

        /// <summary>The Blackboard language base parser lazy singleton.</summary>
        static private PP.Parser.Parser BaseParser => ParserSingleton ??= PP.Loader.Loader.LoadParser(
            PP.Scanner.Default.FromResource(Assembly.GetExecutingAssembly(), resourceName));
        static private PP.Parser.Parser ParserSingleton;

        private Driver driver;

        private bool isDefine;
        private string typeText;

        private LinkedList<object> stack;
        private Dictionary<string, PP.ParseTree.PromptHandle> prompts;
        private Collection ops;
        private Collection funcs;

        /// <summary>Creates a new Blackboard language parser.</summary>
        /// <param name="driver">The driver to modify.</param>
        public Parser(Driver driver) {
            this.driver = driver;

            this.isDefine = false;
            this.typeText = undefinedType;

            this.stack   = new LinkedList<object>();
            this.prompts = null;
            this.ops     = null;
            this.funcs   = null;

            this.initPrompts();
            this.initFuncs();
            this.initConsts();
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

        #region Helpers...

        /// <summary>Initializes the prompts and operators for this parser.</summary>
        private void initPrompts() {
            this.ops = new Collection();
            this.prompts = new Dictionary<string, PP.ParseTree.PromptHandle>() {
                { "Clean", this.handleClean },

                { "NewTypeInputNoAssign",   this.handleNewTypeInputNoAssign },
                { "NewTypeInputWithAssign", this.handleNewTypeInputWithAssign },
                { "NewVarInputWithAssign",  this.handleNewVarInputWithAssign },
                { "AssignExisting",         this.handleAssignExisting },

                { "TypeDefine",             this.handleTypeDefine },
                { "VarDefine",              this.handleVarDefine },
                { "PullTrigger",            this.handlePullTrigger },
                { "ConditionalPullTrigger", this.handleConditionalPullTrigger },

                { "SetType",   this.handleSetType },
                { "StartCall", this.handleStartCall },
                { "Call",      this.handleEndCall },
                { "PushId",    this.handlePushId },
                { "PushBool",  this.handlePushBool },
                { "PushInt",   this.handlePushInt },
                { "PushHex",   this.handlePushHex },
                { "PushFloat", this.handlePushFloat },
                { "StartId",   this.handleStartId },
                { "AddId",     this.handleAddId },
            };
            this.addProcess("Trinary", 3,
                new Input3<IValue<bool>, IValue<bool>,   IValue<bool>>(  (test, left, right) => new Select<bool>(  test, left, right)),
                new Input3<IValue<bool>, IValue<int>,    IValue<int>>(   (test, left, right) => new Select<int>(   test, left, right)),
                new Input3<IValue<bool>, IValue<double>, IValue<double>>((test, left, right) => new Select<double>(test, left, right)));
            this.addProcess("Logical-Or", 2,
                new Input2<IValue<bool>, IValue<bool>>((left, right) => new Or( left, right)),
                new Input2<ITrigger,     ITrigger>(    (left, right) => new Any(left, right)));
            this.addProcess("Logical-Xor", 2,
                new Input2<IValue<bool>, IValue<bool>>((left, right) => new Xor(    left, right)),
                new Input2<ITrigger,     ITrigger>(    (left, right) => new OnlyOne(left, right)));
            this.addProcess("Logical-And", 2,
                new Input2<IValue<bool>, IValue<bool>>((left, right) => new And(left, right)),
                new Input2<ITrigger,     ITrigger>(    (left, right) => new All(left, right)));
            this.addProcess("Or", 2,
                new Input2<IValue<bool>, IValue<bool>>((left, right) => new Or(       left, right)),
                new Input2<IValue<int>,  IValue<int>>( (left, right) => new BitwiseOr(left, right)),
                new Input2<ITrigger,     ITrigger>(    (left, right) => new Any(      left, right)));
            this.addProcess("Xor", 2,
                new Input2<IValue<bool>, IValue<bool>>((left, right) => new Xor(       left, right)),
                new Input2<IValue<int>,  IValue<int>>( (left, right) => new BitwiseXor(left, right)),
                new Input2<ITrigger,     ITrigger>(    (left, right) => new OnlyOne(   left, right)));
            this.addProcess("And", 2,
                new Input2<IValue<bool>, IValue<bool>>((left, right) => new And(       left, right)),
                new Input2<IValue<int>,  IValue<int>>( (left, right) => new BitwiseAnd(left, right)),
                new Input2<ITrigger,     ITrigger>(    (left, right) => new All(       left, right)));
            this.addProcess("Equal", 2,
                new Input2<IValue<bool>,   IValue<bool>>(  (left, right) => new Equal<bool>(  left, right)),
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new Equal<int>(   left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new Equal<double>(left, right)));
            this.addProcess("Not-Equal", 2,
                new Input2<IValue<bool>,   IValue<bool>>(  (left, right) => new NotEqual<bool>(  left, right)),
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new NotEqual<int>(   left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new NotEqual<double>(left, right)));
            this.addProcess("Greater", 2,
                new Input2<IValue<bool>,   IValue<bool>>(  (left, right) => new GreaterThan<bool>(  left, right)),
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new GreaterThan<int>(   left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new GreaterThan<double>(left, right)));
            this.addProcess("Less", 2,
                new Input2<IValue<bool>,   IValue<bool>>(  (left, right) => new LessThan<bool>(  left, right)),
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new LessThan<int>(   left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new LessThan<double>(left, right)));
            this.addProcess("Greater-Equal", 2,
                new Input2<IValue<bool>,   IValue<bool>>(  (left, right) => new GreaterThanOrEqual<bool>(  left, right)),
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new GreaterThanOrEqual<int>(   left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new GreaterThanOrEqual<double>(left, right)));
            this.addProcess("Less-Equal", 2,
                new Input2<IValue<bool>,   IValue<bool>>(  (left, right) => new LessThanOrEqual<bool>(  left, right)),
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new LessThanOrEqual<int>(   left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new LessThanOrEqual<double>(left, right)));
            this.addProcess("Shift-Right", 2,
                new Input2<IValue<int>, IValue<int>>((left, right) => new RightShift(left, right)));
            this.addProcess("Shift-Left", 2,
                new Input2<IValue<int>, IValue<int>>((left, right) => new LeftShift(left, right)));
            this.addProcess("Sum", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new SumInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new SumFloat(left, right)));
            this.addProcess("Subtract", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new SubInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new SubFloat(left, right)));
            this.addProcess("Multiply", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new MulInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new MulFloat(left, right)));
            this.addProcess("Divide", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new DivInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new DivFloat(left, right)));
            this.addProcess("Modulo", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new ModInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new ModFloat(left, right)));
            this.addProcess("Remainder", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new RemInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new RemFloat(left, right)));
            this.addProcess("Power", 2,
                new Input2<IValue<int>,    IValue<int>>(   (left, right) => new PowerInt(  left, right)),
                new Input2<IValue<double>, IValue<double>>((left, right) => new PowerFloat(left, right)));
            this.addProcess("Negate", 1,
                new Input1<IValue<int>>(   (input) => new NegInt(input)),
                new Input1<IValue<double>>((input) => new NegFloat(input)));
            this.addProcess("Not", 1,
                new Input1<IValue<bool>>((input) => new Not(input)));
            this.addProcess("Invert", 1,
                new Input1<IValue<int>>((input) => new BitwiseNot(input)));
        }

        /// <summary>This adds a prompt and operator handler methods.</summary>
        /// <param name="name">The name of the prompt to add to.</param>
        /// <param name="count">The number of values to pop off the stack for this function.</param>
        /// <param name="funcs">The functions that will handle this prompt based on types of values popped off the stack.</param>
        private void addProcess(string name, int count, params IFunction[] funcs) {
            this.prompts[name] = (PP.ParseTree.PromptArgs args) => {
                INode[] inputs = this.popNode(count);
                PP.Scanner.Location loc = args.Tokens[^1].End;
                INode node = this.ops.Build(name, inputs, loc);
                this.push(Cast.IsConstant(inputs) ? Cast.ToLiteral(node) : node);
            };
            this.ops.Add(name, funcs);
        }

        /// <summary>Prepares all the functions that can be called.</summary>
        private void initFuncs() {
            this.funcs = new Collection().
                Add("abs",
                    new Input1<IValue<int>>(   (input) => new AbsInt(  input)),
                    new Input1<IValue<double>>((input) => new AbsFloat(input))).
                Add("all",
                    new InputN<ITrigger>((inputs) => new All(inputs))).
                Add("and",
                    new InputN<IValue<bool>>((inputs) => new And(inputs))).
                Add("any",
                    new InputN<ITrigger>((inputs) => new Any(inputs))).
                Add("clamp",
                    new Input3<IValue<int>,    IValue<int>,    IValue<int>>(   (input1, input2, input3) => new Clamp<int>(   input1, input2, input3)),
                    new Input3<IValue<double>, IValue<double>, IValue<double>>((input1, input2, input3) => new Clamp<double>(input1, input2, input3))).
                Add("float",
                    new Input1<IValue<int>>((input) => new IntToFloat(input))).
                Add("int",
                    new Input1<IValue<double>>((input) => new Truncate(input))).
                Add("implies",
                    new Input2<IValue<bool>, IValue<bool>>((input1, input2) => new Implies(input1, input2))).
                Add("latch",
                    new Input2<ITrigger, IValue<bool>>(  (input1, input2) => new Latch<bool>(  input1, input2)),
                    new Input2<ITrigger, IValue<int>>(   (input1, input2) => new Latch<int>(   input1, input2)),
                    new Input2<ITrigger, IValue<double>>((input1, input2) => new Latch<double>(input1, input2))).
                Add("lerp",
                    new Input3<IValue<double>, IValue<double>, IValue<double>>((input1, input2, input3) => new Lerp(input1, input2, input3))).
                Add("max",
                    new InputN<IValue<int>>(   (inputs) => new Max<int>(   inputs)),
                    new InputN<IValue<double>>((inputs) => new Max<double>(inputs))).
                Add("min",
                    new InputN<IValue<int>>(   (inputs) => new Min<int>(   inputs)),
                    new InputN<IValue<double>>((inputs) => new Min<double>(inputs))).
                Add("mul",
                    new InputN<IValue<int>>(   (inputs) => new MulInt(  inputs)),
                    new InputN<IValue<double>>((inputs) => new MulFloat(inputs))).
                Add("on",
                    new Input1<IValue<bool>>((input) => new OnTrue(input))).
                Add("onChange",
                    new InputN<INode>((inputs) => new OnChange(inputs))).
                Add("onFalse",
                    new Input1<IValue<bool>>((input) => new OnFalse(input))).
                Add("onlyOne",
                    new InputN<ITrigger>((inputs) => new OnlyOne(inputs))).
                Add("onTrue",
                    new Input1<IValue<bool>>((input) => new OnTrue(input))).
                Add("or",
                    new InputN<IValue<bool>>((inputs) => new Or(inputs))).
                Add("round",
                    new Input1<IValue<double>>((input) => new Round(input))).
                Add("trunc",
                    new Input1<IValue<double>>((input) => new Truncate(input)));
        }

        /// <summary>Adds in initial constants.</summary>
        private void initConsts() {
            INamespace scope = this.driver.Nodes;
            _ = new Const<double>("e",         scope, S.Math.E);
            _ = new Const<double>("pi",        scope, S.Math.PI);
            _ = new Const<double>("tau",       scope, S.Math.Tau);
            _ = new Const<double>("sqrt2",     scope, S.Math.Sqrt(2));
            _ = new Const<double>("nan",       scope, double.NaN);
            _ = new Const<double>("inf",       scope, double.PositiveInfinity);
            _ = new Const<double>("posInf",    scope, double.PositiveInfinity);
            _ = new Const<double>("negInf",    scope, double.NegativeInfinity);
            _ = new Const<double>("maxFloat",  scope, double.MaxValue);
            _ = new Const<double>("minFloat",  scope, double.MinValue);
            _ = new Const<double>("maxInt",    scope, int.MaxValue);
            _ = new Const<double>("minInt",    scope, int.MinValue);
            _ = new Const<double>("floatSize", scope, sizeof(double));
            _ = new Const<double>("intSize",   scope, sizeof(int));
        }

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
        private INode find(Identifier id) =>
            this.scope(id, false)?.Find(id[^1]);

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
            INamespace scope = this.scope(id, true);
            if (scope is null) throw new Exception("The group in " + id + " is not an identifier group "+
                    "so it can not be used as a scope for an input variable at " + loc + ".");

            string name = id[^1];
            if (scope.Exists(name)) throw new Exception("Can not create a new " + this.typeText+ " input. "+
                    "An identifier already exists by the name " + name + " in " + scope + " at " + loc + ".");

            INode _ =
                this.typeText == "bool"    ? new InputValue<bool>(  name, scope) :
                this.typeText == "int"     ? new InputValue<int>(   name, scope) :
                this.typeText == "float"   ? new InputValue<double>(name, scope) :
                this.typeText == "trigger" ? new InputTrigger(      name, scope) :
                // The following shouldn't happen since parser language checks type,
                // unless the parser language is updated and not this method.
                throw new Exception("Unknown type: " + this.typeText);
        }

        /// <summary>This creates a new input node of a specific type and assigns it with an initial value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleNewTypeInputWithAssign(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop() as Identifier;
            INamespace scope = this.scope(id, true);
            if (scope is null) throw new Exception("The group in " + id + " is not an identifier group "+
                    "so it can not be used as a scope for an assigned input variable at " + loc + ".");

            string name = id[^1];
            if (scope.Exists(name)) throw new Exception("Can not assign a new " + this.typeText+ " input. "+
                    "An identifier already exists by the name " + name + " in " + scope + " at " + loc + ".");
            
            if (this.typeText == "bool") {
                bool value =
                    (right is IValue<bool> rightBool) ? rightBool.Value :
                    throw new Exception("Can not assign a " + Cast.PrettyName(right) + " to a bool.");
                _ = new InputValue<bool>(name, scope, value);
            } else if (this.typeText == "int") {
                int value =
                    (right is IValue<int> rightInt) ? rightInt.Value :
                    throw new Exception("Can not assign a " + Cast.PrettyName(right) + " to an int.");
                _ = new InputValue<int>(name, scope, value);
            } else if (this.typeText == "float") {
                double value =
                    (right is IValue<double> rightFloat) ? rightFloat.Value :
                    (right is IValue<int>    rightInt)   ? rightInt.Value :
                    throw new Exception("Can not assign a " + Cast.PrettyName(right) + " to a float.");
                _ = new InputValue<double>(name, scope, value);
            } else if (this.typeText == "trigger") {
                bool value =
                    (right is ITrigger     rightTrigger) ? rightTrigger.Provoked :
                    (right is IValue<bool> rightBool)    ? rightBool.Value :
                    throw new Exception("Can not assign a " + Cast.PrettyName(right) + " to a trigger.");
                new InputTrigger(name, scope).Trigger(value);
            } else {
                // The following shouldn't happen since parser language checks type,
                // unless the parser language is updated and not this method.
                throw new Exception("Unknown type: " + this.typeText);
            }
        }

        /// <summary>This creates a new input node and assigns it with an initial value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleNewVarInputWithAssign(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop() as Identifier;
            INamespace scope = this.scope(id, true);
            if (scope is null) throw new Exception("The group in " + id + " is not an identifier group "+
                    "so it can not be used as a scope for an untyped input variable at " + loc + ".");

            string name = id[^1];
            if (scope.Exists(name)) throw new Exception("Can not assign a new untyped " + this.typeText+ " input. "+
                    "An identifier already exists by the name " + name + " in " + scope + " at " + loc + ".");

            INode _ =
                right is IValue<bool>   rightBool    ? new InputValue<bool>(  name, scope, rightBool.Value) :
                right is IValue<int>    rightInt     ? new InputValue<int>(   name, scope, rightInt.Value) :
                right is IValue<double> rightFloat   ? new InputValue<double>(name, scope, rightFloat.Value) :
                right is ITrigger       rightTrigger ? new InputTrigger(      name, scope, rightTrigger.Provoked):
                throw new Exception(Cast.PrettyName(right) + " can not be assigned.");
        }

        /// <summary>This assigns several existing input nodes with a new value.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleAssignExisting(PP.ParseTree.PromptArgs args) {
            INode right = this.popNode().First();
            while (this.stack.Count > 0) {
                Identifier id = this.pop() as Identifier;
                INode left = this.find(id);
                if (left is null) throw new Exception("Unknown input variable " + id + " at " + id.Location + ".");
                if (left is IValueInput<bool> leftBool) {
                    bool value =
                        (right is IValue<bool> rightBool) ? rightBool.Value :
                        throw new Exception("Can not assign a " + Cast.PrettyName(right) + " to a bool.");
                    leftBool.SetValue(value);
                } else if (left is IValueInput<int> leftInt) {
                    int value =
                        (right is IValue<int> rightInt) ? rightInt.Value :
                        throw new Exception("Can not assign a " + Cast.PrettyName(right) + " to an int.");
                    leftInt.SetValue(value);
                } else if (left is IValueInput<double> leftFloat) {
                    double value =
                        (right is IValue<double> rightFloat) ? rightFloat.Value :
                        (right is IValue<int>    rightInt)   ? rightInt.Value :
                        throw new Exception("Can not assign a " + Cast.PrettyName(right) + " to a float.");
                    leftFloat.SetValue(value);
                } else if (left is ITriggerInput leftTrigger) {
                    bool value =
                        (right is ITrigger     rightTrigger) ? rightTrigger.Provoked :
                        (right is IValue<bool> rightBool)    ? rightBool.Value :
                        throw new Exception("Can not assign a " + Cast.PrettyName(right) + " to a trigger.");
                    leftTrigger.Trigger(value);
                } else throw new Exception("Unable to assign to " + Cast.PrettyName(left) + " at " + id.Location + ".");
            }
        }

        /// <summary>This handles defining a new typed output node.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleTypeDefine(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop() as Identifier;
            INamespace scope = this.scope(id, true);
            if (scope is null) throw new Exception("The group in " + id + " is not an identifier group "+
                    "so it can not be used as a scope for a typed definition at " + loc + ".");

            string name = id[^1];
            if (scope.Exists(name)) throw new Exception("Can not define a new typed " + this.typeText+ " node. "+
                    "An identifier already exists by the name " + name + " in " + scope + " at " + loc + ".");

            INode _ =
                this.typeText == "bool"    ? new OutputValue<bool>(  Cast.As<IValue<bool>>(right),   name, scope) :
                this.typeText == "int"     ? new OutputValue<int>(   Cast.As<IValue<int>>(right),    name, scope) :
                this.typeText == "float"   ? new OutputValue<double>(Cast.As<IValue<double>>(right), name, scope) :
                this.typeText == "trigger" ? new OutputTrigger(      Cast.As<ITrigger>(right),       name, scope) :
                throw new Exception("Unable to define " + id + " of type " + this.typeText + " with " + Cast.PrettyName(right) + " at " + id.Location + ".");
        }

        /// <summary>This handles defining a new untyped output node.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleVarDefine(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            INode right = this.popNode().First();
            Identifier id = this.pop() as Identifier;
            INamespace scope = this.scope(id, true);
            if (scope is null) throw new Exception("The group in " + id + " is not an identifier group "+
                    "so it can not be used as a scope for a typed definition at " + loc + ".");

            string name = id[^1];
            if (scope.Exists(name)) throw new Exception("Can not define a new typed " + this.typeText+ " node. "+
                    "An identifier already exists by the name " + name + " in " + scope + " at " + loc + ".");
            
            INode _ =
                right is IValue<bool>   rightBool    ? new OutputValue<bool>(  rightBool,    name, scope) :
                right is IValue<int>    rightInt     ? new OutputValue<int>(   rightInt,     name, scope) :
                right is IValue<double> rightFloat   ? new OutputValue<double>(rightFloat,   name, scope) :
                right is ITrigger       rightTrigger ? new OutputTrigger(      rightTrigger, name, scope):
                throw new Exception(Cast.PrettyName(right) + " can not be assigned.");
        }

        /// <summary>This handles when a trigger is provoked unconditionally.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePullTrigger(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Identifier id = this.pop() as Identifier;
            INode node = this.find(id);
            if (node is null)
                throw new Exception("No trigger by the name " + id + " was found at " + loc + ".");
            if (node is not ITriggerInput trigger)
                throw new Exception("May only provoke an input trigger. " + id + " is not an input trigger at " + loc + ".");
            trigger.Trigger();
        }

        /// <summary>This handles when a trigger should only be provoked if a condition returns true.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handleConditionalPullTrigger(PP.ParseTree.PromptArgs args) {
            PP.Scanner.Location loc = args.Tokens[^1].End;
            Identifier id = this.pop() as Identifier;
            INode node = this.popNode().First();
            if (node is not IValue<bool> boolNode)
                throw new Exception("May only conditionally provoke the trigger " + id + " with a bool or trigger at " + loc + ".");

            if (boolNode.Value) {
                if (node is null)
                    throw new Exception("No trigger by the name " + id + " was found at " + loc + ".");
                if (node is not ITriggerInput trigger)
                    throw new Exception("May only provoke an input trigger. " + id + " is not an input trigger at " + loc + ".");
                trigger.Trigger();
            }
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
                this.push(new Literal<bool>(value));
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
                this.push(new Literal<int>(value));
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
                this.push(new Literal<int>(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \""+text+"\" as a hex int.", ex);
            }
        }

        /// <summary>This handles pushing a float literal value onto the stack.</summary>
        /// <param name="args">The token information from the parser.</param>
        private void handlePushFloat(PP.ParseTree.PromptArgs args) {
            string text = args.Tokens[^1].Text;
            try {
                double value = double.Parse(text);
                this.push(new Literal<double>(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \""+text+"\" as a float.", ex);
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
