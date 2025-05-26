using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class Parser
    {
        public static ParseResult Parse(List<string> tokens)
        {
            ParseResult result = new ParseResult();
            
            // Validate command word with registry
            string name = tokens[0];
            result.CommandName = name;
            if (!CommandRegistry.TryGetCommand(name, out ICommand command))
            {
                result.Error = ParseError.InvalidCommandName;
                return result;
            }
            
            // Check for sub commands
            
            // Parse args

            result.Error = ParseError.None;
            result.Command = command;
            return result;
        }
    }
    
    public struct ParseResult
    {
        public ParseError Error;
        public ICommand Command;

        public string CommandName;
    }

    public enum ParseError
    {
        None,
        InvalidCommandName,
        
    }
}