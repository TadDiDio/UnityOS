using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Defines the asynchronous execution logic for this command.
        /// </summary>
        /// <param name="context">The context of this execution.</param>
        /// <param name="cancellationToken">A cancellation token to stop execution.</param>
        /// <returns>The command's output.</returns>
        public Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken);
    }
}
