using System;
using System.Collections.Generic;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Interface for command registries.
    /// </summary>
    public interface ICommandRegistry
    {
        /// <summary>
        /// The table of all command schemas.
        /// </summary>
        public Dictionary<string, CommandSchema> SchemaTable { get; }
        
        
        /// <summary>
        /// Returns a list of all registered command names.
        /// </summary>
        /// <returns>List of command names.</returns>
        public List<string> AllCommandNames();
        
        
        /// <summary>
        /// Gets the schema for a command if it exists.
        /// </summary>
        /// <param name="fullyQualifiedName">The name of the command and all parents using a dot delimiter.</param>
        /// <param name="schema">The output command schema.</param>
        /// <returns>True if a match was found for the given name.</returns>
        public bool TryResolveCommandSchema(string fullyQualifiedName, out CommandSchema schema);
        
        
        /// <summary>
        /// Gets the deepest valid schema based on a list of tokens.
        /// </summary>
        /// <param name="tokens">The tokens that specify a command path.</param>
        /// <param name="schema">The output command schema.</param>
        /// <returns>True if a match was found for the given tokens.</returns>
        public bool TryResolveCommandSchema(List<string> tokens, out CommandSchema schema);


        /// <summary>
        /// Registers a command to the kernel's registry. Only persists until the domain is reloaded.
        /// </summary>
        /// <param name="type">The type of the command to register. Must inherit from ICommand.</param>
        public void RegisterCommand(Type type);

        
        /// <summary>
        /// Gets the fully qualified name for a command.
        /// </summary>
        /// <param name="commandType">The command type to get the name of.</param>
        /// <returns>The fully qualified name.</returns>
        public string GetFullyQualifiedCommandName(Type commandType);
    }
}