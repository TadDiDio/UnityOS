using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Kernel;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// Interface for shell applications.
    /// </summary>
    public interface IShellApplication : IKernelApplication
    {
        /// <summary>
        /// Creates a new shell session.
        /// </summary>
        /// <param name="defaultContext">The default IO context attached to this session.</param>
        public void CreateSession(IOContext defaultContext);


        /// <summary>
        /// Handles a command request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="userToken">The commands cancellation token.</param>
        /// <param name="ioContext">The IO Context to use.</param>
        public Task<CommandExecutionResult> HandleCommandRequestAsync
        (
            ShellRequest request,
            CancellationToken userToken,
            IOContext ioContext
        );


        /// <summary>
        /// Gets the session matching the id.
        /// </summary>
        /// <param name="sessionId">The id.</param>
        /// <returns>The session.</returns>
        public ShellSession GetSession(Guid sessionId);
    }
}
