using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole
{
    public static class TypeParserRegistry
    {
        private static Dictionary<Type, BaseTypeParser> _typeParsers = new();

        public static void Initialize()
        {
            StaticResetRegistry.Register(Clear);
        }

        private static void Clear()
        {
            _typeParsers.Clear();
        }
        
        public static void RegisterTypeParser<T>(BaseTypeParser parser)
        {
            _typeParsers.TryAdd(typeof(T), parser);
        }
        
        public static bool TryParse(Type type, TokenStream stream, out object obj)
        {
            obj = null;
            if (!_typeParsers.TryGetValue(type, out var parser))
            {
                OutputManager.SendOutput(MessageFormatter.Error($"There is no parser registered for type {type}."));
                return false;
            }

            return parser.TryParse(stream, out obj);
        }
    }
}