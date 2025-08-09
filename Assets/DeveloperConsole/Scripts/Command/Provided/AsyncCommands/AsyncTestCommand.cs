using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("async", "Test async command")]
    public class AsyncTestCommand : AsyncCommand
    {
        protected override async Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            WriteLine("Beginning timer");
            for (int i = 3; i > 0; i--)
            {
                WriteLine(i);
                await Task.Delay(1000, cancellationToken);
            }
            return new CommandOutput("Finished");
        }
    }
}
