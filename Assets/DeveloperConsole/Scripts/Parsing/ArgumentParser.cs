using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole
{
    public class ArgumentParser
    {
        private ICommand _command;
        private Type _commandType;
        
        private TokenStream _tokenStream;

        private HashSet<FieldInfo> _fieldsSet = new(); 
        private HashSet<SwitchArgAttribute> _switchAttributesPresent = new(); 
        
        public ArgumentParser(ICommand command, List<string> tokens)
        {
            _command = command;
            _commandType = command.GetType();
            _tokenStream = new TokenStream(tokens.Skip(1).ToList()); // Skip command name
        }

        public ArgumentParseResult Parse()
        {
            ArgumentParseResult errorResult = new();
            
            // First parse positionals, they must appear first
            errorResult = ParsePositionals();
            if (!errorResult.Success) return errorResult;
            
            // Then parse existing switches
            errorResult = ParseSwitches();
            if (!errorResult.Success) return errorResult;
            
            // Finally check all validated attributes
            errorResult = ValidateAttributes();
            return !errorResult.Success ? errorResult : new ArgumentParseResult { Success = true };
        }
        private ArgumentParseResult ParsePositionals()
        {
            foreach (var field in ReflectionParsing.GetPositionalArgFieldsInOrder(_commandType))
            {
                if (!TypeParserRegistry.TryParse(field.FieldType, _tokenStream, out var obj))
                {
                    return new ArgumentParseResult
                    {
                        Error = ArgumentParseError.TypeParseFailed,
                        ErroneousField = field
                    };
                }

                field.SetValue(_command, obj);
                _fieldsSet.Add(field);
            }
            
            return new ArgumentParseResult { Success = true };
        }
        
        private ArgumentParseResult ParseSwitches()
        {
            while (_tokenStream.HasMore())
            {
                if (!_tokenStream.Peek().StartsWith("-"))
                {
                    return new ArgumentParseResult
                    {
                        Error = ArgumentParseError.TooManyPositionalArgs,
                        ErroneousToken = _tokenStream.Peek()
                    };
                }

                if (_tokenStream.Peek().StartsWith("---"))
                {
                    return new ArgumentParseResult
                    {
                        Error = ArgumentParseError.MalformedSwitchName,
                        ErroneousToken = _tokenStream.Peek()
                    };
                }

                List<string> tokens = new() { _tokenStream.Next() };

                while (_tokenStream.HasMore())
                {
                    string next = _tokenStream.Peek();
                    if (next.StartsWith("-")) break;

                    tokens.Add(_tokenStream.Next());
                }
                
                string switchName = tokens[0];
                switchName = switchName.TrimStart('-');
                switchName = switchName.Length == 1 ? $"-{switchName}" : $"--{switchName}";

                // Get the field associated with the switch
                var result = ReflectionParsing.GetSwitchField(_commandType, switchName);

                if (result.HasValue)
                {
                    var (field, attribute) = result.Value;

                    if (!_switchAttributesPresent.Add(attribute))
                    {
                        return new ArgumentParseResult
                        {
                            Error = ArgumentParseError.DuplicateSwitch,
                            ErroneousField = field,
                            ErroneousAttribute = attribute,
                            ErroneousToken = switchName
                        };
                    }

                    // Parse the value
                    TokenStream switchStream = new(tokens.Skip(1).ToList()); // Skip switch name
                    if (!TypeParserRegistry.TryParse(field.FieldType, switchStream, out var obj))
                    {
                        return new ArgumentParseResult
                        {
                            Error = ArgumentParseError.TypeParseFailed,
                            ErroneousField = field,
                        };
                    }
                
                    field.SetValue(_command, obj);
                    _fieldsSet.Add(field);
                }
                else
                {
                    return new ArgumentParseResult
                    {
                        ErroneousToken = tokens[0],
                        Error = ArgumentParseError.UnrecognizedSwitch,
                    };
                }
            }
            
            return new ArgumentParseResult { Success = true };
        }

        private ArgumentParseResult ValidateAttributes()
        {
            var fields = ReflectionParsing.GetAllFields(_commandType);

            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes<ValidatedAttribute>();
                foreach (var attribute in attributes)
                {
                    AttributeValidationData data = new()
                    {
                        FieldInfo = field,
                        Object = _command,
                        WasSet = _fieldsSet.Contains(field)
                    };
                    if (!attribute.Validate(data))
                    {
                        return new ArgumentParseResult
                        {
                            Error = ArgumentParseError.AttributeValidationError,
                            ErroneousAttribute = attribute,
                            ErroneousField = field
                        };
                    }
                }
            }

            return new ArgumentParseResult { Success = true };
        }
    }

    public enum ArgumentParseError
    {
        TypeParseFailed,
        AttributeValidationError,
        UnrecognizedSwitch,
        MalformedSwitchName,
        DuplicateSwitch,
        TooManyPositionalArgs,
    }
    public struct ArgumentParseResult
    {
        public bool Success;
        public ArgumentParseError Error;
        public FieldInfo ErroneousField;
        public Attribute ErroneousAttribute;
        public string ErroneousToken;
    }
}