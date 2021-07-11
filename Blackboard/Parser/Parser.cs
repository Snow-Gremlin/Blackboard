using System.Collections.Generic;
using System.Text;
using System.Linq;
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

        public Parser(Driver driver) {
            this.driver = driver;
            this.stack = new LinkedList<INode>();
        }

        public void Read(params string[] input) =>
            this.Read(input as IEnumerable<string>);

        public void Read(IEnumerable<string> input) {
            PP.Parser.Result result = BaseParser.Parse(input);
            if (result.Errors.Length > 0)
                throw new Exception(string.Join('\n', result.Errors));
            this.read(result.Tree);
        }

        private void read(PP.ParseTree.ITreeNode node) {
            node.Process(new Dictionary<string, PP.ParseTree.PromptHandle>(){
                { "InputDefault", this.handleInputDefault },
                { "InputAssign",  this.handleInputAssign },
                { "StartAssign",  this.handleStartAssign },
                { "EndAssign",    this.handleEndAssign },
                { "PullTrigger",  this.handlePullTrigger },
                { "StartDefine",  this.handleStartDefine },
                { "EndDefine",    this.handleEndDefine },
                { "Trinary",      this.handleTrinary },
                { "Add",          this.handleAdd },
                { "Subtract",     this.handleSubtract },
                { "Or",           this.handleOr },
                { "Multiply",     this.handleMultiply },
                { "Divide",       this.handleDivide },
                { "Modulo",       this.handleModulo },
                { "And",          this.handleAnd },
                { "Xor",          this.handleXor },
                { "Negate",       this.handleNegate },
                { "Not",          this.handleNot },
                { "Invert",       this.handleInvert },
                { "StartCall",    this.handleStartCall },
                { "Call",         this.handleCall },
                { "PushId",       this.handlePushId },
                { "PushBool",     this.handlePushBool },
                { "PushInt",      this.handlePushInt },
                { "PushFloat",    this.handlePushFloat },
            });
        }

        #region Helpers...

        private void push(INode node) =>
            this.stack.AddLast(node);

        private INode pop() {
            INode node = this.stack.Last();
            this.stack.RemoveLast();
            return node;
        }

        /// <summary>Determines if the given nodes can be cast to bool IValues.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>True if they can be cast, otherwise false.</returns>
        static private bool isBool(params INode[] nodes) {
            foreach (INode node in nodes) {
                if (node is not IValue<bool>) return false;
            }
            return true;
        }

        /// <summary>Determines if the given nodes can be cast to triggers.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>True if they can be cast, otherwise false.</returns>
        static private bool isTrigger(params INode[] nodes) {
            foreach (INode node in nodes) {
                if (node is not ITrigger &&
                    node is not IValue<bool>) return false;
            }
            return true;
        }

        /// <summary>Determines if the given nodes can be cast to int IValues.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>True if they can be cast, otherwise false.</returns>
        static private bool isInt(params INode[] nodes) {
            foreach (INode node in nodes) {
                if (node is not IValue<int>) return false;
            }
            return true;
        }

        /// <summary>Determines if the given nodes can be cast to double IValues.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>True if they can be cast, otherwise false.</returns>
        static private bool isDouble(params INode[] nodes) {
            foreach (INode node in nodes) {
                if (node is not IValue<double> &&
                    node is not IValue<int>) return false;
            }
            return true;
        }

        /// <summary>Casts the given node to a bool IValue.</summary>
        /// <param name="node">The node to cast.</param>
        /// <returns>The bool IValue or it throws an exception if it can't cast.</returns>
        static private IValue<bool> asBool(INode node) =>
            node is IValue<bool> value ? value :
            throw new Exception("Can not cast "+node+" to IValue<bool>.");

        /// <summary>Casts the given node to a trigger.</summary>
        /// <param name="node">The node to cast.</param>
        /// <returns>The bool IValue or it throws an exception if it can't cast.</returns>
        static private ITrigger asTrigger(INode node) =>
            node is ITrigger trig ? trig :
            node is IValue<bool> value ? new OnTrue(value) :
            throw new Exception("Can not cast "+node+" to ITrigger.");

        /// <summary>Casts the given node to a int IValue.</summary>
        /// <param name="node">The node to cast.</param>
        /// <returns>The int IValue or it throws an exception if it can't cast.</returns>
        static private IValue<int> asInt(INode node) =>
            node is IValue<int> value ? value :
            throw new Exception("Can not cast "+node+" to IValue<int>.");

        /// <summary>Casts the given node to a double IValue.</summary>
        /// <param name="node">The node to cast.</param>
        /// <returns>The double IValue or it throws an exception if it can't cast.</returns>
        static private IValue<double> asDouble(INode node) =>
            node is IValue<double> dValue ? dValue :
            node is IValue<int> iValue ? new IntToFloat(iValue) :
            throw new Exception("Can not cast "+node+" to IValue<double>.");

        /// <summary>Gets a pretty name used for exceptions which can be thrown for invalid input.</summary>
        /// <param name="node">The node to get the name for.</param>
        /// <returns>The name for the node.</returns>
        static private string prettyName(INode node) =>
            node is Counter        ? "counter" :
            node is Toggler        ? "toggler" :
            node is IValue<bool>   ? "bool" :
            node is IValue<int>    ? "int" :
            node is IValue<double> ? "float" :
            node is ITrigger       ? "trigger" :
            "unknown:"+node;

        #endregion
        #region Handlers...

        private void handleInputDefault(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleInputAssign(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleStartAssign(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleEndAssign(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handlePullTrigger(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleStartDefine(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleEndDefine(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleTrinary(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleAdd(PP.ParseTree.PromptArgs args) {
            INode right = this.pop(), left = this.pop();
            if (isInt(left, right))         this.push(new SumInt(asInt(left), asInt(right)));
            else if (isDouble(left, right)) this.push(new SumFloat(asDouble(left), asDouble(right)));
            else throw new Exception("Add expected two ints or two double " +
                "but got " + prettyName(left) + " and " + prettyName(right) + ".");
        }

        private void handleSubtract(PP.ParseTree.PromptArgs args) {
            INode right = this.pop(), left = this.pop();
            if (isInt(left, right))         this.push(new SubInt(asInt(left), asInt(right)));
            else if (isDouble(left, right)) this.push(new SubFloat(asDouble(left), asDouble(right)));
            else throw new Exception("Subtract expected two ints or two double " +
                "but got " + prettyName(left) + " and " + prettyName(right) + ".");
        }

        private void handleOr(PP.ParseTree.PromptArgs args) {
            INode right = this.pop(), left = this.pop();
            if (isBool(left, right))         this.push(new Or(asBool(left), asBool(right)));
            else if (isTrigger(left, right)) this.push(new Any(asTrigger(left), asTrigger(right)));
            else if (isInt(left, right))     this.push(new BitwiseOr(asInt(left), asInt(right)));
            else throw new Exception("Or expected two triggers, two bools, or two ints " +
                "but got " + prettyName(left) + " and " + prettyName(right) + ".");
        }

        private void handleMultiply(PP.ParseTree.PromptArgs args) {
            INode right = this.pop(), left = this.pop();
            if (isInt(left, right))         this.push(new MulInt(asInt(left), asInt(right)));
            else if (isDouble(left, right)) this.push(new MulFloat(asDouble(left), asDouble(right)));
            else throw new Exception("Multiply expected two ints or two double " +
                "but got " + prettyName(left) + " and " + prettyName(right) + ".");
        }

        private void handleDivide(PP.ParseTree.PromptArgs args) {
            INode right = this.pop(), left = this.pop();
            if (isInt(left, right))         this.push(new DivInt(asInt(left), asInt(right)));
            else if (isDouble(left, right)) this.push(new DivFloat(asDouble(left), asDouble(right)));
            else throw new Exception("Divide expected two ints or two double " +
                "but got " + prettyName(left) + " and " + prettyName(right) + ".");
        }

        private void handleModulo(PP.ParseTree.PromptArgs args) {
            INode right = this.pop(), left = this.pop();
            if (isInt(left, right))         this.push(new ModInt(asInt(left), asInt(right)));
            else if (isDouble(left, right)) this.push(new ModFloat(asDouble(left), asDouble(right)));
            else throw new Exception("Modulo expected two ints or two double " +
                "but got " + prettyName(left) + " and " + prettyName(right) + ".");
        }

        private void handleAnd(PP.ParseTree.PromptArgs args) {
            INode right = this.pop(), left = this.pop();
            if (isBool(left, right))         this.push(new And(asBool(left), asBool(right)));
            else if (isTrigger(left, right)) this.push(new All(asTrigger(left), asTrigger(right)));
            else if (isInt(left, right))     this.push(new BitwiseAnd(asInt(left), asInt(right)));
            else throw new Exception("And expected two triggers, two bools, or two ints " +
                "but got " + prettyName(left) + " and " + prettyName(right) + ".");
        }
         
        private void handleXor(PP.ParseTree.PromptArgs args) {
            INode right = this.pop(), left = this.pop();
            if (isBool(left, right))         this.push(new Xor(asBool(left), asBool(right)));
            else if (isTrigger(left, right)) this.push(new OnlyOne(asTrigger(left), asTrigger(right)));
            else if (isInt(left, right))     this.push(new BitwiseXor(asInt(left), asInt(right)));
            else throw new Exception("Xor expected two triggers, two bools, or two ints " +
                "but got " + prettyName(left) + " and " + prettyName(right) + ".");
        }

        private void handleNegate(PP.ParseTree.PromptArgs args) {
            INode node = this.pop();
            if (isInt(node))         this.push(new NegInt(asInt(node)));
            else if (isDouble(node)) this.push(new NegFloat(asDouble(node)));
            else throw new Exception("Negate expected an int or float but got " + prettyName(node) + ".");
        }

        private void handleNot(PP.ParseTree.PromptArgs args) {
            INode node = this.pop();
            if (isBool(node)) this.push(new Not(asBool(node)));
            else throw new Exception("Not expected a bool but got " + prettyName(node) + ".");
        }

        private void handleInvert(PP.ParseTree.PromptArgs args) {
            INode node = this.pop();
            if (isInt(node)) this.push(new BitwiseNot(asInt(node)));
            else throw new Exception("Invert expected an int but got " + prettyName(node) + ".");
        }

        private void handleStartCall(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleCall(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handlePushId(PP.ParseTree.PromptArgs args) {
            string name = args.Tokens[^1].Text;
            this.push(this.driver.Find(name));
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
