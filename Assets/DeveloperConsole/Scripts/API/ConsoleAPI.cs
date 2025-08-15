using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeveloperConsole.Bindings;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Kernel;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.IO;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;
using DeveloperConsole.Windowing;
using Object = UnityEngine.Object;

namespace DeveloperConsole
{
    /// <summary>
    /// Client facing API to interact with the developer console.
    /// </summary>
    public static class ConsoleAPI
    {
        private static TResult WithService<TService, TResult>(Func<TService, TResult> func)
        where TService : class
        {
            if (!Kernel.IsInitialized) throw new InvalidOperationException("Kernel is not initialized yet a service was requested from it.");
            if (!typeof(TService).IsInterface) throw new InvalidOperationException("TService must be an interface.");

            try
            {
                var service = Kernel.Instance.Get<TService>();
                return func(service);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return default;
            }
        }

        private static void WithService<TService>(Action<TService> action)
        where TService : class
        {
            if (!Kernel.IsInitialized) throw new InvalidOperationException("Kernel is not initialized yet a service was requested from it.");
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

        public static class Bindings
        {
            /// <summary>
            /// Returns all current bindings.
            /// </summary>
            /// <returns>All current bindings.</returns>
            public static ReadOnlyDictionary<Type, object> GetAllBindings()
            {
                return WithService<IObjectBindingsManager, ReadOnlyDictionary<Type, object>>(m => m.GetAllBindings());
            }


            /// <summary>
            /// Binds an object for future commands to use.
            /// </summary>
            /// <param name="type">The type to bind for.</param>
            /// <param name="obj">The object instance to bind to.</param>
            public static void BindObject(Type type, object obj)
            {
                WithService<IObjectBindingsManager>(m => m.BindObject(type, obj));
            }


            /// <summary>
            /// Tries to get a Unity object by searching the scene with the matching parameters.
            /// </summary>
            /// <param name="type">The type of object to get.</param>
            /// <param name="name">The name to find if there isn't an object already bound. Ignored if empty.</param>
            /// <param name="tag">The tag to find if there isn't an object already bound. Ignored if empty.</param>
            /// <param name="obj">The bound object.</param>
            /// <returns>True if a bound object was found in the cache or scene.</returns>
            public static bool TryGetUnityObjectBinding(Type type, string name, string tag, out Object obj)
            {
                Object result = null;

                bool success = WithService<IObjectBindingsManager, bool>(m => m.TryGetUnityObjectBinding(type, name, tag, out result));
                obj = result;

                return success;
            }


            /// <summary>
            /// Tries to get a plain C# object from the cache.
            /// </summary>
            /// <param name="type">The type to get.</param>
            /// <param name="obj">The bound object.</param>
            /// <returns>True if a cached object exists.</returns>
            public static bool TryGetPlainCSharpBinding(Type type, out object obj)
            {
                object result = null;

                bool success = WithService<IObjectBindingsManager, bool>(m => m.TryGetPlainCSharpBinding(type, out result));
                obj = result;

                return success;
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
                return WithService<ICommandParser, TokenizationResult>(p => p.Tokenize(input));
            }


            /// <summary>
            /// Parses a token stream into the command target.
            /// </summary>
            /// <param name="stream">The stream to parse.</param>
            /// <param name="target">The target to parse to.</param>
            /// <returns>The result.</returns>
            public static ParseResult ParseCommand(TokenStream stream, CommandParseTarget target)
            {
                return WithService<ICommandParser, ParseResult>(p => p.Parse(stream, target));
            }


            /// <summary>
            /// Adapts a stream to a type.
            /// </summary>
            /// <param name="type">The target type.</param>
            /// <param name="stream">The input stream.</param>
            /// <returns>The adaptation result.</returns>
            public static TypeAdaptResult AdaptTypeFromStream(Type type, TokenStream stream)
            {
                return WithService<ITypeAdapterRegistry, TypeAdaptResult>(r => r.AdaptFromStream(type, stream));
            }


            /// <summary>
            /// Adapts a string to a type.
            /// </summary>
            /// <param name="type">The target type.</param>
            /// <param name="input">The input string.</param>
            /// <param name="tokenizer">An optional tokenizer override.</param>
            /// <returns>The adaptation result.</returns>
            public static TypeAdaptResult AdaptTypeFromString(Type type, string input, ITokenizer tokenizer = null)
            {
                return WithService<ITypeAdapterRegistry, TypeAdaptResult>(r =>
                    r.AdaptFromString(type, input, tokenizer));
            }


            /// <summary>
            /// Tells if the type can be adapted by the currently registered adapters.
            /// </summary>
            /// <param name="type">The type to adapt.</param>
            /// <returns>True if it can be.</returns>
            public static bool CanAdaptType(Type type)
            {
                return WithService<ITypeAdapterRegistry, bool>(r => r.CanAdaptType(type));
            }


            /// <summary>
            /// Registers a type adapter. Safe to call multiple times.
            /// </summary>
            /// <param name="adapter">The adapter.</param>
            /// <typeparam name="T">The type the adapter adapts to.</typeparam>
            public static void RegisterTypeAdapter<T>(ITypeAdapter adapter)
            {
                WithService<ITypeAdapterRegistry>(r => r.RegisterAdapter<T>(adapter));
            }


            /// <summary>
            /// Adds a rule to the command parser.
            /// </summary>
            /// <param name="ruleType">The type of the rule to add.</param>
            public static void AddRule(Type ruleType)
            {
                WithService<ICommandParser>(p => p.RegisterParseRule(ruleType));
            }


            /// <summary>
            /// Removes a rule from the command parser.
            /// </summary>
            /// <param name="ruleType">The type of the rule to remove.</param>
            public static void RemoveRule(Type ruleType)
            {
                WithService<ICommandParser>(p => p.UnregisterParseRule(ruleType));
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
            /// Gets the fully qualified name for a command.
            /// </summary>
            /// <param name="commandType">The command type to get the name for.</param>
            /// <returns>The fully qualified name.</returns>
            public static string GetFullyQualifiedName(Type commandType)
            {
                return WithService<ICommandRegistry, string>(r => r.GetFullyQualifiedCommandName(commandType));
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


            /// <summary>
            /// Registers a command to the kernel's registry. Only persists until the domain is reloaded.
            /// </summary>
            /// <typeparam name="T">The type of the command to register.</typeparam>
            public static void RegisterCommand<T>() where T : ICommand
            {
                RegisterCommand(typeof(T));
            }


            /// <summary>
            /// Registers a command to the kernel's registry. Only persists until the domain is reloaded.
            /// </summary>
            /// <param name="type">The type of the command to register.</param>
            public static void RegisterCommand(Type type)
            {
                if (!typeof(ICommand).IsAssignableFrom(type))
                {
                    Log.Error($"Cannot register type {type.Name} because it is not an ICommand.");
                    return;
                }

                WithService<ICommandRegistry>(r => r.RegisterCommand(type));
            }


            // TODO: No longer accurate
            /// <summary>
            /// Gets all command names.
            /// </summary>
            /// <returns>A list of all names.</returns>
            public static List<string> GetAllCommandNames()
            {
                return WithService<ICommandRegistry, List<string>>(r => r.AllCommandNames());
            }
        }


        /// <summary>
        /// A container for shell related API calls
        /// </summary>
        public static class Shell
        {
            /// <summary>
            /// Creates a new session for a human interface.
            /// </summary>
            /// <param name="defaultContext">The default IO interface.</param>
            public static void CreateShellSession(IOContext defaultContext)
            {
                WithService<IShellApplication>(s => s.CreateSession(defaultContext));
            }


            /// <summary>
            /// Gets a session by its id.
            /// </summary>
            /// <param name="sessionId">The id.</param>
            /// <returns>The session.</returns>
            public static ShellSession GetShellSession(Guid sessionId)
            {
                return WithService<IShellApplication, ShellSession>(s => s.GetSession(sessionId));
            }
        }


        /// <summary>
        /// A container for windowing related API calls
        /// </summary>
        public static class Windowing
        {
            /// <summary>
            /// Registers a window.
            /// </summary>
            /// <param name="window">The window.</param>
            public static void RegisterWindow(IWindow window)
            {
                WithService<IWindowManager>(m => m.RegisterWindow(window));
            }


            /// <summary>
            /// Unregisters a window.
            /// </summary>
            /// <param name="window">The window.</param>
            public static void UnregisterWindow(IWindow window)
            {
                WithService<IWindowManager>(m => m.UnregisterWindow(window));
            }
        }
    }
}
