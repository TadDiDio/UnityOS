using System;
using UnityEngine;
using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class TypeParserRegistry : ITypeParserRegistryProvider
    {
        private Dictionary<Type, BaseTypeParser> _typeParsers = new();
        
        public TypeParserRegistry()
        {
            // Register default type parsers
            RegisterTypeParser<int>(new IntParser());;
            RegisterTypeParser<bool>(new BoolParser());
            RegisterTypeParser<float>(new FloatParser());
            RegisterTypeParser<Color>(new ColorParser());
            RegisterTypeParser<string>(new StringParser());
            RegisterTypeParser<Type>(new TypeTypeParser());
            RegisterTypeParser<Vector2>(new Vector2Parser());
            RegisterTypeParser<Vector3>(new Vector3Parser());
        }
        
        public void RegisterTypeParser<T>(BaseTypeParser parser)
        {
            _typeParsers.TryAdd(typeof(T), parser);
        }
        
        public TypeParseResult TryParse(Type type, TokenStream stream)
        {
            if (!_typeParsers.TryGetValue(type, out var parser))
            {
                string error = $"There is no type parser of {type.Name}, Did you forget to register it?";
                Log.Warning(error);
                
                return new TypeParseResult
                {
                    Success = false,
                    Value = null,
                    ErrorMessage = error
                };
            }

            return parser.TryParseStream(stream);
        }
    }
}