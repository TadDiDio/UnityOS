using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("prompt", "Enters a new prompt to test features.")]
    public class PromptCommand : AsyncCommand
    {
        protected override async Task<CommandOutput> Execute(AsyncCommandContext context, CancellationToken cancellationToken)
        {
            using (PushPromptForScope("prompt"))
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var graph = await PromptForCommand(cancellationToken);

                    await RunCommandGraph(graph, cancellationToken);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
