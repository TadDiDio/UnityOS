using System.Threading.Tasks;

namespace DeveloperConsole
{
    [ConfirmBeforeExecuting]
    [Command("async", "Test async command", true)]
    public class AsyncTestCommand : AsyncCommand
    {
        public override async Task<CommandResult> ExecuteAsync(CommandContext context)
        {
            await Task.Delay(3000);
            return new CommandResult("Finished");
        }
    }
}