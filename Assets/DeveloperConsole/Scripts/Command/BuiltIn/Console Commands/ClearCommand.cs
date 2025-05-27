namespace DeveloperConsole
{
    public class ClearCommand : ConsoleCommand
    {
        protected override string Name() => "clear";
        protected override string Description() => "Clears the console";
        protected override CommandResult Execute(ConsoleCommandArgs args)
        {
            args.ConsoleState.OutputBuffer.Clear();
            return new CommandResult();
        }
    }
}