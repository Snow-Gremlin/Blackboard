namespace BlackboardTools {

    /// <summary>This is the interface for any command which can be called.</summary>
    internal interface ICommand {

        /// <summary>The name of the command which is used in the command line to call the implementing command.</summary>
        /// <remarks>This name may not contain any quotes or whitespace.</remarks>
        public string Name { get; }

        /// <summary>The short description to show when help is shown for all commands.</summary>
        public string ShortDescription { get; }

        /// <summary>The long description to shown when help for this command specifically is asked for.</summary>
        public string FullDescription { get; }

        /// <summary>This runs the command with the given arguments.</summary>
        /// <param name="args">The arguments for this command.</param>
        public void Run(CommandArgs args);
    }
}
