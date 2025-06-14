using System.Collections.Generic;
using System.Linq;

namespace DeveloperConsole.Parsing.Tokenizing
{
    /// <summary>
    /// Streams a list of tokens for easier processing.
    /// </summary>
    public class TokenStream
    {
        private readonly List<string> _tokens;
        private int _position;
        
        
        /// <summary>
        /// Creates a new token stream.
        /// </summary>
        /// <param name="tokens">The list of tokens to stream.</param>
        public TokenStream(List<string> tokens) => _tokens = tokens;
        
        
        /// <summary>
        /// Whether there are tokens remaining.
        /// </summary>
        /// <returns>True if there are more tokens to consume.</returns>
        public bool HasMore() => _position < _tokens.Count;
        
        
        /// <summary>
        /// The number of remaining tokens.
        /// </summary>
        /// <returns>The number of tokens.</returns>
        public int Count() => _tokens.Count - _position;
        
        
        /// <summary>
        /// Gets the next token without consuming it.
        /// </summary>
        /// <returns>The next token.</returns>
        public string Peek() => HasMore() ? _tokens[_position] : null;
        
        
        /// <summary>
        /// Gets and consumes the next token.
        /// </summary>
        /// <returns>The next token.</returns>
        public string Next() => HasMore() ? _tokens[_position++] : null;
        
        
        /// <summary>
        /// Gets a copy of all remaining tokens.
        /// </summary>
        /// <returns>All remaining tokens.</returns>
        public IEnumerable<string> Remaining() => _tokens.Skip(_position);
        
        
        /// <summary>
        /// Gets and consumes multiple tokens. If there are not enough tokens remaining,
        /// only the number present are returned.
        /// </summary>
        /// <param name="count">The number of tokens to read.</param>
        /// <returns>The read tokens.</returns>
        public IEnumerable<string> Read(int count)
        {
            var result = _tokens.Skip(_position).Take(count).ToList();
            _position += result.Count;
            return result;
        }
    }
}