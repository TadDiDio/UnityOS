using DeveloperConsole.Command;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// Stores the result of a parse. Represents both single type parser results and entire parse results.
    /// </summary>
    public class ParseResult
    {
        /// <summary>
        /// The failure status.
        /// </summary>
        public Status Status;
        
        
        /// <summary>
        /// The value of the parse.
        /// </summary>
        public object Value;
        
        
        /// <summary>
        /// The error message if failed.
        /// </summary>
        public string ErrorMessage;
        
        
        /// <summary>
        /// Creates a successful parse result.
        /// </summary>
        /// <param name="value">The value that was parsed.</param>
        /// <returns>The result.</returns>
        public static ParseResult Success(object value)
        {
            return new ParseResult
            {
                Status = Status.Success,
                Value = value,
                ErrorMessage = null
            };
        }
        
        
        /// <summary>
        /// Creates a finished parse result.
        /// </summary>
        /// <returns>The result.</returns>
        public static ParseResult Finished()
        {
            return new ParseResult
            {
                Status = Status.Success,
                ErrorMessage = null
            };
        }
        
        
        /// <summary>
        /// Creates a result reflecting a failed type parsing.
        /// </summary>
        /// <param name="errorToken">The token that caused the error.</param>
        /// <param name="arg">The current arg.</param>
        /// <returns>The result.</returns>
        public static ParseResult TypeParsingFailed(string errorToken, ArgumentSpecification arg)
        {
            return new ParseResult
            {
                ErrorMessage = $"Failed to parse '{errorToken}' ({arg.Name}) as a " +
                               $"{TypeFriendlyNames.TypeToName(arg.FieldInfo.FieldType)}",
            };
        }
        
        
        /// <summary>
        /// Creates a result reflecting a token not consumed error.
        /// </summary>
        /// <param name="errorToken">The token that caused the error.</param>
        /// <param name="arg">The current arg.</param>
        /// <returns>The result.</returns>
        public static ParseResult TokenNotConsumed(string errorToken, ArgumentSpecification arg)
        {
            return new ParseResult
            {
                ErrorMessage = $"The token '{errorToken}' ({arg.Name}) was not consumed by " +
                               $"{TypeFriendlyNames.TypeToName(arg.FieldInfo.FieldType)} but " +
                               $"should have been.",
            };
        }
        
        
        /// <summary>
        /// Creates a failed parse result.
        /// </summary>
        /// <param name="message">The failure message.</param>
        /// <returns>The result.</returns>
        public static ParseResult ValidationFailed(string message)
        {
            return new ParseResult
            {
                ErrorMessage = message
            };
        }
        
        
        /// <summary>
        /// Creates a failed parse result.
        /// </summary>
        /// <param name="token">The unexpected token.</param>
        /// <returns>The result.</returns>
        public static ParseResult UnexpectedToken(string token)
        {
            return new ParseResult
            {
                ErrorMessage = $"Saw an unexpected token: '{token}'"
            };
        }
    }
}

