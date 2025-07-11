using System;

namespace DeveloperConsole.IO
{
    public interface IInputChannel
    {
        public Action<IInput> OnInputSubmitted { get; set; }
    }
}
