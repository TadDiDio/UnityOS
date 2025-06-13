using System;
using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole.IO
{
    public interface IInputManager
    {
        public List<IInputSource> InputSources { get; }
        public Action<CommandRequest> OnCommandInput { get; set; }
        
        
        public void RegisterInputSource(IInputSource source);
        public void UnregisterInputSource(IInputSource source);
        public void UnregisterAllInputSources();
    }
}