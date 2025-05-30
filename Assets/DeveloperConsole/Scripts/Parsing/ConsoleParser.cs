using System.Linq;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class ConsoleParser
    {
        public static ParseResult Parse(List<string> tokens)
        {
            return Parse(tokens, "");
        }

        private static ParseResult Parse(List<string> tokens, string parentName)
        {
            // Check registry for command
            string fullyQualifiedCommandName = parentName == "" ? tokens[0] : $"{parentName}.{tokens[0]}";
            if (!ValidateCommandName(fullyQualifiedCommandName, out var result, out ICommand command)) return result;
            
            ReflectionParser reflectionParser = new ReflectionParser(command);
            
            // Recursive subcommand analysis
            if (tokens.Count > 1 && reflectionParser.HasSubcommandWithSimpleName(tokens[1]))
            {
                return Parse(tokens.Skip(1).ToList(), fullyQualifiedCommandName);
            }
            
            // Parse args
            ArgumentParser argParser = new(command, tokens, reflectionParser);
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
        private static bool ValidateCommandName(string name, out ParseResult result, out ICommand command)
        {
            result = new ParseResult
            {
                CommandName = name
            };
            
            if (CommandRegistry.TryGetCommand(name, out command)) return true;

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