using System;
using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class TypeAdapterRegistry : ITypeAdapterRegistry
    {
        private Dictionary<Type, ITypeAdapter> _adapters = new();

        public TypeAdapterRegistry()
        {
            // Register default type parsers
            RegisterAdapter<int>(new IntAdapter());
            RegisterAdapter<bool>(new BoolAdapter());
            RegisterAdapter<float>(new FloatAdapter());
            RegisterAdapter<Color>(new ColorAdapter());
            RegisterAdapter<Color>(new AlphaColorAdapter());
            RegisterAdapter<string>(new StringAdapter());
            RegisterAdapter<Type>(new TypeAdapter());
            RegisterAdapter<Vector2>(new Vector2Adapter());
            RegisterAdapter<Vector3>(new Vector3Adapter());
        }

        public void RegisterAdapter<T>(ITypeAdapter adapter)
        {
            _adapters[typeof(T)] = adapter;
        }

        public TypeAdaptResult AdaptFromStream(Type type, TokenStream stream)
        {
            return GetAdapter(type).AdaptFromTokens(stream);
        }

        public TypeAdaptResult AdaptFromString(Type type, string value, ITokenizer tokenizer = null)
        {
            return GetAdapter(type).ConvertFromString(value, tokenizer);
        }

        private ITypeAdapter GetAdapter(Type type)
        {
            if (!_adapters.TryGetValue(type, out var adapter))
            {
                throw new InvalidOperationException($"No type adapter registered for type {type.Name}");
            }
            return adapter;
        }
    }
}
