using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DeveloperConsole.Command;
using DeveloperConsole.Command.Execution;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Graph
{
    public struct GraphParseResult
    {
        public bool Succeeded;
        public string ErrorMessage;
        public CommandGraph Graph;

        private GraphParseResult(bool success, string errorMessage, CommandGraph graph)
        {
            Succeeded = success;
            ErrorMessage = errorMessage;
            Graph = graph;
        }

        public static GraphParseResult Failure(string message)
        {
            return new GraphParseResult(false, message, null);
        }

        public static GraphParseResult Success(CommandGraph graph)
        {
            return new GraphParseResult(true, null, graph);
        }
    }

    // TODO: Convert to system dependency in DI container and allow users to override specific strings for behavior
    // Also consider making this a rule bases system like the parser so that adding new rules is easy.
    public class GraphParser
    {
        private class IntermediateContext
        {
            public GraphNode Node = new();
            public string FromLink;
        }

        /*
         * Considers the following mappings currently:
         *
         *  ;  = On completion
         *  && = On success
         *  || = On failure
         *  |  = Pipe
         */
        private Regex _linkSplitter = new(@"(;|&&|&|\|\||\|)", RegexOptions.Compiled);

        private string _errorMessage;

        /// <summary>
        /// Makes an execution graph from a string.
        /// </summary>
        /// <param name="commandLine">The raw command line input.</param>
        /// <param name="session">The session this input is associated with.</param>
        /// <returns>The parsing result.</returns>
        public GraphParseResult ParseToGraph(string commandLine, ShellSession session)
        {
            var parts  = _linkSplitter.Split(commandLine);
            var tokens = TokenizeAndExpand(parts, session, true);

            if (!MakeContexts(tokens, out var contexts) || !BuildGraph(contexts, out var graph))
            {
                return GraphParseResult.Failure(_errorMessage);
            }

            return GraphParseResult.Success(graph);
        }

        private bool IsLink(string token)
        {
            return _linkSplitter.IsMatch(token) && token.Length == _linkSplitter.Match(token).Value.Length;
        }

        // TODO: Currently does not handle !alias, that is an error
        private List<string> TokenizeAndExpand(string[] parts, ShellSession session, bool expandAliases)
        {
            var result = new List<string>();

            foreach (var part in parts)
            {
                // -- Directly add links
                if (IsLink(part))
                {
                    result.Add(part);
                    continue;
                }

                // -- Tokenize the initial input
                var tokenizeResult = ConsoleAPI.Parsing.Tokenize(part);
                if (tokenizeResult.Status is TokenizationStatus.Empty)
                {
                    result.Add("");
                    continue;
                }

                // -- Check if there is an alias and expand if so
                if (!expandAliases || !session.AliasManager.TryGetAlias(tokenizeResult.Tokens[0], out var alias))
                {
                    result.AddRange(tokenizeResult.Tokens);
                    continue;
                }

                // -- Split the alias into tokens and add them
                var aliasParts = _linkSplitter.Split(alias);
                var aliasTokens = TokenizeAndExpand(aliasParts, session, false);
                aliasTokens.AddRange(tokenizeResult.Tokens.Skip(1));

                result.AddRange(aliasTokens);
            }

            return result;
        }

        // Tokens come in the alternating form cmd, link, cmd, link with "" representing an empty command
        private bool MakeContexts(List<string> tokens, out List<IntermediateContext> contexts)
        {
            contexts = new List<IntermediateContext>();

            // End tokens with a ";" to avoid single command edge case not being computed
            tokens.Add(";");

            var currentTokens = new List<string>();
            var fromLink = ";";
            foreach (var token in tokens)
            {
                if (!IsLink(token))
                {
                    currentTokens.Add(token);
                    continue;
                }

                var context = new IntermediateContext();

                if (currentTokens[0].StartsWith("!"))
                {
                    currentTokens[0] = currentTokens[0][1..];
                    context.Node.ExecutionContext.Windowed = true;
                }

                // Don't try to execute null entries
                if (string.IsNullOrWhiteSpace(currentTokens[0]))
                {
                    currentTokens.Clear();
                    fromLink = token;
                    continue;
                }

                if (!ParseToCommand(currentTokens, out var command)) return false;

                context.FromLink = fromLink;
                context.Node.Command = command;
                contexts.Add(context);
                currentTokens.Clear();

                fromLink = token;
            }

            return true;
        }

        private bool ParseToCommand(List<string> commandTokens, out ICommand command)
        {
            command = null;

            // Ensure we have a matching schema
            if (!ConsoleAPI.Commands.TryResolveCommandSchema(commandTokens, out var schema))
            {
                _errorMessage = $"Could not find a command with name {commandTokens[0]}.";
                return false;
            }

            // Strip name from input tokens
            string fullName = ConsoleAPI.Commands.GetFullyQualifiedName(schema.CommandType);
            commandTokens = StripName(fullName, commandTokens);

            // Parse the command
            TokenStream stream = new(commandTokens);
            CommandParseTarget commandParseTarget = new(schema);
            var parseResult = ConsoleAPI.Parsing.ParseCommand(stream, commandParseTarget);

            if (parseResult.Status is not Status.Success)
            {
                _errorMessage = parseResult.ErrorMessage;
                return false;
            }

            command = commandParseTarget.Command;
            return true;
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

        private bool BuildGraph(List<IntermediateContext> contexts, out CommandGraph graph)
        {
            graph = null;
            GraphBuilder builder = new();

            foreach (var context in contexts)
            {
                switch (context.FromLink)
                {
                    case ";":
                        builder.Then(context.Node);
                        break;
                    case "&&":
                        builder.ThenOnSuccess(context.Node);
                        break;
                    case "||":
                        builder.ThenOnFailure(context.Node);
                        break;
                    case "|":
                        builder.AndPipeTo(context.Node);
                        break;
                    case "&":
                        builder.And(context.Node);
                        break;
                    default:
                        _errorMessage = $"Unknown link type: '{context.FromLink}'";
                        return false;
                }
            }

            graph = builder.Build();
            return true;
        }
    }
}
