using DeveloperConsole.Command;

namespace DeveloperConsole.Core
{
    /// <summary>
    /// Interface for shell applications.
    /// </summary>
    public interface IShellApplication : IKernelApplication
    {
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