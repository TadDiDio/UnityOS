using System;
using DeveloperConsole.IO;
using DeveloperConsole.Command;
using DeveloperConsole.Bindings;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing;
using DeveloperConsole.Windowing;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole
{
    /// <summary>
    /// Container holding all injected runtime dependencies.
    /// </summary>
    public class DependenciesContainer
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
    public class DependenciesFactory
    {
        /// <summary>
        /// Creates a shell.
        /// </summary>
        public Func<IShellApplication> ShellApplicationFactory;
        
        /// <summary>
        /// Creates an input manager.
        /// </summary>
        public Func<IInputManager> InputManagerFactory;
        
        /// <summary>
        /// Creates an output manager.
        /// </summary>
        public Func<IOutputManager> OutputManagerFactory;
        
        /// <summary>
        /// Creates a command executor.
        /// </summary>
        public Func<ICommandExecutor> CommandExecutorFactory;
        
        /// <summary>
        /// Creates a window manager.
        /// </summary>
        public Func<IWindowManager> WindowManagerFactory;
        
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
        public DependenciesContainer Create()
        {
            // Instantiate basic components
            var container = new DependenciesContainer
            {
                OutputManager = OutputManagerFactory?.Invoke() ?? new OutputManager(),
                CommandExecutor = CommandExecutorFactory?.Invoke() ?? new CommandExecutor(),
                Parser = ConsoleParserFactory?.Invoke() ?? new Parser(new DefaultTokenizer()),
                CommandRegistry = CommandRegistryFactory?.Invoke() ?? new CommandRegistry(new ReflectionCommandDiscovery()),
                TypeParserRegistry = TypeParserRegistryFactory?.Invoke() ?? new TypeParserRegistry(),
                ObjectBindingsManager = ObjectBindingsFactory?.Invoke() ?? new ObjectBindingsManager(),
            };

            // Compose complex components
            container.InputManager = InputManagerFactory?.Invoke() ?? new InputManager(container.OutputManager);
            container.Shell = ShellApplicationFactory?.Invoke() ??
                  new ShellApplication
                  (
                      container.CommandExecutor, 
                      container.InputManager,
                      container.OutputManager
                  );
            container.WindowManager = WindowManagerFactory?.Invoke() ?? new WindowManager(container.Shell);

            return container;
        }
    }
}