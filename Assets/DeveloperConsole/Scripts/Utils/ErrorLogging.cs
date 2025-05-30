using System;

namespace DeveloperConsole
{
    public static class ErrorLogging
    {
        public static string ParserError(ParseResult result)
        {
            string validationError = "";
            if (result.ArgumentParseResult.ErroneousAttribute is ValidatedAttribute validated)
            {
                validationError = $": {validated.ErrorMessage()}";
            }
            var message = result.Error switch
            {
                ParseError.None => "An error was detected despite the parser not reporting one. This should not happen.",
                ParseError.InvalidCommandName => $"There is not a command called {result.CommandName}.",
                ParseError.ArgumentError => result.ArgumentParseResult.Error switch
                {
                    ArgumentParseError.TypeParseFailed => $"Failed to parse '{result.ArgumentParseResult.ErroneousToken}' " +
                                                          $"({result.ArgumentParseResult.ErroneousField.Name}) " +
                                                          $"as a {TypeFriendlyName(result.ArgumentParseResult.ErroneousField.FieldType)}",
                    ArgumentParseError.AttributeValidationError =>
                                                          $"Validation for attribute [{result.ArgumentParseResult.ErroneousAttribute.GetType().Name}]" +
                                                          $" failed for field [{result.ArgumentParseResult.ErroneousField.Name}]{validationError}",

                    ArgumentParseError.UnrecognizedSwitch =>   $"Unrecognized switch '{result.ArgumentParseResult.ErroneousToken}'",
                    ArgumentParseError.MalformedSwitchName =>  $"Malformed switch name '{result.ArgumentParseResult.ErroneousToken}'. Switch names should not " +
                                                               $"include any special characters.",
                    ArgumentParseError.DuplicateSwitch =>      $"Duplicate switch [{result.ArgumentParseResult.ErroneousToken}].",
                    ArgumentParseError.MissingPositionalArg => $"Missing positional argument [{result.ArgumentParseResult.ErroneousField.Name}]",
                    ArgumentParseError.UnexpectedToken =>      $"Unexpected token \'{result.ArgumentParseResult.ErroneousToken}\'.",
                    ArgumentParseError.BadVariadicContainer => "No proper variadic args container field found. This must be a List<T> where T is the type" +
                        "of the remaining args. Use string if you want to perform your own casting.",
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