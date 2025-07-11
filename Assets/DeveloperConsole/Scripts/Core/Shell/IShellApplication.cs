using System;
using System.Collections.Generic;
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
        public Guid CreateSession(IShellClient client, List<IInputChannel> extraInputs = null, List<IOutputChannel> extraOutputs = null);


        /// <summary>
        /// Handles a command request.
        /// </summary>
        /// <param name="request">The request.</param>
        public Task<CommandExecutionResult> HandleCommandRequestAsync(ShellRequest request);


        /// <summary>
        /// Gets the session matching the id.
        /// </summary>
        /// <param name="sessionId">The id.</param>
        /// <returns>The session.</returns>
        public ShellSession GetSession(Guid sessionId);
    }
}
