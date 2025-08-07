using System;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// Parses input strings to parse targets.
    /// </summary>
    public interface ICommandParser
    {
        /// <summary>
        /// Tokenizes the input string to a list of tokens.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result.</returns>
        public TokenizationResult Tokenize(string input);


        /// <summary>
        /// Parses a token stream to a parse target.
        /// </summary>
        /// <param name="stream">The token stream.</param>
        /// <param name="target">The parse target.</param>
        /// <returns>The result.</returns>
        public ParseResult Parse(TokenStream stream, ICommandParseTarget target);


        /// <summary>
        /// Registers a parsing rule.
        /// </summary>
        /// <param name="ruleType">The type of the rule to register</param>
        public void RegisterParseRule(Type ruleType);


        /// <summary>
        /// Unregisters a parse rule.
        /// </summary>
        /// <param name="ruleType">The type of the rule to unregister.</param>
        public void UnregisterParseRule(Type ruleType);
    }
}
