using System.Collections.Generic;

namespace DeveloperConsole.IO
{
    public class CompositeOutputChannel : IOutputChannel
    {
        private List<IOutputChannel> _outputChannels;

        public CompositeOutputChannel(List<IOutputChannel> outputChannels)
        {
            _outputChannels = outputChannels;
        }

        public void Write(string token)
        {
            foreach (var outputChannel in _outputChannels)
            {
                outputChannel.Write(token);
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
