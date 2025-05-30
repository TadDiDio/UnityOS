using System.Collections.Generic;

namespace DeveloperConsole
{
    public interface IOutputManager
    {
        public List<IOutputSink> OutputSinks { get; }
        public void RegisterOutputSink(IOutputSink sink);
        public void UnregisterOutputSink(IOutputSink sink);
        public void UnregisterAllOutputSinks();
        public void SendOutput(string message);
    }
}