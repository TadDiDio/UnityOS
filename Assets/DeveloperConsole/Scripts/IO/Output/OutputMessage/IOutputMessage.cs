using DeveloperConsole.Core;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// Interface for all output messages.
    /// </summary>
    public interface IOutputMessage
    {
        // TODO: 
        //public string Channel { get; }
        
        /// <summary>
        /// The session this output came from.
        /// </summary>
        public ShellSession Session { get; }
    }
}