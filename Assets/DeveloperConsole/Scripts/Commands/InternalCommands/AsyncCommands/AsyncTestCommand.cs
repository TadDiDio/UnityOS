using System.Threading.Tasks;

namespace DeveloperConsole
{
    [Command("async", "Test async command", true)]
    [ComfirmBeforeExecuting]
    public class AsyncTestCommand : AsyncCommand
    {
        public override async Task<CommandResult> ExecuteAsync(CommandContext context)
        {
            var message = await context.Shell.WaitForInput("Enter a message");
            return new CommandResult("Received input: " + message);
        }
    }
}