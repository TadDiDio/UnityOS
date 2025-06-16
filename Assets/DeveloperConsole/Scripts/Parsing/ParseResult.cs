using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Rules;

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
        /// The parse values.
        /// </summary>
        public Dictionary<ArgumentSpecification, object> Values;
        
        
        /// <summary>
        /// The error message if failed.
        /// </summary>
        public string ErrorMessage;
        
        
        /// <summary>
        /// Creates a successful parse result.
        /// </summary>
        /// <param name="values">The values that were parsed.</param>
        /// <returns>The result.</returns>
        public static ParseResult Success(Dictionary<ArgumentSpecification, object> values)
        {
            return new ParseResult
            {
                Status = Status.Success,
                Values = values,
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
                Status = Status.Fail,
                ErrorMessage = $"Parsing '{arg.Name}' as a {TypeFriendlyNames.TypeToName(arg.FieldInfo.FieldType)}" +
                               $" failed at '{errorToken}'"
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
                Status = Status.Fail,
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
                Status = Status.Fail,
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
                Status = Status.Fail,
                ErrorMessage = $"Saw an unexpected token: '{token}'"
            };
        }

        
        /// <summary>
        /// Creates a no args set result.
        /// </summary>
        /// <param name="rule">The offending rule.</param>
        /// <returns>The result.</returns>
        public static ParseResult NoArgSet(IParseRule rule)
        {
            return new ParseResult
            {
                Status = Status.Fail,
                ErrorMessage = $"No args were set by '{rule.GetType().Name}' despite parsing returing success."
            };
        }
        
        /// <summary>
        /// Creates a too many args result.
        /// </summary>
        /// <returns>The result.</returns>
        public static ParseResult TooManyArgs()
        {
            return new ParseResult
            {
                Status = Status.Fail,
                ErrorMessage = "Too many arguments attempting to set while applying long bool switch rule."
            };
        }
    }
}

