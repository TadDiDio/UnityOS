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
            await Task.Delay(3000);
            return new CommandOutput("Finished");
        }
    }
}