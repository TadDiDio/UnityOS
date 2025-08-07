using System;
using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing
{
    public class CommandParser : ICommandParser
    {
        private List<IParseRule> _rules = new();
        private readonly ITokenizer _tokenizer;


        /// <summary>
        /// Creates a new parser.
        /// </summary>
        /// <param name="tokenizer">The tokenizer to use.</param>
        public CommandParser(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public TokenizationResult Tokenize(string input)
        {
            return _tokenizer.Tokenize(input);
        }

        /// <summary>
        /// Parses an argument token stream into the parse target.
        /// Note that this stream should only include arguments and not
        /// target identifiers like command names.
        /// </summary>
        /// <param name="stream">The token stream.</param>
        /// <param name="target">The target.</param>
        /// <returns>The result.</returns>
        public ParseResult Parse(TokenStream stream, ICommandParseTarget target)
        {
            ParseContext context = new(target);

            HashSet<ArgumentSpecification> unsetArgs = new(target.GetArguments());

            // Iterate over tokens in order
            while (stream.HasMore())
            {
                bool validParse = false;
                string token = stream.Peek();

                // Find the first rule that matches and get the list of args it should set -
                // note this number is normally one however some rules look for tokens setting multiple at once
                foreach (var rule in _rules)
                {
                    // Filter the args
                    var argsToSet = rule.Filter(token, unsetArgs.ToArray(), context);
                    if (argsToSet == null || argsToSet.Length == 0) continue;

                    int remainingTokens = stream.Count();

                    // Apply rule
                    var parseResult = rule.Apply(stream, argsToSet);

                    if (parseResult.Status is not Status.Success) return parseResult;

                    validParse = true;
                    if (parseResult.Values is { Count: > 0 })
                    {
                        // Check for consumed tokens
                        if (stream.Count() == remainingTokens)
                        {
                            return ParseResult.TokenNotConsumed(token, argsToSet[0]);
                        }

                        // Update iterations and target
                        foreach (var (arg, value) in parseResult.Values!)
                        {
                            unsetArgs.Remove(arg);
                            target.SetArgument(arg, value);
                        }
                    }

                    // Only apply a single rule per token.
                    break;
                }

                if (!validParse)
                {
                    return ParseResult.UnexpectedToken(token);
                }
            }

            if (!target.Validate(out string errorMessage))
            {
                return ParseResult.ValidationFailed(errorMessage);
            }

            return ParseResult.Finished();
        }

        public void RegisterParseRule(Type ruleType)
        {
            if (!typeof(IParseRule).IsAssignableFrom(ruleType))
            {
                Log.Warning($"Command parsing rule of type {ruleType.Name} does not implement " +
                            $"{nameof(IParseRule)} and will be skipped.");
                return;
            }

            if (_rules.Any(rule => rule.GetType() == ruleType)) return;

            _rules.Add((IParseRule)Activator.CreateInstance(ruleType));
            _rules = _rules.OrderBy(r => r.Priority()).ToList();
        }

        public void UnregisterParseRule(Type ruleType)
        {
            IParseRule toRemove = _rules.FirstOrDefault(rule => rule.GetType() == ruleType);
            _rules.Remove(toRemove);
        }
    }
}
