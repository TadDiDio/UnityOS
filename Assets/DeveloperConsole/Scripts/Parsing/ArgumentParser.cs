using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Codice.CM.Common.Serialization;
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
            var errorResult = ParsePositionals();
            if (!errorResult.Success) return errorResult;
            
            errorResult = ParseRemaining();
            if (!errorResult.Success) return errorResult;
            
            errorResult = ValidateAttributes();
            return !errorResult.Success ? errorResult : new ArgumentParseResult { Success = true };
        }
        private ArgumentParseResult ParsePositionals()
        {
            foreach (var field in ReflectionParsing.GetPositionalArgFieldsInOrder(_commandType))
            {
                if (!_tokenStream.HasMore())
                {
                    return new ArgumentParseResult
                    {
                        Error = ArgumentParseError.MissingPositionalArg,
                        ErroneousField = field
                    };
                }
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
        private ArgumentParseResult ParseRemaining()
        {
            while (_tokenStream.HasMore())
            {
                if (!_tokenStream.Peek().StartsWith("-")) return SetVariadicArgs();

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
                    
                    PrependRemainingArgs(switchStream);
                    
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
            
            SetEmptyVariadicListIfPresent();
            
            return new ArgumentParseResult { Success = true };
        }

        private void SetEmptyVariadicListIfPresent()
        {
            var result = ReflectionParsing.GetVariadicArgsField(_commandType);
            if (!result.HasValue) return;
            
            var (field, type) = result.Value;
            try
            {
                Type constructedListType = typeof(List<>).MakeGenericType(type);
                object listInstance = Activator.CreateInstance(constructedListType);
                field.SetValue(_command, listInstance);
            }
            finally { }
        }
        private ArgumentParseResult SetVariadicArgs()
        {
            var result = ReflectionParsing.GetVariadicArgsField(_commandType);
            if (!result.HasValue)
            {
                return new ArgumentParseResult
                {
                    Error = ArgumentParseError.UnexpectedToken,
                    ErroneousToken = _tokenStream.Peek()
                };
            }

            var (field, type) = result.Value;

            try
            {
                Type constructedListType = typeof(List<>).MakeGenericType(type);
                object listInstance = Activator.CreateInstance(constructedListType);
                MethodInfo addMethod = constructedListType.GetMethod("Add");

                while (_tokenStream.HasMore())
                {
                    if (!TypeParserRegistry.TryParse(type, _tokenStream, out var obj))
                    {
                        return new ArgumentParseResult
                        {
                            Error = ArgumentParseError.TypeParseFailed,
                            ErroneousField = field,
                        };
                    }
                
                    addMethod.Invoke(listInstance, new[] { obj });
                }
            
                field.SetValue(_command, listInstance);
            
                return new ArgumentParseResult { Success = true };
            }
            catch
            {
                return new ArgumentParseResult { Error = ArgumentParseError.BadVariadicContainer };
            }
        }

        private void PrependRemainingArgs(TokenStream remaining)
        {
            List<string> tokens = new();
            tokens.AddRange(remaining.Remaining());
            tokens.AddRange(_tokenStream.Remaining());
            _tokenStream = new TokenStream(tokens);
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
        MissingPositionalArg,
        AttributeValidationError,
        UnrecognizedSwitch,
        MalformedSwitchName,
        DuplicateSwitch,
        UnexpectedToken,
        BadVariadicContainer
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