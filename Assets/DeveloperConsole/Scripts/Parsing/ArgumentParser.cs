using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public class ArgumentParser
    {
        private ICommand _command;
        private Type _commandType;

        private TokenStream _tokenStream;
        private ReflectionParser _reflectionParser;
        
        private HashSet<FieldInfo> _fieldsSet = new(); 
        private HashSet<SwitchArgAttribute> _switchAttributesPresent = new(); 
        
        public ArgumentParser(ICommand command, List<string> tokens, ReflectionParser reflectionParser)
        {
            _command = command;
            _commandType = command.GetType();
            _tokenStream = new TokenStream(tokens.Skip(1).ToList()); // Skip command name
            _reflectionParser = reflectionParser;
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
            foreach (var field in _reflectionParser.GetPositionalArgFieldsInOrder())
            {
                if (!_tokenStream.HasMore())
                {
                    return new ArgumentParseResult
                    {
                        Error = ArgumentParseError.MissingPositionalArg,
                        ErroneousField = field
                    };
                }
                
                List<string> remainingTokens = _tokenStream.Remaining().ToList();
                if (!TypeParserRegistry.TryParse(field.FieldType, _tokenStream, out var obj))
                {
                    int tokensUsed = remainingTokens.Count - _tokenStream.Count();
                    string failureToken = string.Join(", ", remainingTokens.Take(tokensUsed));
                    return new ArgumentParseResult
                    {
                        Error = ArgumentParseError.TypeParseFailed,
                        ErroneousField = field,
                        ErroneousToken = failureToken
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
                
                string switchName = tokens[0];
                switchName = switchName.TrimStart('-');
                switchName = switchName.Length == 1 ? $"-{switchName}" : $"--{switchName}";

                // Get the field associated with the switch
                var result = _reflectionParser.GetSwitchField(switchName);

                bool isBoolSwitch;
                if (!result.HasValue)
                {
                    return new ArgumentParseResult
                    {
                        ErroneousToken = tokens[0],
                        Error = ArgumentParseError.UnrecognizedSwitch,
                    };
                }
                
                var (field, attribute) = result.Value;
                isBoolSwitch = field.FieldType == typeof(bool);
                    
                while (_tokenStream.HasMore())
                {
                    string next = _tokenStream.Peek();
                    
                    // Only attempt to grab a single token if its a bool switch,
                    // and if its not true or false, don't consume it
                    if (isBoolSwitch)
                    {
                        if (!bool.TryParse(next, out bool _)) break;
                    }
                    
                    // Don't consider negative numbers to be switches
                    if (next.StartsWith("-"))
                    {
                        if (!float.TryParse(next, out float _)) break;
                    };

                    tokens.Add(_tokenStream.Next());
                }
               
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
                List<string> remainingTokens = switchStream.Remaining().ToList();
                if (!TypeParserRegistry.TryParse(field.FieldType, switchStream, out var obj))
                {
                    int tokensUsed = remainingTokens.Count - switchStream.Count();
                    string failureToken = string.Join(", ", remainingTokens.Take(tokensUsed));
                    return new ArgumentParseResult
                    {
                        Error = ArgumentParseError.TypeParseFailed,
                        ErroneousField = field,
                        ErroneousToken = failureToken
                    };
                }
                
                PrependRemainingArgs(switchStream);
                
                field.SetValue(_command, obj);
                _fieldsSet.Add(field);
            }
            
            SetEmptyVariadicListIfPresent();
            
            return new ArgumentParseResult { Success = true };
        }
        private void SetEmptyVariadicListIfPresent()
        {
            var result = _reflectionParser.GetVariadicArgsField(out bool _);
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
            var result = _reflectionParser.GetVariadicArgsField(out bool badContainer);
            if (!result.HasValue)
            {
                if (badContainer)
                {
                    return new ArgumentParseResult { Error = ArgumentParseError.BadVariadicContainer };
                }
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
                    List<string> remainingTokens = _tokenStream.Remaining().ToList();
                    if (!TypeParserRegistry.TryParse(type, _tokenStream, out var obj))
                    {
                        int tokensUsed = remainingTokens.Count - _tokenStream.Count();
                        string failureToken = string.Join(", ", remainingTokens.Take(tokensUsed));
                        return new ArgumentParseResult
                        {
                            Error = ArgumentParseError.TypeParseFailed,
                            ErroneousField = field,
                            ErroneousToken = failureToken
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
            var fields = _reflectionParser.GetAllFields();

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