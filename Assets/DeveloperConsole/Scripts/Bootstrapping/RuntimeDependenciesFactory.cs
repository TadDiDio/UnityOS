using System;
using DeveloperConsole.IO;
using DeveloperConsole.Command;
using DeveloperConsole.Bindings;
using DeveloperConsole.Core;
using DeveloperConsole.Parsing;
using DeveloperConsole.Windowing;
using DeveloperConsole.Parsing.Tokenizing;

// TODO: Instantiate all componentes
namespace DeveloperConsole
{
    /// <summary>
    /// Container holding all injected runtime dependencies.
    /// </summary>
    public class ConsoleRuntimeDependencies
    {
        /// <summary>
        /// The shell.
        /// </summary>
        public IShellApplication Shell;
        
        
        /// <summary>
        /// The input manager.
        /// </summary>
        public IInputManager InputManager;
        
        
        /// <summary>
        /// The output manager.
        /// </summary>
        public IOutputManager OutputManager;
        
        
        /// <summary>
        /// The command executor.
        /// </summary>
        public ICommandExecutor CommandExecutor;
        
        
        /// <summary>
        /// The parser.
        /// </summary>
        public IParser Parser;
        
        
        /// <summary>
        /// The command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry;
        
        
        /// <summary>
        /// The type parser registry.
        /// </summary>
        public ITypeParserRegistryProvider TypeParserRegistry;
        
        /// <summary>
        /// The object bindings manager.
        /// </summary>
        public IObjectBindingsManager ObjectBindingsManager;
        
        
        /// <summary>
        /// The window manager.
        /// </summary>
        public IWindowManager WindowManager;
    }
    
    /// <summary>
    /// Factory for creating the kernel's dependency container.
    /// </summary>
    public class RuntimeDependenciesFactory
    {
        // Factories can be overriden for custom injection
        
        /// <summary>
        /// Creates a window manager.
        /// </summary>
        public Func<IWindowManager> WindowManagerFactory;
        
        
        /// <summary>
        /// Creates an input manager.
        /// </summary>
        public Func<IInputManager> InputManagerFactory;
        
        
        /// <summary>
        /// Creates an output manager.
        /// </summary>
        public Func<IOutputManager> OutputManagerFactory;
        
        
        /// <summary>
        /// Creates a type parser registry.
        /// </summary>
        public Func<ITypeParserRegistryProvider> TypeParserRegistryFactory;
        
        
        /// <summary>
        /// Creates a command registry.
        /// </summary>
        public Func<ICommandRegistry> CommandRegistryFactory;
        
        
        /// <summary>
        /// Creates a parser.
        /// </summary>
        public Func<IParser> ConsoleParserFactory;
        
        
        /// <summary>
        /// Creates an object bindings manager.
        /// </summary>
        public Func<IObjectBindingsManager> ObjectBindingsFactory;

        
        /// <summary>
        /// Creates the dependency container.
        /// </summary>
        /// <returns>The container.</returns>
        public ConsoleRuntimeDependencies Create()
        {
            // Instantiate basic components
            var container = new ConsoleRuntimeDependencies
            {
                ObjectBindingsManager = ObjectBindingsFactory?.Invoke() ?? new ObjectBindingsManager(),
                WindowManager = WindowManagerFactory?.Invoke() ?? new WindowManager(),
                InputManager = InputManagerFactory?.Invoke() ?? new InputManager(),
                OutputManager = OutputManagerFactory?.Invoke() ?? new OutputManager(),
                TypeParserRegistry = TypeParserRegistryFactory?.Invoke() ?? new TypeParserRegistry(),
                Parser = ConsoleParserFactory?.Invoke() ?? new Parser(new DefaultTokenizer()),
                CommandRegistry = CommandRegistryFactory?.Invoke() ?? new CommandRegistry(new ReflectionCommandDiscovery())
            };

            return container;
        }
    }
}