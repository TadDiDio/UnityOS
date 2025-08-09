using System;
using DeveloperConsole.Command;
using DeveloperConsole.Bindings;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing;
using DeveloperConsole.Windowing;
using DeveloperConsole.Parsing.Tokenizing;
using DeveloperConsole.Parsing.TypeAdapting;

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
        /// The command executor.
        /// </summary>
        public ICommandExecutor CommandExecutor;

        /// <summary>
        /// The parser.
        /// </summary>
        public ICommandParser CommandParser;

        /// <summary>
        /// The command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry;

        /// <summary>
        /// The type adapter registry.
        /// </summary>
        public ITypeAdapterRegistry TypeAdapterRegistry;

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
        /// Creates a command executor.
        /// </summary>
        public Func<ICommandExecutor> CommandExecutorFactory;

        /// <summary>
        /// Creates a window manager.
        /// </summary>
        public Func<IWindowManager> WindowManagerFactory;

        /// <summary>
        /// Creates a type adapter registry.
        /// </summary>
        public Func<ITypeAdapterRegistry> TypeAdapterRegistryFactory;

        /// <summary>
        /// Creates a command registry.
        /// </summary>
        public Func<ICommandRegistry> CommandRegistryFactory;

        /// <summary>
        /// Creates a parser.
        /// </summary>
        public Func<ICommandParser> ConsoleParserFactory;

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
                CommandExecutor = CommandExecutorFactory?.Invoke() ?? new CommandExecutor(),
                CommandParser = ConsoleParserFactory?.Invoke() ?? new CommandParser(new DefaultTokenizer()),
                CommandRegistry = CommandRegistryFactory?.Invoke() ?? new CommandRegistry(new ReflectionCommandDiscovery()),
                TypeAdapterRegistry = TypeAdapterRegistryFactory?.Invoke() ?? new TypeAdapterRegistry(),
                ObjectBindingsManager = ObjectBindingsFactory?.Invoke() ?? new ObjectBindingsManager(),
                WindowManager = WindowManagerFactory?.Invoke() ?? new WindowManager()
            };

            // Compose complex components
            container.Shell = ShellApplicationFactory?.Invoke() ?? new ShellApplication(container.CommandExecutor, container.WindowManager);

            return container;
        }
    }
}
