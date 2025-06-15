using System;
using System.Collections.Generic;
using DeveloperConsole.Bindings;
using DeveloperConsole.Command;
using DeveloperConsole.Core;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using Object = UnityEngine.Object;

namespace DeveloperConsole
{
    /// <summary>
    /// Client facing API to interact with the developer console.
    /// </summary>
    public static class ConsoleAPI
    {
        private static TResult WithService<TService, TResult>(Func<TService, TResult> func, TResult fallback = default)
        where TService : class
        {
            if (!Kernel.IsInitialized) return fallback;
            if (!typeof(TService).IsInterface) throw new InvalidOperationException("TService must be an interface.");
            
            try
            {
                var service = Kernel.Instance.Get<TService>();
                return func(service);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return fallback;
            }
        }

        private static void WithService<TService>(Action<TService> action)
        where TService : class
        {
            if (!Kernel.IsInitialized) return;
            if (!typeof(TService).IsInterface) throw new InvalidOperationException("TService must be an interface.");

            try
            {
                var service = Kernel.Instance.Get<TService>();
                action(service);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
        
        
        // TODO: Convert all these to use WithService helper and also check that errors but not exceptions
        // return the correct defaults.
        
        
        
        /// <summary>
        /// Tries to get an object by checking if there is a binding, then searching for one if not.
        /// </summary>
        /// <param name="type">The type of object to get.</param>
        /// <param name="name">The name to find if there isn't an object already bound. Ignored if empty.</param>
        /// <param name="tag">The tag to find if there isn't an object already bound. Ignored if empty.</param>
        /// <param name="obj">The bound object.</param>
        /// <returns>True if a bound object was found in the cache or scene.</returns>
        public static bool TryGetBinding(Type type, string name, string tag, out Object obj)
        {
            obj = null;
            if (!Kernel.IsInitialized) return false;
            var bindingsProvider = Kernel.Instance.Get<IObjectBindingsManager>();
            
            try
            {
                var result = bindingsProvider.TryGetBinding(type, name, tag, out obj);
                return result;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
        }
        
        
        /// <summary>
        /// Binds an object for commands to reference.
        /// </summary>
        /// <param name="objType">The type to bind.</param>
        /// <param name="name">The name to search for. Ignored if empty.</param>
        /// <param name="tag">The tag to search for. Ignored if empty.</param>
        /// <returns>The bound object or null if one wasn't found.</returns>
        public static Object ResolveBinding(Type objType, string name, string tag)
        {
            if (!Kernel.IsInitialized) return null;
            var bindingsProvider = Kernel.Instance.Get<IObjectBindingsManager>();
            
            try
            {
                var result = bindingsProvider.ResolveBinding(objType, name, tag);
                return result;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return null;
            }
        }

        
        /// <summary>
        /// Gets all current bindings.
        /// </summary>
        /// <returns>The bindings table.</returns>
        public static Dictionary<Type, Object> GetAllBindings()
        {
            if (!Kernel.IsInitialized) return null;
            var bindingsProvider = Kernel.Instance.Get<IObjectBindingsManager>();
            
            try
            {
                return bindingsProvider.GetAllBindings();
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return null;
            }
        }
        

        /// <summary>
        /// A container for parsing related API calls.
        /// </summary>
        public static class Parsing
        {
            /// <summary>
            /// Tokenizes a string.
            /// </summary>
            /// <param name="input">The input to tokenize.</param>
            /// <returns>The result.</returns>
            public static TokenizationResult Tokenize(string input)
            {
                return WithService<IParser, TokenizationResult>(p => p.Tokenize(input));
            }
            
            
            /// <summary>
            /// Parses a token stream into the target.
            /// </summary>
            /// <param name="stream">The stream to parse.</param>
            /// <param name="target">The target to parse to.</param>
            /// <returns>The result.</returns>
            public static ParseResult Parse(TokenStream stream, IParseTarget target)
            {
                return WithService<IParser, ParseResult>(p => p.Parse(stream, target));
            }
            
            
            /// <summary>
            /// Tries to parse a stream of tokens into a specific type.
            /// Only tokens which are parsed correctly are consumed.
            /// </summary>
            /// <param name="targetType"></param>
            /// <param name="stream"></param>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static TypeParseResult TryParseType(Type targetType, TokenStream stream)
            {
                return WithService<ITypeParserRegistryProvider, TypeParseResult>
                    (r => r.TryParse(targetType, stream));
            }
            
            
            /// <summary>
            /// Registers a new type parser.
            /// </summary>
            /// <param name="parser">The parser to register.</param>
            /// <typeparam name="T">The type it parses.</typeparam>
            public static void RegisterTypeParser<T>(BaseTypeParser parser)
            {
                WithService<ITypeParserRegistryProvider>(r => r.RegisterTypeParser<T>(parser));
            }
        }

        
        /// <summary>
        /// A container for command related API calls.
        /// </summary>
        public static class Commands
        {
            /// <summary>
            /// Tries to resolve a fully qualified command name to a schema.
            /// </summary>
            /// <param name="fullyQualifiedName">The name.</param>
            /// <param name="schema">The matching schema.</param>
            /// <returns>True if there is a matching schema.</returns>
            public static bool TryResolveCommandSchema(string fullyQualifiedName, out CommandSchema schema)
            {
                CommandSchema commandSchema = null;
                var success = WithService<ICommandRegistry, bool>(r => r.TryResolveCommandSchema(fullyQualifiedName, out commandSchema));

                schema = commandSchema;
                return success;
            }
          
            
            /// <summary>
            /// Tries to resolve a list of tokens to the deepest valid schema in a nested hierarchy.
            /// </summary>
            /// <param name="tokens">The tokens.</param>
            /// <param name="schema">The matching schema.</param>
            /// <returns>True if there is a matching schema.</returns>
            public static bool TryResolveCommandSchema(List<string> tokens, out CommandSchema schema)
            {
                CommandSchema commandSchema = null;
                var success = WithService<ICommandRegistry, bool>(r => r.TryResolveCommandSchema(tokens, out commandSchema));

                schema = commandSchema;
                return success;
            }
     
            
            /// <summary>
            /// Tests if there is a command with the given name.
            /// </summary>
            /// <param name="fullyQualifiedName">The name.</param>
            /// <returns>True if there is.</returns>
            public static bool IsValidCommand(string fullyQualifiedName)
            {
                return WithService<ICommandRegistry, bool>(r => r.TryResolveCommandSchema(fullyQualifiedName, out _));
            }
            
            
            // TODO: Make this follow the Try pattern the others all use in this class
            /// <summary>
            /// Gets a command description from a name.
            /// </summary>
            /// <param name="fullyQualifiedName">The name.</param>
            /// <returns>The description.</returns>
            public static string GetCommandDescription(string fullyQualifiedName)
            {
                return WithService<ICommandRegistry, string>(r =>
                    r.TryResolveCommandSchema(fullyQualifiedName, out var schema) ? schema.Description : ""
                );
            }
            
            
            // TODO: No longer accurate
            /// <summary>
            /// Gets all command names.
            /// </summary>
            /// <returns>A list of all names.</returns>
            public static List<string> GetBaseCommandNames()
            {
                return WithService<ICommandRegistry, List<string>>(r => r.AllCommandNames());
            }
        }
    }
}