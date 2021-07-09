using System.Collections.Generic;
using System.Text;
using Blackboard.Core;
using Blackboard.Core.Bases;
using Blackboard.Core.Caps;
using Blackboard.Core.Interfaces;
using PP = PetiteParser;

namespace Blackboard.Parser {
    public class Parser {

        static private PP.Parser.Parser baseParser => PP.Loader.Loader.
            LoadParser(Encoding.UTF8.GetString(Properties.Resources.Parser));

        private Driver driver;
        private List<INode> stack;

        public Parser(Driver driver) {
            this.driver = driver;
            this.stack = new List<INode>();
        }

        public void Read(params string[] input) =>
            this.Read(input as IEnumerable<string>);

        public void Read(IEnumerable<string> input) {
            PP.Parser.Result result = baseParser.Parse(input);
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
                { "Invert",       this.handleInvert },
                { "StartCall",    this.handleStartCall },
                { "Call",         this.handleCall },
                { "PushId",       this.handlePushId },
                { "PushBool",     this.handlePushBool },
                { "PushInt",      this.handlePushInt },
                { "PushFloat",    this.handlePushFloat },
            });
        }

        private void push(INode node) =>
            this.stack.Add(node);

        private INode pop() {
            int last = this.stack.Count-1;
            INode node = this.stack[last];
            this.stack.RemoveAt(last);
            return node;
        }

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
            // TODO: Implement
        }

        private void handleSubtract(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleOr(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleMultiply(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleDivide(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleModulo(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleAnd(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleXor(PP.ParseTree.PromptArgs args) {
            // TODO: Implement
        }

        private void handleNegate(PP.ParseTree.PromptArgs args) {
            INode node = this.pop();
            if (node is IValue<int>)
                this.push(new NegInt(node as IValue<int>));
            else if (node is IValue<double>)
                this.push(new NegFloat(node as IValue<double>));
            else throw new Exception("Negate expected an IValue<int> or IValue<double> but got "+node.GetType()+".");
        }

        private void handleInvert(PP.ParseTree.PromptArgs args) {
            INode node = this.pop();
            if (node is IValue<int>)
                this.push(new BitwiseNot(node as IValue<int>));
            else throw new Exception("Invert expected an IValue<int> but got "+node.GetType()+".");
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
    }
}
