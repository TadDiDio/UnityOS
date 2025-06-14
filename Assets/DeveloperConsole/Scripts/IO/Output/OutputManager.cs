using System.Collections.Generic;

namespace DeveloperConsole.IO
{
    /// TODO: This doesn't happen yet lol
    /// <summary>
    /// An output manager that forwards messages to all subscribed sinks.
    /// </summary>
    public class OutputManager : IOutputManager
    {
        public List<IOutputSink> OutputSinks { get;  } = new();

        public void RegisterOutputSink(IOutputSink outputSink)
        {
            if (OutputSinks.Contains(outputSink))
            {
                // TODO: Warning to console
                return;
            }
            
            OutputSinks.Add(outputSink);
        }

        public void UnregisterOutputSink(IOutputSink outputSink)
        {
            OutputSinks.Remove(outputSink);
        }

        public void UnregisterAllOutputSinks()
        {
            OutputSinks.Clear();
        }

        public void Emit(IOutputMessage message)
        {
            foreach (var output in OutputSinks) output.ReceiveOutput(message);
        }
    }
}