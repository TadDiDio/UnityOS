namespace DeveloperConsole
{
    public class ConsoleParser : IConsoleParser
    {
        private ICommandRegistryProvider _commandRegistry;

        public ConsoleParser(ICommandRegistryProvider commandRegistry)
        {
            _commandRegistry = commandRegistry;
        }
        
        public ParseResult Parse(TokenStream tokenStream)
        {
            return Parse(tokenStream, "");
        }

        private ParseResult Parse(TokenStream tokenStream, string parentName)
        {
            // Check registry for command
            string fullyQualifiedCommandName = parentName == "" ? tokenStream.Peek() : $"{parentName}.{tokenStream.Peek()}";
            if (!ValidateCommandName(fullyQualifiedCommandName, out var result, out ICommand command)) return result;
            
            ReflectionParser reflectionParser = new ReflectionParser(command);
            
            // Recursive subcommand analysis
            tokenStream.Next();
            if (tokenStream.HasMore() && reflectionParser.HasSubcommandWithSimpleName(tokenStream.Peek()))
            {
                return Parse(tokenStream, fullyQualifiedCommandName);
            }
            
            // Parse args
            ArgumentParser argParser = new(command, tokenStream, reflectionParser);
            var argumentParseResult = argParser.Parse();
            if (!argumentParseResult.Success)
            {
                result.ArgumentParseResult = argumentParseResult;
                result.Error = ParseError.ArgumentError;
                return result;
            }
            
            result.Error = ParseError.None;
            result.Command = command;
            return result;
        }
        private bool ValidateCommandName(string name, out ParseResult result, out ICommand command)
        {
            result = new ParseResult
            {
                CommandName = name
            };
            
            if (_commandRegistry.TryGetCommand(name, out command)) return true;

            result.Error = ParseError.InvalidCommandName;
            return false;
        }
    }
    
    public struct ParseResult
    {
        public ParseError Error;
        public ArgumentParseResult ArgumentParseResult;
        
        public ICommand Command;
        public string CommandName;
    }

    public enum ParseError
    {
        None,
        InvalidCommandName,
        ArgumentError
    }
}