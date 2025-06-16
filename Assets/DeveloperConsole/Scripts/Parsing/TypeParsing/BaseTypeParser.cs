using System.Linq;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// Parses a particular type.
    /// </summary>
    public abstract class BaseTypeParser
    {
        /// <summary>
        /// Parses a token stream into an object.
        /// </summary>
        /// <param name="streamCopy">The stream.</param>
        /// <param name="obj">The result object.</param>
        /// <returns>True if successful.</returns>
        protected abstract bool TryParse(TokenStream streamCopy, out object obj);

        
        /// <summary>
        /// Parses a stream into a value. Tokens are only consumed if the entire parse succeeds, otherwise no
        /// token is consumed.
        /// </summary>
        /// <param name="tokenStream">The stream.</param>
        /// <returns>The result.</returns>
        public TypeParseResult TryParseStream(TokenStream tokenStream)
        {
            TokenStream copy = new TokenStream(tokenStream.Remaining().ToList());

            bool success = TryParse(copy, out object obj);
            int numConsumed = tokenStream.Count() - copy.Count();
            
            if (success)
            {
                // Only consume actual tokens on success
                tokenStream.Read(numConsumed);    
                
                return new TypeParseResult
                {
                    Success = true,
                    Value = obj,
                    ErrorMessage = null
                };
            }

            return new TypeParseResult
            {
                Success = false,
                Value = null,
                ErrorMessage = string.Join(", ", copy.Read(numConsumed).ToList())
            };
        }
    }
    

    /// <summary>
    /// Holds data regarding the result of a type parsing.
    /// </summary>
    public struct TypeParseResult
    {
        public bool Success;
        public object Value;
        public string ErrorMessage;
    }
}