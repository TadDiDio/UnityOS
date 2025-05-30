using System.Collections.Generic;

namespace DeveloperConsole
{
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
            if (!OutputSinks.Contains(outputSink))
            {
                // TODO: Warning to console
                return;
            }
            
            OutputSinks.Remove(outputSink);
        }

        public void UnregisterAllOutputSinks()
        {
            OutputSinks.Clear();
        }

        public void SendOutput(string message)
        {
            foreach (var output in OutputSinks) output.ReceiveOutput(message);
        }
    }
}