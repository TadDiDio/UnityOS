using DeveloperConsole.Command;
using DeveloperConsole.Core.Kernel;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// Interface for shell applications.
    /// </summary>
    public interface IShellApplication : IKernelApplication
    {
        public IInputManager InputManager { get; }
        public IOutputManager OutputManager { get; }
        
        /// <summary>
        /// Creates a new, unique session.
        /// </summary>
        /// <returns>The session.</returns>
        public ShellSession CreateSession();
        
        
        /// <summary>
        /// Handles a command request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HandleCommandRequestAsync(CommandRequest request);
    }
}