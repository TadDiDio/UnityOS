using System;

namespace DeveloperConsole
{
    public static class ConsoleAPI
    {
        public static KernelWrapper Kernel = new();
        public static ParserWrapper Parser = new();
        
        public static InputWrapper Input = new();
        public static OutputWrapper Output = new();
        
        public static GraphicsWrapper Graphics = new();
        
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
            public void RegisterSource(IConsoleInputSource source) => ConsoleInputManager.RegisterInputMethod(source);
            public void UnregisterSource(IConsoleInputSource source) => ConsoleInputManager.UnregisterInputMethod(source);
        }
        
        public class OutputWrapper
        {
            public void RegisterSink(IConsoleOutputSink sink) => ConsoleOutputManager.RegisterOutputSink(sink);
            public void UnregisterSink(IConsoleOutputSink sink) => ConsoleOutputManager.UnregisterOutputSink(sink);
        }
        
        public class GraphicsWrapper
        {
            public void RegisterComponent(IGraphical graphical) => GraphicsManager.Register(graphical);
            public void UnregisterComponent(IGraphical graphical) => GraphicsManager.Unregister(graphical);
        }
        
        public static void SetTokenizer(ITokenizer tokenizer) => TokenizationManager.SetTokenizer(tokenizer);
    }
}