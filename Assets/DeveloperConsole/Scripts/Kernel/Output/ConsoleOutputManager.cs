using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class ConsoleOutputManager
    {
        private static List<IConsoleOutputSink> _outputSinks = new();

        public static void RegisterOutputSink(IConsoleOutputSink outputSink)
        {
            if (_outputSinks.Contains(outputSink))
            {
                // TODO: Warning to console
                return;
            }
            
            _outputSinks.Add(outputSink);
        }

        public static void UnregisterOutputSink(IConsoleOutputSink outputSink)
        {
            if (!_outputSinks.Contains(outputSink))
            {
                // TODO: Warning to console
                return;
            }
            
            _outputSinks.Remove(outputSink);
        }
        
        public static void UnregisterAllOutputSinks() => _outputSinks.Clear();
        
        public static void SendOutput(string message)
        {
            foreach (var output in _outputSinks) output.ReceiveOutput(message);
        }
    }
}