using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public class TypeParserRegistry : ITypeParserRegistryProvider
    {
        private Dictionary<Type, BaseTypeParser> _typeParsers = new();
        private IOutputManager _outputManager;
        
        public TypeParserRegistry(IOutputManager outputManager)
        {
            _outputManager = outputManager;
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
                _outputManager.SendOutput(MessageFormatter.Error($"There is no parser registered for type {type}."));
                return false;
            }

            return parser.TryParse(stream, out obj);
        }
    }
}