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
        
        
        // TODO: Parser checks errormessage null for success, transfer to status check
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
        
        
        /// TODO: More info needed here about what failed.
        /// <summary>
        /// Creates a failed parse result.
        /// </summary>
        /// <returns>The result.</returns>
        public static ParseResult TypeParsingFailed()
        {
            return new ParseResult
            {
                ErrorMessage = "Failed to parse a type."
            };
        }
    }
}

