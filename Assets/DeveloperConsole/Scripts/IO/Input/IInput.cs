using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// Interface for all system inputs.
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// The session associated with the input source that created this input.
        /// </summary>
        public ShellSession ShellSession { get; }
        
        
        /// <summary>
        /// Transforms this input into a command request.
        /// </summary>
        /// <returns>The command request.</returns>
        public CommandRequest GetCommandRequest();
    }
}