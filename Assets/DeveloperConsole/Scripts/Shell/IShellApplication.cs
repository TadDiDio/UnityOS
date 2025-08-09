using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        /// <summary>
        /// Creates a new shell session.
        /// </summary>
        /// <param name="client">The client attached to this session.</param>
        /// <param name="extraInputs">Any input channels in addition to the client.</param>
        /// <param name="extraOutputs">Any output channels in addition to the client.</param>
        /// <returns>The session id.</returns>
        public Guid CreateSession(IPromptResponder client, List<IOutputChannel> extraOutputs = null);


        /// <summary>
        /// Creates a new human facing shell session.
        /// </summary>
        /// <param name="humanInterface">The human interface.</param>
        /// <returns>The session id.</returns>
        public Guid CreateSession(IHumanInterface humanInterface);


        /// <summary>
        /// Handles a command request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="userToken">The commands cancellation token.</param>
        /// <param name="defaultUserInterface">The default interface to use.</param>
        public Task<CommandExecutionResult> HandleCommandRequestAsync(
            ShellRequest request,
            CancellationToken userToken,
            UserInterface defaultUserInterface);


        /// <summary>
        /// Gets the session matching the id.
        /// </summary>
        /// <param name="sessionId">The id.</param>
        /// <returns>The session.</returns>
        public ShellSession GetSession(Guid sessionId);
    }
}
