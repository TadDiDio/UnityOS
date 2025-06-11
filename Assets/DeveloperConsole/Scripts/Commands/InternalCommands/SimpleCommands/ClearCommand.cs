namespace DeveloperConsole
{
    [Command("clear", "Clears the console", true)]
    public class ClearCommand : SimpleCommand
    {
        protected override CommandResult Execute(CommandContext context)
        {
            context.ConsoleState?.OutputBuffer.Clear();
            return new CommandResult();
        }
    }
}