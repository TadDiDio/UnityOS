using System;
using System.Collections.Generic;

namespace DeveloperConsole.IO
{
    public class CompositeInputChannel : IInputChannel, IDisposable
    {
        public Action<IInput> OnInputSubmitted { get; set; }

        private List<IInputChannel> _inputChannels;

        public CompositeInputChannel(List<IInputChannel> inputChannels)
        {
            _inputChannels = inputChannels;

            foreach (var inputChannel in _inputChannels)
            {
                inputChannel.OnInputSubmitted += InputSubmitted;
            }
        }

        private void InputSubmitted(IInput input)
        {
            OnInputSubmitted?.Invoke(input);
        }

        public void Dispose()
        {
            foreach (var inputChannel in _inputChannels)
            {
                inputChannel.OnInputSubmitted -= InputSubmitted;
            }
        }
    }
}
