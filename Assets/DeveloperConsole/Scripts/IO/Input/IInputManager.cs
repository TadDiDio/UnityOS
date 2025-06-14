using System;
using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// Interface for input managers.
    /// </summary>
    public interface IInputManager
    {
        /// <summary>
        /// Global list of all input sources in the system.
        /// </summary>
        public List<IInputSource> InputSources { get; }
        
        
        /// <summary>
        /// Global event for input entering as a command request. This is
        /// always the case unless a session is waiting for input.
        /// </summary>
        public Action<CommandRequest> OnCommandInput { get; set; }
        
        
        /// <summary>
        /// Registers a new input source.
        /// </summary>
        /// <param name="source">The source.</param>
        public void RegisterInputSource(IInputSource source);
        
        
        /// <summary>
        /// Unregisters an input source.
        /// </summary>
        /// <param name="source">The input source.</param>
        public void UnregisterInputSource(IInputSource source);
     
        
        /// <summary>
        /// Unregisters all input sources.
        /// </summary>
        public void UnregisterAllInputSources();
    }
}