using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class OutputManager
    {
        public static List<IConsoleOutputSink> OutputSinks { get;  } = new();

        public static void Initialize()
        {
            StaticResetRegistry.Register(UnregisterAllOutputSinks);
        }
        
        public static void RegisterOutputSink(IConsoleOutputSink outputSink)
        {
            if (OutputSinks.Contains(outputSink))
            {
                // TODO: Warning to console
                return;
            }
            
            OutputSinks.Add(outputSink);
        }

        public static void UnregisterOutputSink(IConsoleOutputSink outputSink)
        {
            if (!OutputSinks.Contains(outputSink))
            {
                // TODO: Warning to console
                return;
            }
            
            OutputSinks.Remove(outputSink);
        }
        
        public static void UnregisterAllOutputSinks() => OutputSinks.Clear();
        
        public static void SendOutput(string message)
        {
            foreach (var output in OutputSinks) output.ReceiveOutput(message);
        }
    }
}