using System.Reflection;
using Codice.Client.BaseCommands;
using DeveloperConsole.Core;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Resolves a text string to a command.
    /// </summary>
    public class TextCommandResolver : ICommandResolver
    {
        private string _rawInput;
        
        /// <summary>
        /// Creates a new text string resolver.
        /// </summary>
        /// <param name="rawInput">The raw text.</param>
        public TextCommandResolver(string rawInput) => _rawInput = rawInput;
        
        public CommandResolutionResult Resolve(ShellSession session)
        {
            // Tokenize input
            var tokenizeResult = ConsoleAPI.Parsing.Tokenize(_rawInput);
            if (tokenizeResult.Status is not TokenizationStatus.Success)
            {
                // Only possible failure case is an empty input, so we will propagate an empty output.
                return CommandResolutionResult.Failed("");
            }
            
            if (!ConsoleAPI.Commands.TryResolveCommandSchema(tokenizeResult.Tokens, out var schema))
            {
                return CommandResolutionResult.Failed($"Could not find a command with name {tokenizeResult.Tokens[0]}.");
            }

            // Strip name
            string fullName = ConsoleAPI.Commands.GetFullyQualifiedName(schema.CommandType);
            string[] parts = fullName.Split('.');

            int idx = 0;
            while (idx < parts.Length && parts[idx].Equals(tokenizeResult.Tokens[idx])) idx++;
            tokenizeResult.Tokens.RemoveRange(0, idx);
            
            // Parse command
            TokenStream stream = new(tokenizeResult.Tokens);
            CommandCommandParseTarget commandParseTarget = new(schema);
            
            var parseResult = ConsoleAPI.Parsing.ParseCommand(stream, commandParseTarget);
            
            if (parseResult.Status is not Status.Success)
            {
                return CommandResolutionResult.Failed(parseResult.ErrorMessage);
            }
            
            return CommandResolutionResult.Success(commandParseTarget.Command);
        }
    }
}