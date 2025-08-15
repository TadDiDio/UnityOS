using System.Collections.Generic;

namespace DeveloperConsole.IO
{
    public class CompositeOutputChannel : IOutputChannel
    {
        private List<IOutputChannel> _outputChannels;

        public CompositeOutputChannel(List<IOutputChannel> outputChannels)
        {
            _outputChannels = outputChannels ?? new List<IOutputChannel>();
        }

        public void Write(string message, bool overwrite = false)
        {
            foreach (var o in _outputChannels) o.Write(message, overwrite);
        }

        public void WriteLine(string line)
        {
            foreach (var o in _outputChannels) o.WriteLine(line);
        }
    }
}
