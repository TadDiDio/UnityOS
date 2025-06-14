using System.Collections.Generic;

namespace DeveloperConsole.Parsing.Tokenizing
{
    /// <summary>
    /// Tokenizes input strings.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Splits an input string into a list of tokens.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The result.</returns>
        public TokenizationResult Tokenize(string input);
    }

    
    /// <summary>
    /// Holds tokenization result info.
    /// </summary>
    public struct TokenizationResult
    {
        /// <summary>
        /// The failure status.
        /// </summary>
        public TokenizationStatus Status;
        
        
        /// <summary>
        /// The tokens. Null if failed.
        /// </summary>
        public List<string> Tokens;

        
        /// <summary>
        /// Creates and empty tokenization result.
        /// </summary>
        /// <returns>The result.</returns>
        public static TokenizationResult Empty()
        {
            return new TokenizationResult
            {
                Tokens = null,
                Status = TokenizationStatus.Empty
            };
        }
        
        
        /// <summary>
        /// Creates a successful tokenization result.
        /// </summary>
        /// <param name="tokens">The resulting tokens.</param>
        /// <returns>The result.</returns>
        public static TokenizationResult Success(List<string> tokens)
        {
            return new TokenizationResult
            {
                Tokens = tokens,
                Status = TokenizationStatus.Success
            };
        }
    }

    
    /// <summary>
    /// The failure status of a tokenization.
    /// </summary>
    public enum TokenizationStatus
    {
        Empty,
        Success,
    }
}