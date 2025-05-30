namespace DeveloperConsole
{
    [Command("clear", "Clears the console")]
    public class ClearCommand : ConsoleCommand
    {
        protected override CommandResult Execute(ConsoleCommandArgs args)
        {
            args.ConsoleState.OutputBuffer.Clear();
            return new CommandResult();
        }
    }
}