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
        private Collection ops;
        private Collection funcs;
        private bool isLocal;

        /// <summary>Creates a new Blackboard language parser.</summary>
        /// <param name="driver">The driver to modify.</param>
        public Parser(Driver driver) {
            this.driver = driver;
            this.stack = new LinkedList<object>();
            this.prompts = null;
            this.ops = null;
            this.funcs = null;
            this.isLocal = false;
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
                { "Clean",       this.handleClean },
                { "StartAssign", this.handleStartAssign },
                { "StartDefine", this.handleStartDefine },
                { "StartConst",  this.handleStartConst },

                { "NewInput",             this.handleNewInput },
                { "EndAssignWithType",    this.handleEndAssignWithType },
                { "EndAssignWithoutType", this.handleEndAssignWithoutType },
                { "EndAssignExisting",    this.handleEndAssignExisting },
                { "EndDefineWithType",    this.handleEndDefineWithType },
                { "EndDefineWithoutType", this.handleEndDefineWithoutType },
                { "EndConstWithType",     this.handleEndConstWithType },
                { "EndConstWithoutType",  this.handleEndConstWithoutType },
                { "PullTrigger",          this.handlePullTrigger },

                { "Receiver",  this.handleReceiver },
                { "StartCall", this.handleStartCall },
                { "Call",      this.handleEndCall },
                { "PushId",    this.handlePushId },
                { "PushBool",  this.handlePushBool },
                { "PushInt",   this.handlePushInt },
                { "PushHex",   this.handlePushHex },
                { "PushFloat", this.handlePushFloat },
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
                INode[] inputs = this.pop(count);
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
            _ = new Const<double>("e",         this.driver.Nodes, S.Math.E);
            _ = new Const<double>("pi",        this.driver.Nodes, S.Math.PI);
            _ = new Const<double>("tau",       this.driver.Nodes, S.Math.Tau);
            _ = new Const<double>("sqrt2",     this.driver.Nodes, S.Math.Sqrt(2));
            _ = new Const<double>("nan",       this.driver.Nodes, double.NaN);
            _ = new Const<double>("inf",       this.driver.Nodes, double.PositiveInfinity);
            _ = new Const<double>("posInf",    this.driver.Nodes, double.PositiveInfinity);
            _ = new Const<double>("negInf",    this.driver.Nodes, double.NegativeInfinity);
            _ = new Const<double>("maxFloat",  this.driver.Nodes, double.MaxValue);
            _ = new Const<double>("minFloat",  this.driver.Nodes, double.MinValue);
            _ = new Const<double>("maxInt",    this.driver.Nodes, int.MaxValue);
            _ = new Const<double>("minInt",    this.driver.Nodes, int.MinValue);
            _ = new Const<double>("floatSize", this.driver.Nodes, sizeof(double));
            _ = new Const<double>("intSize",   this.driver.Nodes, sizeof(int));
        }

        /// <summary>Pushes a new node onto the stack of nodes.</summary>
        /// <param name="node">The node to push.</param>
        private void push(INode node) => this.stack.AddLast(node);

        /// <summary>Pops one or more values off the stack of nodes.</summary>
        /// <param name="count">The number of nodes to pop.</param>
        /// <returns>The popped nodes in the order oldest to newest.</returns>
        private INode[] pop(int count = 1) {
            INode[] nodes = new INode[count];
            for (int i = 0; i < count; i++) {
                nodes[count-1-i] = this.stack.Last.Value as INode;
                this.stack.RemoveLast();
            }
            return nodes;
        }

        /// <summary>Creates a new input variable from the given prompt arguments.</summary>
        /// <param name="args">Only the second and third values are used from the arguments as the type and id name respectivally.</param>
        /// <returns>The newly created node.</returns>
        private INode newInputNode(PP.ParseTree.PromptArgs args) {
            string type = args.Tokens[1].Text;
            string name = args.Tokens[2].Text;
            if (this.driver.Nodes.Exists(name))
                throw new Exception("Can not assign a new " + type+ " input. An identifier already exists by the name " + name + ".");
            
            // Create and return an input based on type.
            return type == "bool" ? new InputValue<bool>(  name, this.driver.Nodes) :
                type == "int"     ? new InputValue<int>(   name, this.driver.Nodes) :
                type == "float"   ? new InputValue<double>(name, this.driver.Nodes) :
                type == "trigger" ? new InputTrigger(      name, this.driver.Nodes) :
                throw new Exception("Unknown type: "+type);
        }

        /// <summary>Assigns the given node with the newest value on the stack.</summary>
        /// <param name="left">The node to assign.</param>
        private void assignNode(INode left) {
            INode right = this.pop().First();
            if (right is IValue<bool> rightBool) {
                bool value = rightBool.Value;
                if (left is InputValue<bool> leftBool) {
                    leftBool.SetValue(value);
                    return;
                }
            } else if (right is IValue<int> rightInt) {
                int value = rightInt.Value;
                if (left is InputValue<int> leftInt) {
                    leftInt.SetValue(value);
                    return;
                } else if (left is InputValue<double> leftFloat) {
                    leftFloat.SetValue(value);
                    return;
                }
            } else if (right is IValue<double> rightFloat) {
                double value = rightFloat.Value;
                if (left is InputValue<double> leftFloat) {
                    leftFloat.SetValue(value);
                    return;
                }
            } else if (right is ITrigger rightTrigger) {
                bool value = rightTrigger.Triggered;
                if (left is InputTrigger leftTrigger) {
                    leftTrigger.Trigger(value);
                    return;
                } else if (left is InputValue<bool> leftBool) {
                    leftBool.SetValue(value);
                    return;
                }
            }
            throw new Exception(Cast.PrettyName(right) + " can not be assigned to " + Cast.PrettyName(left) + ".");
        }

        #endregion
        #region Handlers...

        private void handleClean(PP.ParseTree.PromptArgs args) {
            args.Tokens.Clear();
            this.stack.Clear(); // Should be already clear.
        }

        private void handleStartAssign(PP.ParseTree.PromptArgs args) =>
            this.isLocal = true;

        private void handleStartDefine(PP.ParseTree.PromptArgs args) =>
            this.isLocal = false;

        private void handleStartConst(PP.ParseTree.PromptArgs args) =>
            this.isLocal = true;

        private void handleNewInput(PP.ParseTree.PromptArgs args) =>
            this.newInputNode(args);

        private void handleEndAssignWithType(PP.ParseTree.PromptArgs args) =>
            this.assignNode(this.newInputNode(args));
        
        private void handleEndAssignWithoutType(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[1].Text;
            if (this.driver.Nodes.Exists(name))
                throw new Exception("Can not assign input. An identifier already exists by the name " + name + ".");

            INode right = this.pop().First();
            if (right is IValue<bool> rightBool)
                _ = new InputValue<bool>(name, this.driver.Nodes, rightBool.Value);
            else if (right is IValue<int> rightInt)
                _ = new InputValue<int>(name, this.driver.Nodes, rightInt.Value);
            else if (right is IValue<double> rightFloat)
                _ = new InputValue<double>(name, this.driver.Nodes, rightFloat.Value);
            else if (right is ITrigger rightTrigger)
                new InputTrigger(name, this.driver.Nodes).Trigger(rightTrigger.Triggered);
            else throw new Exception(Cast.PrettyName(right) + " can not be assigned.");
        }

        private void handleEndAssignExisting(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[0].Text;
            if (!this.driver.Nodes.Exists(name))
                throw new Exception("Failed to find " + name + " to be assigned.");

            INode left = this.driver.Find(name);
            if (left is not IInput) throw new Exception("May only assign a value " +
                "directly to a input variable. " + name + " is not an input variable.");
            this.assignNode(left);
        }

        private void handleEndDefineWithType(PP.ParseTree.PromptArgs args) {
            string type = args.Tokens[0].Text;
            string name = args.Tokens[1].Text;
            if (this.driver.Nodes.Exists(name))
                throw new Exception("May not define a " + type + ". An identifier already exists by the name " + name + ".");

            INode right = this.pop().First();
            if (right is IValue<bool> rightBool) {
                if (type != "bool")
                    throw new Exception("May not define a "+type+" with a bool value.");
                _ = new OutputValue<bool>(rightBool, name, this.driver.Nodes);
            } else if (right is IValue<int> rightInt) {
                INode _ = type == "float" ?
                        new OutputValue<double>(new IntToFloat(rightInt), name, this.driver.Nodes) :
                    type == "int" ?
                        new OutputValue<int>(rightInt, name, this.driver.Nodes) :
                    throw new Exception("May not define a "+type+" with an int value.");
            } else if(right is IValue<double> rightFloat) {
                if (type != "float")
                    throw new Exception("May not define a "+type+" with a float value.");
                _ = new OutputValue<double>(rightFloat, name, this.driver.Nodes);
            } else if(right is ITrigger rightTrigger) {
                if (type != "trigger")
                    throw new Exception("May not define a "+type+" with a trigger value.");
                _ = new OutputTrigger(rightTrigger, name, this.driver.Nodes);
            } else throw new Exception(Cast.PrettyName(right) + " can not be used in a definition.");
        }

        private void handleEndDefineWithoutType(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[0].Text;
            if (this.driver.Nodes.Exists(name))
                throw new Exception("May not define an identifier. An identifier already exists by the name " + name + ".");

            INode right = this.pop().First();
            INode _ = right is IValue<bool> rightBool ?
                    new OutputValue<bool>(rightBool, name, this.driver.Nodes) :
                right is IValue<int> rightInt ?
                    new OutputValue<int>(rightInt, name, this.driver.Nodes) :
                right is IValue<double> rightFloat ?
                    new OutputValue<double>(rightFloat, name, this.driver.Nodes) :
                right is ITrigger rightTrigger ?
                    new OutputTrigger(rightTrigger, name, this.driver.Nodes) :
                throw new Exception(Cast.PrettyName(right) + " can not be used in a definition.");
        }

        private void handleEndConstWithType(PP.ParseTree.PromptArgs args) {
            string type = args.Tokens[1].Text;
            string name = args.Tokens[2].Text;
            if (this.driver.Nodes.Exists(name))
                throw new Exception("May not create " + type + " constant. An identifier already exists by the name " + name + ".");

            INode right = this.pop().First();
            if (type == "trigger")
                throw new Exception("May not define a constant trigger value.");
            else if (right is IValue<bool> rightBool) {
                if (type != "bool")
                    throw new Exception("May not define a "+type+" with a bool constant.");
                _ = new Const<bool>(name, this.driver.Nodes, rightBool.Value);
            } else if (right is IValue<int> rightInt) {
                INode _ = type == "float" ?
                        new Const<double>(name, this.driver.Nodes, rightInt.Value) :
                    type == "int" ?
                        new Const<int>(name, this.driver.Nodes, rightInt.Value) :
                    throw new Exception("May not define a "+type+" with an int constant.");
            } else if (right is IValue<double> rightFloat) {
                if (type != "float")
                    throw new Exception("May not define a "+type+" with a float constant.");
                _ = new Const<double>(name, this.driver.Nodes, rightFloat.Value);
            } else throw new Exception(Cast.PrettyName(right) + " can not be used in a " +
                "definition of a " + type + " constant by the name " + name + ".");
        }

        private void handleEndConstWithoutType(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[1].Text;
            if (this.driver.Nodes.Exists(name))
                throw new Exception("May not create constant. An identifier already exists by the name " + name + ".");

            INode right = this.pop().First();
            INode _ = right is IValue<bool> rightBool ?
                    new Const<bool>(name, this.driver.Nodes, rightBool.Value) :
                right is IValue<int> rightInt ?
                    new Const<int>(name, this.driver.Nodes, rightInt.Value) :
                right is IValue<double> rightFloat ?
                    new Const<double>(name, this.driver.Nodes, rightFloat.Value) :
                right is ITrigger ?
                    throw new Exception("Can not create a constant trigger.") :
                throw new Exception(Cast.PrettyName(right) + " can not be used in a " +
                    "definition of a constant by the name " + name + ".");
        }

        private void handlePullTrigger(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            if (!driver.Trigger(name))
                throw new Exception("May only provoke an input trigger. " + name + " is not an input trigger.");
        }

        private void handleReceiver(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            INode node = this.pop().First();
            if (node is INamespace scope) {
                INode child = scope.Find(name);
                if (child is not null) this.push(child);
                else throw new Exception("No child by the name "+name+" found in "+scope+".");
            } else throw new Exception("May not find a child in "+node+".");
        }
        
        private void handleStartCall(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            if (!this.funcs.ContainsKey(name))
                throw new Exception("No function by the name "+name+".");
            // Push non-INode onto the stack to mark the start of the function.
            this.stack.AddLast(name);
        }

        private void handleEndCall(PP.ParseTree.PromptArgs args) {
            string name;
            LinkedList<INode> inputs = new();
            while (true) {
                object obj = this.stack.Last.Value;
                this.stack.RemoveLast();
                if (obj is INode node) inputs.AddFirst(node);
                else {
                    name = obj as string;
                    break;
                }
            }

            INode funcNode = this.funcs.Build(name, inputs);
            this.push(Cast.IsConstant(inputs) ? Cast.ToLiteral(funcNode) : funcNode);
        }

        private void handlePushId(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            INode node = this.driver.Find(name);
            if (node is null)
                throw new Exception("Identifier " + name + " is unknown.");
            if (this.isLocal) {
                INode literal = Cast.ToLiteral(node);
                if (literal is not null) this.push(literal);
                else throw new Exception(name + " could not be used in assignment.");
            } else this.push(node);
        }

        private void handlePushBool(PP.ParseTree.PromptArgs args) {
            string text = args.Tokens[^1].Text;
            try {
                bool value = bool.Parse(text);
                this.push(new Literal<bool>(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \""+text+"\" as a bool.", ex);
            }
        }

        private void handlePushInt(PP.ParseTree.PromptArgs args) {
            string text = args.Tokens[^1].Text;
            try {
                int value = int.Parse(text);
                this.push(new Literal<int>(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \""+text+"\" as a int.", ex);
            }
        }

        private void handlePushHex(PP.ParseTree.PromptArgs args) {
            string text = args.Tokens[^1].Text[2..];
            try {
                int value = int.Parse(text, NumberStyles.HexNumber);
                this.push(new Literal<int>(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \""+text+"\" as a hex int.", ex);
            }
        }

        private void handlePushFloat(PP.ParseTree.PromptArgs args) {
            string text = args.Tokens[^1].Text;
            try {
                double value = double.Parse(text);
                this.push(new Literal<double>(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse \""+text+"\" as a float.", ex);
            }
        }

        #endregion
    }
}
