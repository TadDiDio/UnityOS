using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Simply a more ergonomic name for an async command.
    /// </summary>
    public abstract class AsyncCommand : ICommand
    {
        public abstract Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken);
    }
}
