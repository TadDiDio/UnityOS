using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("sleep", "Sleeps for some time.")]
    public class Sleep : AsyncCommand
    {
        [Positional(0, "How long to sleep in seconds.")]
        private float seconds;
        protected override async Task<CommandOutput> Execute(AsyncCommandContext context, CancellationToken cancellationToken)
        {
            await Task.Delay((int)(1000 * seconds), cancellationToken);
            return new();
        }
    }
}
