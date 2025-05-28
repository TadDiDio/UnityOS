using System;

namespace DeveloperConsole
{
    public static class ErrorLogging
    {
        public static string ParserError(ParseResult result)
        {
            var message = result.Error switch
            {
                ParseError.None => "An error was detected despite the parser not reporting one. This should not happen.",
                ParseError.InvalidCommandName => $"There is not command called {result.CommandName}.",
                ParseError.ArgumentError => result.ArgumentParseResult.Error switch
                {
                    ArgumentParseError.TypeParseFailed => $"Failed to parse {result.ArgumentParseResult.ErroneousField.Name} " +
                                                          $"as a {TypeFriendlyName(result.ArgumentParseResult.ErroneousField.FieldType)}",
                    ArgumentParseError.AttributeValidationError => $"Validation for attribute {result.ArgumentParseResult.ErroneousAttribute.GetType().Name}" +
                                                         $" failed for field {result.ArgumentParseResult.ErroneousField.Name}",
                    ArgumentParseError.UnrecognizedSwitch => $"Unrecognized switch {result.ArgumentParseResult.ErroneousToken}",
                    ArgumentParseError.MalformedSwitchName => $"Malformed switch name {result.ArgumentParseResult.ErroneousToken}. Switch names should not " +
                                                              $"include any special characters.",
                    ArgumentParseError.DuplicateSwitch => $"Duplicate switch {result.ArgumentParseResult.ErroneousToken}.",
                    ArgumentParseError.TooManyPositionalArgs => $"Too many positional arguments.",
                    _ => "Argument parsing error."
                },
                _ => "Parser Error",
            };
            
            return MessageFormatter.Error(message);
        }

        private static string TypeFriendlyName(Type type)
        {
            return type switch
            {
                null => "null",
                not null when type == typeof(int) => "int",
                not null when type == typeof(float) => "float",
                not null when type == typeof(bool) => "bool",
                not null when type == typeof(string) => "string",
                not null when type == typeof(double) => "double",
                not null when type == typeof(long) => "long",
                not null when type == typeof(short) => "short",
                not null when type == typeof(byte) => "byte",
                not null when type == typeof(char) => "char",
                _ => type.Name
            };
        }
    }
}