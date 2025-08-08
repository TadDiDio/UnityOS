using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [ExcludeFromCmdRegistry(true)]
    [Command("fish", "Easter egg for Bucket of Fish team.")]
    public class FishCommand : AsyncCommand
    {
        public override async Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                context.Session.WriteLine("You a good lookin' fish.");
                await Task.Delay(10, cancellationToken);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
