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

        public void Write(string message)
        {
            foreach (var outputChannel in _outputChannels)
            {
                outputChannel.Write(message);
            }
        }

        public void OverWrite(string message)
        {
            foreach (var outputChannel in _outputChannels)
            {
                outputChannel.OverWrite(message);
            }
        }

        public void WriteLine(string line)
        {
            foreach (var outputChannel in _outputChannels)
            {
                outputChannel.WriteLine(line);
            }
        }
    }
}
