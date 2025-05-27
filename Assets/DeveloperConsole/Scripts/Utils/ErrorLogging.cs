namespace DeveloperConsole
{
    public static class ErrorLogging
    {
        public static string ParserError(ParseResult result)
        {
            var message = result.Error switch
            {
                ParseError.InvalidCommandName => $"There is not command called {result.CommandName}.",
                _ => "",
            };
            
            return MessageFormatter.Error(message);
        }
    }
}