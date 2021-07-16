using System.Collections.Generic;
using System.Text;
using System.Linq;
using Blackboard.Parser.Functions;
using Blackboard.Core;
using Blackboard.Core.Bases;
using Blackboard.Core.Caps;
using Blackboard.Core.Interfaces;
using PP = PetiteParser;

namespace Blackboard.Parser {
    public class Parser {

        static private PP.Parser.Parser ParserSingleton;
        static private PP.Parser.Parser BaseParser =>
            ParserSingleton ??= PP.Loader.Loader.LoadParser(Encoding.UTF8.GetString(Properties.Resources.Parser));

        private Driver driver;
        private LinkedList<INode> stack;
        private Dictionary<string, PP.ParseTree.PromptHandle> prompts;
        private Collection ops;

        public Parser(Driver driver) {
            this.driver = driver;
            this.stack = new LinkedList<INode>();
            this.prompts = null;
            this.ops = null;
            this.initPrompts();
            this.initOps();
        }

        private void initPrompts() {
            this.prompts = new Dictionary<string, PP.ParseTree.PromptHandle>() {
                { "Clean",          this.handleClean },
                { "NewInput",       this.handleNewInput },
                { "LookupInput",    this.handleLookupInput },
                { "Assign",         this.handleAssign },
                { "PullTrigger",    this.handlePullTrigger },
                { "Define",         this.handleDefine },
                { "DefineWithType", this.handleDefineWithType },
                { "Trinary",        this.handleTrinary },
                { "StartCall",      this.handleStartCall },
                { "Call",           this.handleCall },
                { "PushId",         this.handlePushId },
                { "PushBool",       this.handlePushBool },
                { "PushInt",        this.handlePushInt },
                { "PushFloat",      this.handlePushFloat },
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
        }

        private void initOps() {
            this.ops = new Collection {
                { "Sum",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new SumInt(left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new SumFloat(left, right)) },
                { "Subtract",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new SubInt(left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new SubFloat(left, right)) },
                { "Or",
                    new Input2<IValue<bool>, IValue<bool>>((left, right) => new Or(left, right)),
                    new Input2<IValue<int>,  IValue<int>>( (left, right) => new BitwiseOr(left, right)),
                    new Input2<ITrigger,     ITrigger>(    (left, right) => new Any(left, right)) },
                { "Multiply",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new MulInt(left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new MulFloat(left, right)) },
                { "Divide",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new DivInt(left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new DivFloat(left, right)) },
                { "Modulo",
                    new Input2<IValue<int>,    IValue<int>>(   (left, right) => new ModInt(left, right)),
                    new Input2<IValue<double>, IValue<double>>((left, right) => new ModFloat(left, right)) },
                { "And",
                    new Input2<IValue<bool>, IValue<bool>>((left, right) => new And(left, right)),
                    new Input2<IValue<int>,  IValue<int>>( (left, right) => new BitwiseAnd(left, right)),
                    new Input2<ITrigger,     ITrigger>(    (left, right) => new All(left, right)) },
                { "Xor",
                    new Input2<IValue<bool>, IValue<bool>>((left, right) => new Xor(left, right)),
                    new Input2<IValue<int>,  IValue<int>>( (left, right) => new BitwiseXor(left, right)),
                    new Input2<ITrigger,     ITrigger>(    (left, right) => new OnlyOne(left, right)) },
                { "Negate",
                    new Input1<IValue<int>>(   (input) => new NegInt(input)),
                    new Input1<IValue<double>>((input) => new NegFloat(input)) },
                { "Not",
                    new Input1<IValue<bool>>((input) => new Not(input)) },
                { "Invert",
                    new Input1<IValue<int>>((input) => new BitwiseNot(input)) }
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

        private IEnumerable<INode> pop(int count = 1) => this.stack.TakeLast(count);

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
            else throw new Exception("Unexpected type: "+type);
        }

        private void handleLookupInput(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            INode node = this.driver.Find(name);
            if (node is not IInput)
                throw new Exception("May only assign a value directly " + 
                    "to a input variable. " + name + " is not an input variable.");
            this.push(node);
        }

        private void handleAssign(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handlePullTrigger(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleDefine(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleDefineWithType(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleTrinary(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleStartCall(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleCall(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handlePushId(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            INode node = this.driver.Find(name);
            if (node is null)
                throw new Exception(name + " is unknown");
            this.push(node);
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
