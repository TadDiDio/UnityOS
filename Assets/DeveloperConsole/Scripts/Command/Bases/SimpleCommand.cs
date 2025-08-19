using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// A simple command which exhibits one-shot behavior.
    /// </summary>
    public abstract class SimpleCommand : CommandBase
    {
        protected override async Task<CommandOutput> ExecuteAsync(FullCommandContext fullContext, CancellationToken cancellationToken)
        {
            SimpleContext context = new SimpleContext
            (
                new RestrictedSessionContext(fullContext.Session.AliasManager),
                fullContext.Environment
            );

            return await Task.FromResult(Execute(context));
        }


        /// <summary>
        /// Defines the synchronous execution logic for this command.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns>The command's output.</returns>
        protected abstract CommandOutput Execute(SimpleContext context);
    }
}
