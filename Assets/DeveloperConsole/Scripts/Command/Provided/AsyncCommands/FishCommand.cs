using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [ExcludeFromCmdRegistry(true)]
    [Command("fish", "Easter egg for Bucket of Fish team.")]
    public class FishCommand : AsyncCommand
    {
        protected override async Task<CommandOutput> Execute(AsyncCommandContext context, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                WriteLine("You a good lookin' fish!");

                await Task.Delay(10, cancellationToken);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
