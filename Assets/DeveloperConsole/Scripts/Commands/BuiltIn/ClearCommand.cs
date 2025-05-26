namespace DeveloperConsole.BuiltIn
{
    public class ClearCommand : BuiltInCommand
    {
        protected override string Name() => "clear";
        protected override string Description() => "Clears the console";
        protected override CommandResult Execute(BuiltInCommandArgs args)
        {
            args.ConsoleState.OutputBuffer.Clear();
            return new CommandResult();
        }
    }
}