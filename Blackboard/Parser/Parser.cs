using Blackboard.Core;
using Blackboard.Core.Caps;
using Blackboard.Core.Interfaces;
using Blackboard.Parser.Functions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PP = PetiteParser;

namespace Blackboard.Parser {
    public class Parser {

        private const string resourceName = "Blackboard.Parser.Parser.lang";
        static private PP.Parser.Parser ParserSingleton;
        static private PP.Parser.Parser BaseParser => ParserSingleton ??= PP.Loader.Loader.LoadParser(
            PP.Scanner.Default.FromResource(Assembly.GetExecutingAssembly(), resourceName));

        private Driver driver;
        private LinkedList<INode> stack;
        private Dictionary<string, PP.ParseTree.PromptHandle> prompts;
        private Collection ops;
        private bool isAssign;

        public Parser(Driver driver) {
            this.driver = driver;
            this.stack = new LinkedList<INode>();
            this.prompts = null;
            this.ops = null;
            this.isAssign = false;
            this.initPrompts();
            this.initOps();
        }

        private void initPrompts() {
            this.prompts = new Dictionary<string, PP.ParseTree.PromptHandle>() {
                { "Clean",             this.handleClean },
                { "NewInput",          this.handleNewInput },
                { "LookupInput",       this.handleLookupInput },
                { "StartAssign",       this.handleStartAssign },
                { "EndAssign",         this.handleEndAssign },
                { "StartDefine",       this.handleStartDefine },
                { "EndDefine",         this.handleEndDefine },
                { "EndDefineWithType", this.handleEndDefineWithType },
                { "PullTrigger",       this.handlePullTrigger },
                { "StartCall",         this.handleStartCall },
                { "Call",              this.handleEndCall },
                { "PushId",            this.handlePushId },
                { "PushBool",          this.handlePushBool },
                { "PushInt",           this.handlePushInt },
                { "PushFloat",         this.handlePushFloat },
            };
            this.addProcess("Sum", 2);
            this.addProcess("Subtract", 2);
            this.addProcess("Or", 2);
            this.addProcess("Multiply", 2);
            this.addProcess("Divide", 2);
            this.addProcess("Modulo", 2);
            this.addProcess("And", 2);
            this.addProcess("Xor", 2);
            this.addProcess("Negate", 1);
            this.addProcess("Not", 1);
            this.addProcess("Invert", 1);
            this.addProcess("Trinary", 3);
        }

        private void initOps() {
            this.ops = new Collection {
                { "Sum",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new SumInt(  left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new SumFloat(left, right)) },
                { "Subtract",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new SubInt(  left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new SubFloat(left, right)) },
                { "Or",
                    new Input2<IValue<bool>, IValue<bool>>((left, right) => new Or(       left, right)),
                    new Input2<IValue<int>,  IValue<int>>( (left, right) => new BitwiseOr(left, right)),
                    new Input2<ITrigger,     ITrigger>(    (left, right) => new Any(      left, right)) },
                { "Multiply",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new MulInt(  left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new MulFloat(left, right)) },
                { "Divide",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new DivInt(  left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new DivFloat(left, right)) },
                { "Modulo",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new ModInt(  left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new ModFloat(left, right)) },
                { "And",
                    new Input2<IValue<bool>, IValue<bool>>((left, right) => new And(       left, right)),
                    new Input2<IValue<int>,  IValue<int>>( (left, right) => new BitwiseAnd(left, right)),
                    new Input2<ITrigger,     ITrigger>(    (left, right) => new All(       left, right)) },
                { "Xor",
                    new Input2<IValue<bool>, IValue<bool>>((left, right) => new Xor(       left, right)),
                    new Input2<IValue<int>,  IValue<int>>( (left, right) => new BitwiseXor(left, right)),
                    new Input2<ITrigger,     ITrigger>(    (left, right) => new OnlyOne(   left, right)) },
                { "Negate",
                    new Input1<IValue<int>>(   (input) => new NegInt(input)),
                    new Input1<IValue<double>>((input) => new NegFloat(input)) },
                { "Not",
                    new Input1<IValue<bool>>((input) => new Not(input)) },
                { "Invert",
                    new Input1<IValue<int>>((input) => new BitwiseNot(input)) },
                { "Trinary",
                    new Input3<IValue<bool>, IValue<bool>,   IValue<bool>>(  (test, left, right) => new Select<bool>(  test, left, right)),
                    new Input3<IValue<bool>, IValue<int>,    IValue<int>>(   (test, left, right) => new Select<int>(   test, left, right)),
                    new Input3<IValue<bool>, IValue<double>, IValue<double>>((test, left, right) => new Select<double>(test, left, right)) }
            };
        }

        public void Read(params string[] input) =>
            this.Read(input as IEnumerable<string>);

        public void Read(IEnumerable<string> input) {
            PP.Parser.Result result = BaseParser.Parse(input);
            if (result.Errors.Length > 0)
                throw new Exception(string.Join('\n', result.Errors));
            this.read(result.Tree);
        }

        private void read(PP.ParseTree.ITreeNode node) =>
            node.Process(this.prompts);

        private void push(INode node) => this.stack.AddLast(node);

        private IEnumerable<INode> pop(int count = 1) {
            INode[] nodes = new INode[count];
            for (int i = 0; i < count; i++) {
                nodes[count-1-i] = this.stack.Last.Value;
                this.stack.RemoveLast();
            }
            return nodes;
        }

        private void addProcess(string name, int count) =>
            this.prompts[name] = (PP.ParseTree.PromptArgs args) => this.push(this.ops.Build(name, this.pop(count)));

        #region Handlers...

        private void handleClean(PP.ParseTree.PromptArgs args) =>
            args.Tokens.Clear();

        private void handleNewInput(PP.ParseTree.PromptArgs args) {
            string type = args.Tokens[^2].Text;
            string id   = args.Tokens[^1].Text;
            if      (type == "bool")    this.push(new InputValue<bool>(  id, this.driver.Nodes));
            else if (type == "int")     this.push(new InputValue<int>(   id, this.driver.Nodes));
            else if (type == "float")   this.push(new InputValue<double>(id, this.driver.Nodes));
            else if (type == "trigger") this.push(new InputTrigger(      id, this.driver.Nodes));
            else throw new Exception("Unknown type: "+type);
        }

        private void handleLookupInput(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            INode node = this.driver.Find(name);
            if (node is not IInput)
                throw new Exception("May only assign a value directly " +
                    "to a input variable. " + name + " is not an input variable.");
            this.push(node);
        }

        private void handleStartAssign(PP.ParseTree.PromptArgs args) =>
            this.isAssign = true;

        private void handleEndAssign(PP.ParseTree.PromptArgs args) {
            INode[] nodes = this.pop(2).ToArray();
            INode left = nodes[0], right = nodes[1];
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

        private void handleStartDefine(PP.ParseTree.PromptArgs args) =>
            this.isAssign = false;

        private void handleEndDefine(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[0].Text;
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

        private void handleEndDefineWithType(PP.ParseTree.PromptArgs args) {
            string type = args.Tokens[0].Text;
            string name = args.Tokens[1].Text;
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

        private void handlePullTrigger(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            if (!driver.Trigger(name))
                throw new Exception("May only provoke an input trigger. " + name + " is not an input trigger.");
        }

        private void handleStartCall(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleEndCall(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handlePushId(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            INode node = this.driver.Find(name);
            if (node is null)
                throw new Exception(name + " is unknown");
            if (this.isAssign) {
                if      (node is IValue<bool> nodeBool)    this.push(new Literal<bool>(nodeBool.Value));
                else if (node is IValue<int> nodeInt)      this.push(new Literal<int>(nodeInt.Value));
                else if (node is IValue<double> nodeFloat) this.push(new Literal<double>(nodeFloat.Value));
                else if (node is ITrigger nodeTrigger)     this.push(new Literal<bool>(nodeTrigger.Triggered));
                else throw new Exception(name + " could not be used in assignment.");
            } else this.push(node);
        }

        private void handlePushBool(PP.ParseTree.PromptArgs args) {
            bool value = bool.Parse(args.Tokens[^1].Text);
            this.push(new Literal<bool>(value));
        }

        private void handlePushInt(PP.ParseTree.PromptArgs args) {
            int value = int.Parse(args.Tokens[^1].Text);
            this.push(new Literal<int>(value));
        }

        private void handlePushFloat(PP.ParseTree.PromptArgs args) {
            double value = double.Parse(args.Tokens[^1].Text);
            this.push(new Literal<double>(value));
        }

        #endregion
    }
}
