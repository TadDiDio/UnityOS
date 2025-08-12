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
        public List<string> Tokens;

        /// <summary>
        /// Creates a new text string resolver.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public TokenCommandResolver(List<string> tokens) => Tokens = tokens;

        public CommandResolutionResult Resolve(ShellSession session, bool expandAliases)
        {
            // Empty input means we give empty output
            if (Tokens == null || Tokens.Count == 0) return CommandResolutionResult.Failed("");

            if (expandAliases)
            {
                var expanded = session.GetAlias(Tokens[0], out bool replaced);

                expanded.AddRange(Tokens.Skip(1));
                Tokens = expanded;

                if (replaced)
                {
                    return CommandResolutionResult.AliasExpansion(Tokens);
                }
            }

            if (!ConsoleAPI.Commands.TryResolveCommandSchema(Tokens, out var schema))
            {
                return CommandResolutionResult.Failed($"Could not find a command with name {Tokens[0]}.");
            }

            // Strip name
            string fullName = ConsoleAPI.Commands.GetFullyQualifiedName(schema.CommandType);
            Tokens = StripName(fullName, Tokens);

            // Parse command
            TokenStream stream = new(Tokens);
            CommandParseTarget commandParseTarget = new(schema);

            var parseResult = ConsoleAPI.Parsing.ParseCommand(stream, commandParseTarget);

            if (parseResult.Status is not Status.Success)
            {
                return CommandResolutionResult.Failed(parseResult.ErrorMessage);
            }

            return CommandResolutionResult.Success(commandParseTarget.Command);
        }

        private List<string> StripName(string fullyQualifiedName, List<string> tokens)
        {
            if (tokens == null || tokens.Count == 0) return new List<string>();

            var commandParts = fullyQualifiedName.Split('.');

            int commandIndex = 0;
            int tokenIndex = 0;

            while (commandIndex < commandParts.Length && tokenIndex < tokens.Count)
            {
                string token = tokens[tokenIndex];

                var tokenSubParts = token.Split('.');

                int subPartsMatched = 0;
                int cmdPartCheckIndex = commandIndex;

                while (subPartsMatched < tokenSubParts.Length && cmdPartCheckIndex < commandParts.Length)
                {
                    if (tokenSubParts[subPartsMatched] == commandParts[cmdPartCheckIndex])
                    {
                        subPartsMatched++;
                        cmdPartCheckIndex++;
                    }
                    else break;
                }

                if (subPartsMatched == tokenSubParts.Length)
                {
                    commandIndex += subPartsMatched;
                    tokenIndex++;
                }
                else break;
            }

            return tokens.Skip(tokenIndex).ToList();
        }
    }
}
