using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeveloperConsole
{
    public class ArgumentParser
    {
        private ICommand _command;

        private TokenStream _tokenStream;
        private ReflectionParser _reflectionParser;
        
        private HashSet<FieldInfo> _fieldsSet = new(); 
        private HashSet<SwitchArgAttribute> _switchAttributesPresent = new(); 
        
        /// <summary>
        /// Creates a new ArgumentParser.
        /// </summary>
        /// <param name="command">The command to set up.</param>
        /// <param name="tokenStream">The token stream containing arguments but not the command name.</param>
        /// <param name="reflectionParser">A reflection parser initialized for the command type.</param>
        public ArgumentParser(ICommand command, TokenStream tokenStream, ReflectionParser reflectionParser)
        {
            _command = command;
            _tokenStream = tokenStream;
            _reflectionParser = reflectionParser;
            _command.RegisterTypeParsers();
        }

        public async Task<ArgumentParseResult> ParseAsync()
        {
            var errorResult = ParsePositionals();
            if (!errorResult.Success) return errorResult;
            
            errorResult = ParseRemaining();
            if (!errorResult.Success) return errorResult;
            
            errorResult = await ValidateAttributes();
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
                if (!Kernel.Instance.Get<ITypeParserRegistryProvider>().TryParse(field.FieldType, _tokenStream, out var obj))
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
                string token = _tokenStream.Peek();

                if (!IsSwitch(token))
                    return SetVariadicArgs();

                if (IsMalformedSwitch(token))
                    return MalformedSwitchResult(token);

                // Collect tokens representing this switch and its values
                var tokens = new List<string> { _tokenStream.Next() };
                var switchName = tokens[0];

                var switchFields = new List<FieldInfo>();

                if (IsMultipleShortSwitches(switchName, out var switches))
                {
                    var result = ParseMultipleShortSwitches(switchName, switches);
                    if (!result.Success)
                        return result.ErrorResult;

                    switchFields.AddRange(result.Fields);
                }
                else
                {
                    var result = ParseSingleSwitch(switchName, tokens);
                    if (!result.Success)
                        return result.ErrorResult;

                    switchFields.Add(result.Field);
                }

                var parseResult = ParseSwitchFieldValues(switchFields, tokens);
                if (!parseResult.Success)
                    return parseResult;
            }

            SetEmptyVariadicListIfPresent();

            return new ArgumentParseResult { Success = true };
        }
        
        private ArgumentParseResult MalformedSwitchResult(string token)
        {
            return new ArgumentParseResult
            {
                Error = ArgumentParseError.MalformedSwitchName,
                ErroneousToken = token
            };
        }
        
        private (bool Success, ArgumentParseResult ErrorResult, List<FieldInfo> Fields) ParseMultipleShortSwitches(string switchName, string switches)
        {
            var fields = new List<FieldInfo>();

            foreach (var option in switches)
            {
                var info = _reflectionParser.GetSwitchField(option.ToString());
                if (!info.HasValue)
                {
                    return (false, new ArgumentParseResult
                    {
                        ErroneousToken = option.ToString(),
                        Error = ArgumentParseError.UnrecognizedSwitch,
                    }, null);
                }

                var (field, attribute) = info.Value;

                if (!_switchAttributesPresent.Add(attribute))
                {
                    return (false, new ArgumentParseResult
                    {
                        Error = ArgumentParseError.DuplicateSwitch,
                        ErroneousField = field,
                        ErroneousAttribute = attribute,
                        ErroneousToken = switchName
                    }, null);
                }

                if (field.FieldType != typeof(bool))
                {
                    return (false, new ArgumentParseResult
                    {
                        ErroneousToken = option.ToString(),
                        Error = ArgumentParseError.NonBoolCoalescing,
                    }, null);
                }

                fields.Add(field);
            }

            return (true, new ArgumentParseResult {Success = true}, fields);
        }
        
        private (bool Success, ArgumentParseResult ErrorResult, FieldInfo Field) ParseSingleSwitch(string switchName, List<string> tokens)
        {
            var result = _reflectionParser.GetSwitchField(switchName);
            if (!result.HasValue)
            {
                return (false, new ArgumentParseResult
                {
                    ErroneousToken = switchName,
                    Error = ArgumentParseError.UnrecognizedSwitch,
                }, null);
            }

            var (field, attribute) = result.Value;

            if (!_switchAttributesPresent.Add(attribute))
            {
                return (false, new ArgumentParseResult
                {
                    Error = ArgumentParseError.DuplicateSwitch,
                    ErroneousField = field,
                    ErroneousAttribute = attribute,
                    ErroneousToken = switchName
                }, null);
            }

            bool isBoolSwitch = field.FieldType == typeof(bool);

            while (_tokenStream.HasMore())
            {
                string next = _tokenStream.Peek();

                if (isBoolSwitch)
                {
                    if (!bool.TryParse(next, out _)) break;
                }

                if (next.StartsWith("-") && !float.TryParse(next, out _))
                    break;

                tokens.Add(_tokenStream.Next());
            }

            return (true, new ArgumentParseResult {Success = true}, field);
        }
        
        private ArgumentParseResult ParseSwitchFieldValues(List<FieldInfo> switchFields, List<string> tokens)
        {
            foreach (var field in switchFields)
            {
                var switchStream = new TokenStream(tokens.Skip(1).ToList()); // skip switch name
                var remainingTokens = switchStream.Remaining().ToList();

                if (!Kernel.Instance.Get<ITypeParserRegistryProvider>().TryParse(field.FieldType, switchStream, out var obj))
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

            return new ArgumentParseResult { Success = true };
        }
        
        private bool IsSwitch(string token) => token.StartsWith("-");

        private bool IsMultipleShortSwitches(string token, out string switches)
        {
            switches = null;
            if (!IsSwitch(token)) return false;
            if (token.StartsWith("--")) return false;
            
            switches = token.TrimStart('-');
            
            return switches.Length > 1;
        }
            
        private bool IsMalformedSwitch(string token) => token.StartsWith("---");
        
        
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
                    if (!Kernel.Instance.Get<ITypeParserRegistryProvider>().TryParse(type, _tokenStream, out var obj))
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
        private async Task<ArgumentParseResult> ValidateAttributes()
        {
            var fields = _reflectionParser.GetAllFields();

            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes<ValidatedArgumentAsync>();
                foreach (var attribute in attributes)
                {
                    AttributeValidationData data = new()
                    {
                        FieldInfo = field,
                        Object = _command,
                        WasSet = _fieldsSet.Contains(field)
                    };
                    
                    if (!await attribute.ValidateAsync(data))
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
        BadVariadicContainer,
        NonBoolCoalescing
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