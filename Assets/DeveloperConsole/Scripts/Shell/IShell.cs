using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// Interface for shell applications.
    /// </summary>
    public interface IShell
    {
        /// <summary>
        /// Creates a new shell session.
        /// </summary>
        /// <param name="client">The default client for this shell session.</param>
        public void CreateSession(IShellClient client);


        /// <summary>
        /// Handles a command request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ioContext">The IO Context to use.</param>
        /// <param name="userToken">The commands cancellation token.</param>
        public Task<Status> HandleCommandRequestAsync
        (
            ShellRequest request,
            IOContext ioContext,
            CancellationToken userToken
        );


        /// <summary>
        /// Gets the session matching the id.
        /// </summary>
        /// <param name="sessionId">The id.</param>
        /// <returns>The session.</returns>
        public ShellSession GetSession(Guid sessionId);
    }
}
