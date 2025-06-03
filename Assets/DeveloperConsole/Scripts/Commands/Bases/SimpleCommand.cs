using System.Threading.Tasks;

namespace DeveloperConsole
{
    /// <summary>
    /// A simple command which exhibits one-shot behavior.
    /// </summary>
    public abstract class SimpleCommand : CommandBase
    {
        public override async Task<CommandResult> ExecuteAsync(CommandContext context)
        {
            return await Task.FromResult(Execute(context));
        }
        protected abstract CommandResult Execute(CommandContext context);
    }
}