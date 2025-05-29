using System;
using System.Linq;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class ConsoleParser
    {
        public static bool TryParse(Type type, TokenStream stream, out object obj)
        {
            return TypeParserRegistry.TryParse(type, stream, out obj);
        }
        public static ParseResult Parse(List<string> tokens)
        {
            // Check registry for command
            // TODO: Must be able to validate subcommand names here too. maybe flag included as arg in Parse() if is sub
            if (!ValidateCommandName(tokens[0], out var result, out ICommand command)) return result;
            
            ReflectionParser reflectionParser = new ReflectionParser(command.GetType());
            
            // TODO: Untested
            // Recursive subcommand analysis
            if (tokens.Count > 1 && reflectionParser.HasSubcommandWithName(tokens[1]))
            {
                return Parse(tokens.Skip(1).ToList());
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