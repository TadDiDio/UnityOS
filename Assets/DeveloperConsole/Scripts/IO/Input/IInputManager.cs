using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public interface IInputManager
    {
        public event Action<string> InputSubmitted;
        public List<IInputSource> InputSources { get; }
        public void RegisterInputSource(IInputSource source);
        public void UnregisterInputSource(IInputSource source);
        public void UnregisterAllInputSources();
    }
}