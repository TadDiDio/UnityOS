using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// Parses input strings to parse targets.
    /// </summary>
    public interface IParser
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
        public ParseResult Parse(TokenStream stream, IParseTarget target);
    }
}