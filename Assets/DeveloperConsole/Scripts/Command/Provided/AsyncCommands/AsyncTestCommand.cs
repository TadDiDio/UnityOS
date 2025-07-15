using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [ConfirmBeforeExecuting]
    [Command("async", "Test async command")]
    public class AsyncTestCommand : AsyncCommand
    {
        public override async Task<CommandOutput> ExecuteAsync(CommandContext context)
        {
            context.Session.WriteLine("Beginning timer");
            for (int i = 3; i > 0; i--)
            {
                context.Session.WriteLine(i);
                await Task.Delay(1000);
            }
            return new CommandOutput("Finished");
        }
    }
}
