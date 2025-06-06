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
        public Func<ICommandParser> ConsoleParserFactory;
        public Func<IObjectBindingsProvider> ObjectBindingsFactory;

        public ConsoleRuntimeDependencies Create()
        {
            var deps = new ConsoleRuntimeDependencies
            {
                ObjectBindingsManager = ObjectBindingsFactory?.Invoke() ?? new ObjectBindingsManager(),
                WindowManager = WindowManagerFactory?.Invoke() ?? new WindowManager(),
                InputManager = InputManagerFactory?.Invoke() ?? new BufferedInputManager(),
                OutputManager = OutputManagerFactory?.Invoke() ?? new OutputManager(),
                AutoRegistration = AutoRegistrationFactory?.Invoke() ?? new AutoRegistration(),
                TokenizationManager = TokenizationManagerFactory?.Invoke() ?? new TokenizationManager()
            };

            // Compose dependent ones after the basics exist
            deps.TypeParserRegistry = TypeParserRegistryFactory?.Invoke() ?? new TypeParserRegistry(deps.OutputManager);
            deps.CommandRegistry = CommandRegistryFactory?.Invoke() ?? new CommandRegistry(deps.AutoRegistration.AllCommands(new ReflectionAutoRegistration()));
            deps.CommandParser = ConsoleParserFactory?.Invoke() ?? new CommandParser(deps.CommandRegistry);

            // Register type parsers
            deps.TypeParserRegistry.RegisterTypeParser<int>(new IntParser());;
            deps.TypeParserRegistry.RegisterTypeParser<bool>(new BoolParser());
            deps.TypeParserRegistry.RegisterTypeParser<float>(new FloatParser());
            deps.TypeParserRegistry.RegisterTypeParser<Color>(new ColorParser());
            deps.TypeParserRegistry.RegisterTypeParser<string>(new StringParser());
            deps.TypeParserRegistry.RegisterTypeParser<Type>(new TypeTypeParser());
            deps.TypeParserRegistry.RegisterTypeParser<Vector2>(new Vector2Parser());
            deps.TypeParserRegistry.RegisterTypeParser<Vector3>(new Vector3Parser());
     
            return deps;
        }
    }

    public class ConsoleRuntimeDependencies
    {
        public IObjectBindingsProvider ObjectBindingsManager;
        public IWindowManager WindowManager;
        public IInputManager InputManager;
        public IOutputManager OutputManager;
        public IAutoRegistrationProvider AutoRegistration;
        public ITokenizationManager TokenizationManager;
        public ITypeParserRegistryProvider TypeParserRegistry;
        public ICommandRegistryProvider CommandRegistry;
        public ICommandParser CommandParser;
    }
}