using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class TypeParserRegistry
    {
        static TypeParserRegistry()
        {
            // TODO: Expose something here to allow users to inject their own
            IntParser intParser = new();
            FloatParser floatParser = new();
            StringParser stringParser = new();
            BoolParser boolParser = new();
            Vector2Parser vector2Parser = new();
            Vector3Parser vector3Parser = new();
            ColorParser colorParser = new();
            AlphaColorParser alphaColorParser = new();
            
            RegisterTypeParser(intParser);
            RegisterTypeParser(floatParser);
            RegisterTypeParser(stringParser);
            RegisterTypeParser(boolParser);
            RegisterTypeParser(vector2Parser);
            RegisterTypeParser(vector3Parser);
            RegisterTypeParser(colorParser);
            RegisterTypeParser(alphaColorParser);
        }
        
        private static Dictionary<Type, ITypeParser> _typeParsers = new();
        public static void RegisterTypeParser<T>(ITypeParser<T> parser) => _typeParsers.Add(typeof(T), parser);
        public static bool TryGetTypeParser<T>(out ITypeParser<T> parser)
        {
            parser = null;
            
            if (!_typeParsers.TryGetValue(typeof(T), out var raw)) return false;
            if (raw is not BaseTypeParser<T> casted) return false;
            
            parser = casted;
            return true;
        }
    }
}