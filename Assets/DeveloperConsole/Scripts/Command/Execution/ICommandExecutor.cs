using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Interface for command executors.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="executionRequest">The request.</param>
        /// <param name="ioContext">The IO context this command should use.</param>
        /// <param name="cancellationToken">The command's cancellation token.</param>
        /// <returns>The execution result.</returns>
        public Task<Status> ExecuteCommand(ShellRequest executionRequest, IOContext ioContext, CancellationToken cancellationToken);
    }
}
