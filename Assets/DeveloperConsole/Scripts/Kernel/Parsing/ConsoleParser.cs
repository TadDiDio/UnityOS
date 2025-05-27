using System;
using System.Linq;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class ConsoleParser
    {
        public static bool TryParse<T>(TokenStream stream, out T obj)
        {
            return TypeParserRegistry.TryParse(stream, out obj);
        }
        public static ParseResult Parse(List<string> tokens)
        {
            ParseResult result = new();

            // Check registry for command
            // TODO: Must be able to validate subcommand names here too. maybe flag included as arg in Parse() if is sub
            if (!ValidateCommandName(tokens[0], ref result, out ICommand command)) return result;
            
            // EARLY SUCCESS: Single command word with no args can be executed
            if (tokens.Count == 1)
            {
                result.Error = ParseError.None;
                result.Command = command;
                return result;
            }
            
            // Recursive subcommand analysis
            if (IsSubcommand(tokens[1], command.GetType())) return Parse(tokens.Skip(1).ToList());
            
            // Parse args
            if (!ParseArgs(command, tokens, ref result)) return result;
            
            result.Error = ParseError.None;
            result.Command = command;
            return result;
        }

        private static bool ValidateCommandName(string name, ref ParseResult result, out ICommand command)
        {
            result.CommandName = name;
            if (CommandRegistry.TryGetCommand(name, out command)) return true;

            result.Error = ParseError.InvalidCommandName;
            return false;
        }
        private static bool IsSubcommand(string name, Type type)
        {
            return type.GetFieldsWithAttribute<SubcommandAttribute>().HasSubcommandWithName(name);
        }

        private static bool ParseArgs(ICommand command, List<string> tokens, ref ParseResult result)
        {
            Type type = command.GetType();
            TokenStream tokenStream = new(tokens.Skip(1).ToList());
            
            // Validate positional args
            var positionalFields = type.GetFieldsWithAttribute<PositionalArgAttribute>().ToList();
            if (!ArgumentParsing.ValidatePositionalArgs(positionalFields, tokenStream))
            {
                result.Error = ParseError.InvalidPositionalArg; return false;
            }
            
            // Validate switches
            var switchFields = type.GetFieldsWithAttribute<SwitchArgAttribute>().ToList();
            if (!ArgumentParsing.ValidateSwitchArgs(switchFields, tokenStream))
            {
                result.Error = ParseError.InvalidSwitchError; return false;
            }

            // TODO: This is NOT finished
            
            return true;
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
        InvalidPositionalArg,
        InvalidSwitchError
    }
}