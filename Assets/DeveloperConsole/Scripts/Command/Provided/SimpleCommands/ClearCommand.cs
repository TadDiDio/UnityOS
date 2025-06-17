using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("Clear", "Clears the terminal.")]
    public class ClearCommand : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            context.Session.OutputBuffer.Clear();
            return new();
        }
    }
}