using System.Linq;
using DeveloperConsole.Parsing;
using System.Collections.Generic;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Resolves a text string to a command.
    /// </summary>
    public class TokenCommandResolver : ICommandResolver
    {
        private List<string> _tokens;

        /// <summary>
        /// Creates a new text string resolver.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public TokenCommandResolver(List<string> tokens) => _tokens = tokens;

        public CommandResolutionResult Resolve(ShellSession session, bool expandAliases)
        {
            // Empty input means we give empty output
            if (_tokens == null || _tokens.Count == 0) return CommandResolutionResult.Failed("");

            if (expandAliases)
            {
                var expanded = session.GetAlias(_tokens[0], out bool replaced);

                expanded.AddRange(_tokens.Skip(1));
                _tokens = expanded;

                if (replaced)
                {
                    return CommandResolutionResult.AliasExpansion(_tokens);
                }
            }

            if (!ConsoleAPI.Commands.TryResolveCommandSchema(_tokens, out var schema))
            {
                return CommandResolutionResult.Failed($"Could not find a command with name {_tokens[0]}.");
            }

            // Strip name
            string fullName = ConsoleAPI.Commands.GetFullyQualifiedName(schema.CommandType);
            string[] parts = fullName.Split('.');

            int idx = 0;
            while (idx < parts.Length && parts[idx].Equals(_tokens[idx])) idx++;
            _tokens.RemoveRange(0, idx);

            // Parse command
            TokenStream stream = new(_tokens);
            CommandParseTarget commandParseTarget = new(schema);

            var parseResult = ConsoleAPI.Parsing.ParseCommand(stream, commandParseTarget);

            if (parseResult.Status is not Status.Success)
            {
                return CommandResolutionResult.Failed(parseResult.ErrorMessage);
            }

            return CommandResolutionResult.Success(commandParseTarget.Command);
        }
    }
}
