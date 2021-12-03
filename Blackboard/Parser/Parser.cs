using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Parser.Performers;
using Blackboard.Parser.Preppers;
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
        static Parser() {
            baseParser = PP.Loader.Loader.LoadParser(
                PP.Scanner.Default.FromResource(Assembly.GetExecutingAssembly(), resourceName));
        }

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

        #region Formula Methods...

        /// <summary>Reads the given lines of input Blackline code.</summary>
        /// <remarks>The commands of this input will be added to formula if valid.</remarks>
        /// <param name="input">The input code to parse.</param>
        /// <returns>The formula for performing the parsed actions.</returns>
        public Formula Read(params string[] input) =>
            this.Read(input as IEnumerable<string>);

        /// <summary>Reads the given lines of input Blackline code.</summary>
        /// <remarks>The commands of this input will be added to formula if valid.</remarks>
        /// <param name="input">The input code to parse.</param>
        /// <returns>The formula for performing the parsed actions.</returns>
        public Formula Read(IEnumerable<string> input, string name = "Unnamed") {
            PP.Parser.Result result = baseParser.Parse(new PP.Scanner.Default(input, name));

            // Check for parser errors.
            if (result.Errors.Length > 0) throw new Exception(result.Errors.Join("\n"));

            // process the resulting tree to build the formula.
            return this.read(result.Tree);
        }

        /// <summary>Reads the given parse tree root for an input Blackline code.</summary>
        /// <param name="node">The parsed tree root node to read from.</param>
        /// <returns>The formula for performing the parsed actions.</returns>
        private Formula read(PP.ParseTree.ITreeNode node) {
            try {
                Builder stacks = new(this.driver);
                node.Process(this.prompts, stacks);
                return stacks.ToFormula();
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
            this.addHandler("pushId",       handlePushId);
            this.addHandler("pushBool",     handlePushBool);
            this.addHandler("pushBin",      handlePushBin);
            this.addHandler("pushOct",      handlePushOct);
            this.addHandler("pushInt",      handlePushInt);
            this.addHandler("pushHex",      handlePushHex);
            this.addHandler("pushDouble",   handlePushDouble);
            this.addHandler("pushString",   handlePushString);
            this.addHandler("pushType",     handlePushType);

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

        /// <summary>This adds a handler for the given name.</summary>
        /// <param name="name">This is the name of the prompt this handler is for.</param>
        /// <param name="hndl">This is the handler to call on this prompt.</param>
        private void addHandler(string name, S.Action<Builder> hndl) =>
            this.prompts[name] = (PP.ParseTree.PromptArgs args) =>
                hndl(args as Builder);

        /// <summary>This adds a prompt for an operator handler.</summary>
        /// <param name="count">The number of values to pop off the stack for this function.</param>
        /// <param name="name">The name of the prompt to add to.</param>
        private void addProcess(int count, string name) {
            INode funcGroup = this.driver.Global.Find(Driver.OperatorNamespace, name);
            this.prompts[name] = (PP.ParseTree.PromptArgs args) => {
                Builder builder = args as Builder;
                PP.Scanner.Location loc = args.LastLocation;
                IPrepper[] inputs = builder.PopPreppers(count);
                builder.PushPrepper(new FuncPrep(loc, new NoPrep(funcGroup), inputs));
            };
        }

        /// <summary>Validates that all prompts in the grammar are handled.</summary>
        private void validatePrompts() {
            string[] unneeded = baseParser.UnneededPrompts(this.prompts);
            string[] missing  = baseParser.MissingPrompts(this.prompts);
            if (unneeded.Length > 0 || missing.Length > 0)
                throw new Exception("Blackboard's parser grammer has prompts which do not match prompt handlers.").
                    With("Not handled", unneeded.Join(", ")).
                    With("Not in grammer", missing.Join(", "));
        }

        #endregion
        #region Prompt Handlers...

        /// <summary>This is called before each statement to prepare and clean up the parser.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleClear(Builder builder) {
            builder.Tokens.Clear();
            builder.ClearStacks();
        }

        /// <summary>This is called when the namespace has openned.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushNamespace(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string name = builder.LastText;
            IWrappedNode scope = builder.CurrentScope;

            // Check if the namespace already exists, even it if is just virtual.
            IWrappedNode next = scope.ReadField(name);
            if (next is not null) {
                if (next.Type.IsAssignableTo(typeof(Namespace)))
                    throw new Exception("Can not open namespace. Another non-namespace exists by that name.").
                         With("Identifier", name).
                         With("Location", loc);
                builder.PushScope(next);
                return;
            }

            // Create a new virtual namespace and a performer to construct the new namespace if this formula is run.
            VirtualNode nextScope = scope.CreateField(name, typeof(Namespace));
            builder.Add(new VirtualNodeWriter(nextScope, new NodeHold(new Namespace())));
            builder.PushScope(nextScope);
        }

        /// <summary>This is called when the namespace had closed.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePopNamespace(Builder builder) =>
            builder.PopScope();

        /// <summary>This creates a new input node of a specific type without assigning the value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleNewTypeInputNoAssign(Builder builder) {
            IdPrep target = builder.PopPrepper<IdPrep>();
            Type t = builder.PopType();
            PP.Scanner.Location loc = builder.LastLocation;

            IFuncDef inputFactory =
                t == Type.Bool    ? InputValue<Bool>.Factory :
                t == Type.Int     ? InputValue<Int>.Factory :
                t == Type.Double  ? InputValue<Double>.Factory :
                t == Type.String  ? InputValue<String>.Factory :
                t == Type.Trigger ? InputTrigger.Factory :
                throw new Exception("Unsupported type for new typed input").
                    With("Location", loc).
                    With("Type", t);

            VirtualNode virtualInput = target.CreateNode(builder, inputFactory.ReturnType);
            IPerformer inputPerf = new FuncPrep(loc, new NoPrep(inputFactory)).Prepare(builder);
            builder.Add(new VirtualNodeWriter(virtualInput, inputPerf));

            // Push the type back onto the stack for the next assignment.
            builder.PushType(t);
        }

        /// <summary>This creates a new input node of a specific type and assigns it with an initial value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleNewTypeInputWithAssign(Builder builder) {
            IPrepper value = builder.PopPrepper<IPrepper>();
            IdPrep target = builder.PopPrepper<IdPrep>();
            Type t = builder.PopType();
            PP.Scanner.Location loc = builder.LastLocation;

            IPerformer valuePerf = value.Prepare(builder, true);
            IFuncDef inputFactory =
                t == Type.Bool    ? InputValue<Bool>.FactoryWithInitialValue :
                t == Type.Int     ? InputValue<Int>.FactoryWithInitialValue :
                t == Type.Double  ? InputValue<Double>.FactoryWithInitialValue :
                t == Type.String  ? InputValue<String>.FactoryWithInitialValue :
                t == Type.Trigger ? InputTrigger.FactoryWithInitialValue :
                throw new Exception("Unsupported type for new typed input with assignment").
                    With("Type", t);

            VirtualNode virtualInput = target.CreateNode(builder, inputFactory.ReturnType);
            Type valueType = Type.FromType(valuePerf.Type);
            if (!t.Match(valueType).IsMatch)
                throw new Exception("May not assign the value to that type of input.").
                    With("Location", loc).
                    With("Input Type", t).
                    With("Value Type", valueType);

            IPerformer inputPerf = new FuncPrep(loc, new NoPrep(inputFactory), new NoPrep(valuePerf)).Prepare(builder);
            builder.Add(new VirtualNodeWriter(virtualInput, inputPerf));

            // Push the type back onto the stack for the next assignment.
            builder.PushType(t);
        }

        /// <summary>This creates a new input node and assigns it with an initial value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleNewVarInputWithAssign(Builder builder) {
            IPrepper value = builder.PopPrepper<IPrepper>();
            IdPrep target = builder.PopPrepper<IdPrep>();
            PP.Scanner.Location loc = builder.LastLocation;

            IPerformer valuePerf = value.Prepare(builder, true);
            Type t = Type.FromType(valuePerf.Type);
            IFuncDef inputFactory =
                t == Type.Bool    ? InputValue<Bool>.FactoryWithInitialValue :
                t == Type.Int     ? InputValue<Int>.FactoryWithInitialValue :
                t == Type.Double  ? InputValue<Double>.FactoryWithInitialValue :
                t == Type.String  ? InputValue<String>.FactoryWithInitialValue :
                t == Type.Trigger ? InputTrigger.FactoryWithInitialValue :
                throw new Exception("Unsupported type for new input").
                    With("Location", loc).
                    With("Type", t);

            VirtualNode virtualInput = target.CreateNode(builder, inputFactory.ReturnType);
            IPerformer inputPerf = new FuncPrep(loc, new NoPrep(inputFactory), new NoPrep(valuePerf)).Prepare(builder);
            builder.Add(new VirtualNodeWriter(virtualInput, inputPerf));
        }

        /// <summary>This handles defining a new typed named node.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleTypeDefine(Builder builder) {
            IPrepper value = builder.PopPrepper<IPrepper>();
            IdPrep target = builder.PopPrepper<IdPrep>();
            Type t = builder.PopType();
            PP.Scanner.Location loc = builder.LastLocation;

            IPerformer valuePerf = value.Prepare(builder, false);
            Type valueType  = Type.FromType(valuePerf.Type);
            TypeMatch match = t.Match(valueType);
            if (!match.IsMatch)
                throw new Exception("May not define the value to that type of input.").
                    With("Location", loc).
                    With("Input Type", t).
                    With("Value Type", valueType);

            VirtualNode virtualInput = target.CreateNode(builder, t.RealType);
            Namespace ops = builder.Driver.Global.Find(Driver.OperatorNamespace) as Namespace;
            if (match.IsImplicit) {
                INode castGroup =
                    t == Type.Bool    ? ops.Find("castBool") :
                    t == Type.Int     ? ops.Find("castInt") :
                    t == Type.Double  ? ops.Find("castDouble") :
                    t == Type.String  ? ops.Find("castString") :
                    t == Type.Trigger ? ops.Find("castTrigger") :
                    throw new Exception("Unsupported type for new definition cast").
                        With("Location", loc).
                        With("Type", t);

                IFuncDef castFunc = (castGroup as IFuncGroup).Find(valueType);
                valuePerf = new Function(castFunc, valuePerf);
            }
            builder.Add(new VirtualNodeWriter(virtualInput, valuePerf));

            // Push the type back onto the stack for the next definition.
            builder.PushType(t);
        }

        /// <summary>This handles defining a new untyped named node.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleVarDefine(Builder builder) {
            IPrepper value = builder.PopPrepper<IPrepper>();
            IdPrep target = builder.PopPrepper<IdPrep>();

            IPerformer  valuePerf    = value.Prepare(builder, false);
            VirtualNode virtualInput = target.CreateNode(builder, valuePerf.Type);
            builder.Add(new VirtualNodeWriter(virtualInput, valuePerf));
        }

        /// <summary>This handles when a trigger is provoked unconditionally.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleProvokeTrigger(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            IdPrep target = builder.PopPrepper<IdPrep>();
            IPerformer targetPerf = target.Prepare(builder, false);

            NoPrep valuePrep = new(Literal.Bool(true)), targetPrep = new(targetPerf), funcPrep = new(InputTrigger.Assign);
            builder.Add(new FuncPrep(loc, funcPrep, targetPrep, valuePrep).Prepare(builder));

            // Push the literal true onto the stack for any following trigger pulls.
            builder.PushPrepper(valuePrep);
        }

        static private void handleTypeGet(Builder builder) {
            // TODO: Implement
        }

        static private void handleVarGet(Builder builder) {
            // TODO: Implement
        }

        /// <summary>This handles when a trigger should only be provoked if a condition returns true.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleConditionalProvokeTrigger(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            IdPrep target  = builder.PopPrepper<IdPrep>();
            IPrepper value = builder.PopPrepper<IPrepper>();

            IPerformer valuePerf  = value.Prepare(builder, false);
            IPerformer targetPerf = target.Prepare(builder, false);

            Type valueType  = Type.FromType(valuePerf.Type);
            TypeMatch match = Type.Trigger.Match(valueType);
            if (!match.IsMatch)
                throw new Exception("May only conditionally provoke a trigger with a bool or trigger.").
                    With("Location", loc).
                    With("Conditional Type", valueType);

            NoPrep valuePrep = new(valuePerf), targetPrep = new(targetPerf), funcPrep = new(InputTrigger.Assign);
            builder.Add(new FuncPrep(loc, funcPrep, targetPrep, valuePrep).Prepare(builder));

            // Push the condition onto the stack for any following trigger pulls.
            builder.PushPrepper(valuePrep);
        }

        /// <summary>This handles assigning the left value to the right value.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleAssignment(Builder builder) {
            IPrepper value  = builder.PopPrepper<IPrepper>();
            IPrepper target = builder.PopPrepper<IPrepper>();
            PP.Scanner.Location loc = builder.LastLocation;

            IPerformer valuePerf  = value.Prepare(builder, false);
            IPerformer targetPerf = target.Prepare(builder, false);

            // Check if the value is an input, this may have to change if we allow assignments to non-input fields.
            if (targetPerf is not WrappedNodeReader targetReader)
                throw new Exception("The target of an assignment must be a wrapped node.").
                    With("Location", loc).
                    With("Target", targetPerf);
            IWrappedNode wrappedTarget = targetReader.WrappedNode;
            if (!wrappedTarget.Type.IsAssignableTo(typeof(IInput)))
                throw new Exception("The target of an assignment must be an input node.").
                    With("Location", loc).
                    With("Type", wrappedTarget.Type).
                    With("Target", wrappedTarget);

            // Check if the base types match. Don't need to check that the type is
            // a data type or trigger since only those can be reduced to constents.
            Type valueType  = Type.FromType(valuePerf.Type);
            Type targetType = Type.FromType(targetPerf.Type);
            if (!valueType.Match(targetType).IsMatch)
                throw new Exception("The value of an assignment must match base types.").
                    With("Location", loc).
                    With("Target", targetPerf).
                    With("Value", valuePerf);

            IFuncDef assignFunc =
                targetType == Type.Bool    ? InputValue<Bool>.Assign :
                targetType == Type.Int     ? InputValue<Int>.Assign :
                targetType == Type.Double  ? InputValue<Double>.Assign :
                targetType == Type.String  ? InputValue<String>.Assign :
                targetType == Type.Trigger ? InputTrigger.Assign :
                throw new Exception("Unsupported type for assignment").
                    With("Location", loc).
                    With("Type", targetType);

            NoPrep valuePrep = new(valuePerf), targetPrep = new(targetPerf), funcPrep = new(assignFunc);
            builder.Add(new FuncPrep(loc, funcPrep, targetPrep, valuePrep).Prepare(builder));

            // Push the value back onto the stack for any following assignments.
            builder.PushPrepper(valuePrep);
        }

        /// <summary>This handles performing a type cast of a node.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleCast(Builder builder) {
            IPrepper value = builder.PopPrepper<IPrepper>();
            Type t = builder.PopType();
            PP.Scanner.Location loc = builder.LastLocation;

            IPerformer valuePerf = value.Prepare(builder, false);
            Type valueType = Type.FromType(valuePerf.Type);
            TypeMatch match = t.Match(valueType, true);
            if (!match.IsMatch && !match.IsAnyCast)
                throw new Exception("The value type can not be cast to the given type.").
                    With("Location", loc).
                    With("Target", t).
                    With("Type", valueType).
                    With("Value", valuePerf);

            Namespace ops = builder.Driver.Global.Find(Driver.OperatorNamespace) as Namespace;
            if (match.IsAnyCast) {
                INode castGroup =
                    t == Type.Bool    ? ops.Find("castBool") :
                    t == Type.Int     ? ops.Find("castInt") :
                    t == Type.Double  ? ops.Find("castDouble") :
                    t == Type.String  ? ops.Find("castString") :
                    t == Type.Trigger ? ops.Find("castTrigger") :
                    throw new Exception("Unsupported type for new definition cast").
                        With("Location", loc).
                        With("Type", t);

                IFuncDef castFunc = (castGroup as IFuncGroup).Find(valueType);
                valuePerf = new Function(castFunc, valuePerf);
            }

            builder.PushPrepper(new NoPrep(valuePerf));
        }

        /// <summary>This handles accessing an identifier to find the receiver for the next identifier.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleMemberAccess(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string name = builder.LastText;
            IPrepper receiver = builder.PopPrepper<IPrepper>();
            
            builder.PushPrepper(new IdPrep(loc, receiver, name));
        }

        /// <summary>This handles preparing for a method call.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleStartCall(Builder builder) {
            IPrepper item = builder.PopPrepper<IPrepper>();
            PP.Scanner.Location loc = builder.LastLocation;
            builder.PushPrepper(new FuncPrep(loc, item));
        }

        /// <summary>This handles the end of a method call and creates the node for the method.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handleAddArg(Builder builder) {
            IPrepper arg = builder.PopPrepper<IPrepper>();
            FuncPrep func = builder.PopPrepper<FuncPrep>();
            func.Arguments.Add(arg);
            builder.PushPrepper(func);
        }

        /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushId(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string name = builder.LastText;

            builder.PushPrepper(new IdPrep(loc, builder.Scopes, name));
        }

        /// <summary>This handles pushing a bool literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushBool(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string text = builder.LastText;

            try {
                bool value = bool.Parse(text);
                builder.PushPrepper(LiteralPrep.Bool(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a bool.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a binary int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushBin(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string text = builder.LastText;

            try {
                int value = S.Convert.ToInt32(text, 2);
                builder.PushPrepper(LiteralPrep.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a binary int.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing an ocatal int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushOct(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string text = builder.LastText;

            try {
                int value = S.Convert.ToInt32(text, 8);
                builder.PushPrepper(LiteralPrep.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse an octal int.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a decimal int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushInt(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string text = builder.LastText;

            try {
                int value = int.Parse(text);
                builder.PushPrepper(LiteralPrep.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a decimal int.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a hexadecimal int literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushHex(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string text = builder.LastText[2..];

            try {
                int value = int.Parse(text, NumberStyles.HexNumber);
                builder.PushPrepper(LiteralPrep.Int(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a hex int.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a double literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushDouble(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string text = builder.LastText;

            try {
                double value = double.Parse(text);
                builder.PushPrepper(LiteralPrep.Double(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to parse a double.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a string literal value onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushString(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string text = builder.LastText;

            try {
                string value = PP.Misc.Text.Unescape(text);
                builder.PushPrepper(LiteralPrep.String(value));
            } catch (S.Exception ex) {
                throw new Exception("Failed to decode escaped sequences.", ex).
                    With("Text", text).
                    With("Location", loc);
            }
        }

        /// <summary>This handles pushing a type onto the stack.</summary>
        /// <param name="builder">The formula builder being worked on.</param>
        static private void handlePushType(Builder builder) {
            PP.Scanner.Location loc = builder.LastLocation;
            string text = builder.LastText;

            Type t = Type.FromName(text);
            if (t is null)
                throw new Exception("Unrecognized type name.").
                    With("Text", text).
                    With("Location", loc);

            builder.PushType(t);
        }

        #endregion
    }
}
