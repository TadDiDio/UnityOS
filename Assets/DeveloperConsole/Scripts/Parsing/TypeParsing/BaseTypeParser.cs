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
        /// <param name="tokenStream">The stream.</param>
        /// <param name="obj">The result object.</param>
        /// <returns>True if successful.</returns>
        protected abstract bool TryParse(TokenStream tokenStream, out object obj);

        
        /// <summary>
        /// Parses a stream into a value.
        /// </summary>
        /// <param name="tokenStream">The stream.</param>
        /// <returns>The result.</returns>
        public TypeParseResult TryParseStream(TokenStream tokenStream)
        {
            TokenStream copy = new TokenStream(tokenStream.Remaining().ToList());

            // TODO: Not sure if its better to provide the copy here...
            if (TryParse(tokenStream, out object obj))
            {
                return new TypeParseResult
                {
                    Success = true,
                    Value = obj,
                    ErrorMessage = null
                };
            }

            int consumed = copy.Count() - tokenStream.Count();
            return new TypeParseResult
            {
                Success = false,
                Value = null,
                ErrorMessage = string.Join(" ", copy.Read(consumed).ToList())
            };
        }
    }

    public struct TypeParseResult
    {
        public bool Success;
        public object Value;
        public string ErrorMessage;
    }
}