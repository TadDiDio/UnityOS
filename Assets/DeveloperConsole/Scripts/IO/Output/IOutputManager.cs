using System.Collections.Generic;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// Distributes outputs to the correct sinks.
    /// </summary>
    public interface IOutputManager
    {
        /// <summary>
        /// Global list of all registered output sinks.
        /// </summary>
        public List<IOutputSink> OutputSinks { get; }
        
        
        /// <summary>
        /// Registers a new output sink.
        /// </summary>
        /// <param name="sink">The sink.</param>
        public void RegisterOutputSink(IOutputSink sink);
        
        
        /// <summary>
        /// Unregisters an output sink.
        /// </summary>
        /// <param name="sink">The sink.</param>
        public void UnregisterOutputSink(IOutputSink sink);
        
        
        /// <summary>
        /// Unregisters all output sinks.
        /// </summary>
        public void UnregisterAllOutputSinks();
        
        
        /// <summary>
        /// Broadcasts a new message to all appropriate sinks.
        /// </summary>
        /// <param name="output">The message.</param>
        public void Emit(IOutputMessage output);
    }
}