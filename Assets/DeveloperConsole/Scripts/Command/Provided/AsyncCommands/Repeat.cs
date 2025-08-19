using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("repeat", "Repeats the input given.")]
    public class Repeat : AsyncCommand
    {
        protected override async Task<CommandOutput> Execute(AsyncCommandContext context, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var message = await PromptAsync<string>("", cancellationToken);

                WriteLine("Repeated: " + message);
            }
        }
    }
}
