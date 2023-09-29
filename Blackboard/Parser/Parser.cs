using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Formula;
using Blackboard.Core.Formula.Factory;
using Blackboard.Core.Innate;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PP = PetiteParser;
using S = System;

namespace Blackboard.Parser;

/// <summary>This will parse the Blackboard language into actions and nodes to apply to the slate.</summary>
sealed internal class Parser {

    /// <summary>Prepares the parser's static variables before they are used.</summary>
    static Parser() => baseParser = PP.Loader.Loader.LoadParser(
        new PP.Scanner.Joiner(
            PP.Scanner.DefaultScanner.FromResource(Assembly.GetExecutingAssembly(), "Blackboard.Parser.Language.Grammar.lang"),
            PP.Scanner.DefaultScanner.FromResource(Assembly.GetExecutingAssembly(), "Blackboard.Parser.Language.Keywords.lang"),
            PP.Scanner.DefaultScanner.FromResource(Assembly.GetExecutingAssembly(), "Blackboard.Parser.Language.Tokens.lang")
        ), ignoreConflicts: false);

    /// <summary>The Blackboard language base parser singleton.</summary>
    static private readonly PP.Parser.Parser baseParser;

    /// <summary>The slate that this Blackboard is to create the actions for.</summary>
    private readonly Slate slate;

    /// <summary>The list of prompt handlers for running the Blackboard grammar with.</summary>
    private readonly Dictionary<string, PP.ParseTree.PromptHandle<Builder.Builder>> prompts;

    /// <summary>Optional logger to debugging and inspecting the parser.</summary>
    private readonly Logger? logger;

    /// <summary>Creates a new Blackboard language parser.</summary>
    /// <param name="slate">The slate to modify.</param>
    /// <param name="logger">An optional logger for debugging and inspecting the parser.</param>
    public Parser(Slate slate, Logger? logger = null) {
        this.slate   = slate;
        this.prompts = new Dictionary<string, PP.ParseTree.PromptHandle<Builder.Builder>>();
        this.logger  = logger.Group(nameof(Parser));

        // Console.WriteLine(PP.Parser.Parser.GetDebugStateString(BaseParser.Grammar));

        this.initPrompts();
        this.validatePrompts();
    }

    #region Read Methods...

    /// <summary>Reads the given lines of input Blackboard code.</summary>
    /// <remarks>The commands of this input will be added to formula if valid.</remarks>
    /// <param name="input">The input code to parse.</param>
    /// <returns>The formula for performing the parsed actions.</returns>
    public Formula Read(params string[] input) =>
        this.Read(input as IEnumerable<string>);

    /// <summary>Reads the given lines of input Blackboard code.</summary>
    /// <remarks>The commands of this input will be added to formula if valid.</remarks>
    /// <param name="input">The input code to parse.</param>
    /// <returns>The formula for performing the parsed actions.</returns>
    public Formula Read(IEnumerable<string> input, string name = "Unnamed") {
        PP.Parser.Result result = baseParser.Parse(new PP.Scanner.DefaultScanner(input, name));

        // Check for parser errors.
        PP.ParseTree.ITreeNode? tree = result.Tree;
        if (result.Errors.Length > 0 || tree is null)
            throw new BlackboardException("Errors while reading code source").
                With("Errors", result.Errors.Join("\n"));

        // process the resulting tree to build the formula.
        return this.read(tree);
    }

    /// <summary>Reads the given parse tree root for an input Blackboard code.</summary>
    /// <param name="node">The parsed tree root node to read from.</param>
    /// <returns>The formula for performing the parsed actions.</returns>
    private Formula read(PP.ParseTree.ITreeNode node) {
        try {
            this.logger.Info("Parser Read");
            Factory factory = new(this.slate, this.logger);
            node.Process(this.prompts, new Builder.Builder(factory));
            this.logger.Info("Parser Done");
            return factory.Build();
        } catch (S.Exception ex) {
            throw new BlackboardException("Error occurred while parsing input code.").
                With("Error", ex);
        }
    }

    #endregion
    #region Prompts Setup...

    /// <summary>Initializes the prompts and operators for this parser.</summary>
    /// <remarks>
    /// The prompts are defined as static methods which then calls into the builder used as the arguments.
    /// This allows for reads to occur in parallel since all the state is kept in the builder.
    /// A unique builder is created per read.
    /// </remarks>
    private void initPrompts() {
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
        this.addHandler("varGet" , handleVarGet);
        
        this.addHandler("typeTemp", handleTypeTemp);
        this.addHandler("varTemp",  handleVarTemp);

        this.addHandler("externNoAssign",   handleExternNoAssign);
        this.addHandler("externWithAssign", handleExternWithAssign);

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
        this.addProcess(2, "power");
        this.addProcess(1, "negate");
        this.addProcess(1, "not");
        this.addProcess(1, "invert");
    }

    /// <summary>This adds a handler for the given name.</summary>
    /// <param name="name">This is the name of the prompt this handler is for.</param>
    /// <param name="hndl">This is the handler to call on this prompt.</param>
    private void addHandler(string name, S.Action<Builder.Builder> hndl) =>
        this.prompts[name] = (Builder.Builder builder) => {
            try {
                this.logger.Info("Handle {0} [{1}]", name, builder.LastLocation);
                hndl(builder);
            } catch (S.Exception ex) {
                this.logger.Error("Error while handling {0} [{1}]: {2}", name, builder.LastLocation, ex);
                throw;
            }
        };

    /// <summary>This adds a prompt for an operator handler.</summary>
    /// <param name="count">The number of values to pop off the stack for this function.</param>
    /// <param name="name">The name of the prompt to add to.</param>
    private void addProcess(int count, string name) {
        if (this.slate.Global.Find(Operators.Namespace, name) is not IFuncGroup funcGroup)
            throw new BlackboardException("Could not find the operation by the given name.").
                With("Name", name);
        this.prompts[name] = (Builder.Builder builder) =>
            builder.HandleProcesses(count, name, funcGroup);
    }

    /// <summary>Validates that all prompts in the grammar are handled.</summary>
    private void validatePrompts() {
        string[] unneeded = baseParser.UnneededPrompts(this.prompts);
        string[] missing  = baseParser.MissingPrompts(this.prompts);
        if (unneeded.Length > 0 || missing.Length > 0)
            throw new BlackboardException("Blackboard's parser grammar has prompts which do not match prompt handlers.").
                With("Not handled", unneeded.Join(", ")).
                With("Not in grammar", missing.Join(", "));
    }

    #endregion
    #region Prompt Handlers...

    /// <summary>This is called before each statement to prepare and clean up the parser.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleClear(Builder.Builder builder) => builder.HandleClear();

    /// <summary>This is called when a new simple identifier has been defined.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleDefineId(Builder.Builder builder) => builder.HandleDefineId();

    /// <summary>This is called when the namespace has opened.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushNamespace(Builder.Builder builder) => builder.HandlePushNamespace();

    /// <summary>This is called when the namespace had closed.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePopNamespace(Builder.Builder builder) => builder.HandlePopNamespace();

    /// <summary>This creates a new input node of a specific type without assigning the value.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleNewTypeInputNoAssign(Builder.Builder builder) => builder.HandleNewTypeInputNoAssign();

    /// <summary>This creates a new input node of a specific type and assigns it with an initial value.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleNewTypeInputWithAssign(Builder.Builder builder) => builder.HandleNewTypeInputWithAssign();

    /// <summary>This creates a new input node and assigns it with an initial value.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleNewVarInputWithAssign(Builder.Builder builder) => builder.HandleNewVarInputWithAssign();

    /// <summary>This handles defining a new typed named node.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleTypeDefine(Builder.Builder builder) => builder.HandleTypeDefine();

    /// <summary>This handles defining a new untyped named node.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleVarDefine(Builder.Builder builder) => builder.HandleVarDefine();

    /// <summary>This handles when a trigger is provoked unconditionally.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleProvokeTrigger(Builder.Builder builder) => builder.HandleProvokeTrigger();

    /// <summary>This handles when a trigger should only be provoked if a condition returns true.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleConditionalProvokeTrigger(Builder.Builder builder) => builder.HandleConditionalProvokeTrigger();

    /// <summary>This handles getting the typed left value and writing it out to the given name.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleTypeGet(Builder.Builder builder) => builder.HandleTypeGet();

    /// <summary>This handles getting the variable type left value and writing it out to the given name.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleVarGet(Builder.Builder builder) => builder.HandleVarGet();

    /// <summary>This handles getting the typed left value as a temporary value with the given name.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleTypeTemp(Builder.Builder builder) => builder.HandleTypeTemp();

    /// <summary>This handles getting the variable type left value as a temporary value with the given name.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleVarTemp(Builder.Builder builder) => builder.HandleVarTemp();
    
    /// <summary>This handles checking for an existing node or creating an external node if there is no existing node.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleExternNoAssign(Builder.Builder builder) => builder.HandleExternNoAssign();
    
    /// <summary>
    /// This handles checking for an existing node or creating an external node if there is no existing node.
    /// If no node exists then the default value for the existing node will be set.
    /// </summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleExternWithAssign(Builder.Builder builder) => builder.HandleExternWithAssign();

    /// <summary>This handles assigning the left value to the right value.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleAssignment(Builder.Builder builder) => builder.HandleAssignment();

    /// <summary>This handles performing a type cast of a node.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleCast(Builder.Builder builder) => builder.HandleCast();

    /// <summary>This handles accessing an identifier to find the receiver for the next identifier.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleMemberAccess(Builder.Builder builder) => builder.HandleMemberAccess();

    /// <summary>This handles preparing for a method call.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleStartCall(Builder.Builder builder) => builder.HandleStartCall();

    /// <summary>This handles the end of a method call and creates the node for the method.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleAddArg(Builder.Builder builder) => builder.HandleAddArg();

    /// <summary>This handles finishing a method call and building the node for the method.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleEndCall(Builder.Builder builder) => builder.HandleEndCall();

    /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushId(Builder.Builder builder) => builder.HandlePushId();

    /// <summary>This handles pushing a bool literal value onto the stack.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushBool(Builder.Builder builder) => builder.HandlePushBool();

    /// <summary>This handles pushing a binary int literal value onto the stack.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushBin(Builder.Builder builder) => builder.HandlePushBin();

    /// <summary>This handles pushing an octal int literal value onto the stack.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushOct(Builder.Builder builder) => builder.HandlePushOct();

    /// <summary>This handles pushing a decimal int literal value onto the stack.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushInt(Builder.Builder builder) => builder.HandlePushInt();

    /// <summary>This handles pushing a hexadecimal int literal value onto the stack.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushHex(Builder.Builder builder) => builder.HandlePushHex();

    /// <summary>This handles pushing a double literal value onto the stack.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushDouble(Builder.Builder builder) => builder.HandlePushDouble();

    /// <summary>This handles pushing a string literal value onto the stack.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushString(Builder.Builder builder) => builder.HandlePushString();

    /// <summary>This handles pushing a type onto the stack.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handlePushType(Builder.Builder builder) => builder.HandlePushType();

    #endregion
}
