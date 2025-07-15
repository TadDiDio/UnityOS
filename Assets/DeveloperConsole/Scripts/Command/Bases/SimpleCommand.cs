using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// A simple command which exhibits one-shot behavior.
    /// </summary>
    public abstract class SimpleCommand : AsyncCommand
    {
        public override async Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Execute(context));
        }


        /// <summary>
        /// Defines the synchronous execution logic for this command.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns>The command's output.</returns>
        protected abstract CommandOutput Execute(CommandContext context);
    }
}
