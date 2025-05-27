using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class TypeParserRegistry
    {
        private static Dictionary<Type, ITypeParser> _typeParsers = new();
        public static void RegisterTypeParser<T>(BaseTypeParser<T> parser)
        {
            _typeParsers.TryAdd(typeof(T), parser);
        }
        
        public static bool TryParse<T>(TokenStream stream, out T obj)
        {
            obj = default;
            if (!_typeParsers.TryGetValue(typeof(T), out var raw))
            {
                // TODO: Error, no parser registered for T
                return false;
            }
            
            return raw is BaseTypeParser<T> parser && parser.TryParse(stream, out obj);
        }
    }
}