using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// A simple command which exhibits one-shot behavior.
    /// </summary>
    public abstract class SimpleCommand : CommandBase
    {
        public override async Task<CommandOutput> ExecuteAsync(CommandContext context)
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