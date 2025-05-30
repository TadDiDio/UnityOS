using System;
using UnityEngine;

namespace DeveloperConsole
{
    public class ConsoleConfiguration
    {
        // Factories can be overriden for custom injection
        public Func<IWindowManager> WindowManagerFactory;
        public Func<IInputManager> InputManagerFactory;
        public Func<IOutputManager> OutputManagerFactory;
        public Func<IAutoRegistrationProvider> AutoRegistrationFactory;
        public Func<ITokenizationManager> TokenizationManagerFactory;
        public Func<ITypeParserRegistryProvider> TypeParserRegistryFactory;
        public Func<ICommandRegistryProvider> CommandRegistryFactory;
        public Func<IConsoleParser> ConsoleParserFactory;

        public ConsoleRuntimeDependencies Create()
        {
            var deps = new ConsoleRuntimeDependencies
            {
                WindowManager = WindowManagerFactory?.Invoke() ?? new WindowManager(),
                InputManager = InputManagerFactory?.Invoke() ?? new InputManager(),
                OutputManager = OutputManagerFactory?.Invoke() ?? new OutputManager(),
                AutoRegistration = AutoRegistrationFactory?.Invoke() ?? new AutoRegistration(),
                TokenizationManager = TokenizationManagerFactory?.Invoke() ?? new TokenizationManager()
            };

            // Compose dependent ones after the basics exist
            deps.TypeParserRegistry = TypeParserRegistryFactory?.Invoke() ?? new TypeParserRegistry(deps.OutputManager);
            deps.CommandRegistry = CommandRegistryFactory?.Invoke() ?? new CommandRegistry(deps.AutoRegistration.AllCommands());
            deps.ConsoleParser = ConsoleParserFactory?.Invoke() ?? new ConsoleParser(deps.CommandRegistry);

            // Register type parsers
            deps.TypeParserRegistry.RegisterTypeParser<int>(new IntParser());;
            deps.TypeParserRegistry.RegisterTypeParser<float>(new FloatParser());
            deps.TypeParserRegistry.RegisterTypeParser<string>(new StringParser());
            deps.TypeParserRegistry.RegisterTypeParser<bool>(new BoolParser());
            deps.TypeParserRegistry.RegisterTypeParser<Vector2>(new Vector2Parser());
            deps.TypeParserRegistry.RegisterTypeParser<Vector3>(new Vector3Parser());
            deps.TypeParserRegistry.RegisterTypeParser<Color>(new ColorParser());
            deps.TypeParserRegistry.RegisterTypeParser<Color>(new AlphaColorParser());
     
            return deps;
        }
    }

    public class ConsoleRuntimeDependencies
    {
        public IWindowManager WindowManager;
        public IInputManager InputManager;
        public IOutputManager OutputManager;
        public IAutoRegistrationProvider AutoRegistration;
        public ITokenizationManager TokenizationManager;
        public ITypeParserRegistryProvider TypeParserRegistry;
        public ICommandRegistryProvider CommandRegistry;
        public IConsoleParser ConsoleParser;
    }
}