using System;

namespace DeveloperConsole
{
    public static class ConsoleAPI
    {
        public static readonly KernelWrapper Kernel = new();
        public static readonly ParserWrapper Parser = new();
        
        public static readonly InputWrapper Input = new();
        public static readonly OutputWrapper Output = new();
        
        public static readonly GraphicsWrapper Graphics = new();
        
        public class KernelWrapper
        {
            public void RegisterTickable(ITickable tickable) => ConsoleKernel.RegisterTickable(tickable);
            public void UnregisterTickable(ITickable tickable) => ConsoleKernel.UnregisterTickable(tickable);
        }
        
        public class ParserWrapper
        {
            public bool TryParse<T>(Type type, TokenStream stream, out object obj) => ConsoleParser.TryParse(type, stream, out obj);
            public void RegisterTypeParser<T>(BaseTypeParser parser) => TypeParserRegistry.RegisterTypeParser<T>(parser);
        }

        public class InputWrapper
        {
            public void RegisterSource(IConsoleInputSource source) => InputManager.RegisterInputMethod(source);
            public void UnregisterSource(IConsoleInputSource source) => InputManager.UnregisterInputMethod(source);
        }
        
        public class OutputWrapper
        {
            public void RegisterSink(IConsoleOutputSink sink) => OutputManager.RegisterOutputSink(sink);
            public void UnregisterSink(IConsoleOutputSink sink) => OutputManager.UnregisterOutputSink(sink);
        }
        
        public class GraphicsWrapper
        {
            public void RegisterComponent(IGraphical graphical) => GraphicsManager.Register(graphical);
            public void UnregisterComponent(IGraphical graphical) => GraphicsManager.Unregister(graphical);
        }
        
        public static void SetTokenizer(ITokenizer tokenizer) => TokenizationManager.SetTokenizer(tokenizer);
    }
}