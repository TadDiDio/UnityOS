namespace DeveloperConsole.Parsing
{
    public class ParseResult
    {
        public Status Status;
        public object Value;
        public string ErrorMessage;
        
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
        public static ParseResult Finished()
        {
            return new ParseResult
            {
                Status = Status.Success,
                ErrorMessage = null
            };
        }
        
        public static ParseResult TypeParsingFailed()
        {
            return new ParseResult
            {
                ErrorMessage = "Failed to parse a type."
            };
        }
    }
}