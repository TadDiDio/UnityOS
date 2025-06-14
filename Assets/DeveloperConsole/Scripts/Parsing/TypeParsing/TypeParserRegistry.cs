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
        
        public bool TryParse(Type type, TokenStream stream, out object obj)
        {
            obj = null;
            if (!_typeParsers.TryGetValue(type, out var parser))
            {
                Log.Warning($"There is no type parser of {type.Name}, Did you forget to register it?");
                return false;
            }

            return parser.TryParse(stream, out obj);
        }
    }
}